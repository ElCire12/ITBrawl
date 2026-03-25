using System.Collections.Generic;
using UnityEngine;

public class SupCam : MonoBehaviour
{
    public LevelBounds levelBounds;

    public List<GameObject> players;

    public float depthUpdateSpeed = 5f;
    public float angleUpdateSpeed = 7f;
    public float positionUpdateSpeed = 7f;

    public float depthMax = -10f;
    public float depthMin = -22f;

    public float angleMax = 11f;
    public float angleMin = 3f;

    private float CameraEulerX;
    private Vector3 cameraPosition;

    public float Yoffset = 4f;

    private void Start()
    {
        //players.Add(levelBounds.gameObject);
    }

    void Update()
    {
        CalculateCameraLocations();
        MoveCamera();
    }

    private void MoveCamera()
    {
        Vector3 position = transform.position;

        if (position != cameraPosition)
        {
            Vector3 targetPosition = new Vector3(
                Mathf.MoveTowards(position.x, cameraPosition.x, positionUpdateSpeed * Time.deltaTime),
                Mathf.MoveTowards(position.y, cameraPosition.y + Yoffset, positionUpdateSpeed * Time.deltaTime),
                Mathf.MoveTowards(position.z, cameraPosition.z, depthUpdateSpeed * Time.deltaTime)
            );

            transform.position = targetPosition;
        }

        Vector3 localEulerAngles = transform.localEulerAngles;

        if (localEulerAngles.x != CameraEulerX)
        {
            Vector3 targetEulerAngles = new Vector3(CameraEulerX, localEulerAngles.y, localEulerAngles.z);
            transform.localEulerAngles = Vector3.MoveTowards(localEulerAngles, targetEulerAngles, angleUpdateSpeed * Time.deltaTime);
        }
    }

    private void CalculateCameraLocations()
    {
        Vector3 averageCenter = Vector3.zero;
        Vector3 totalPositions = Vector3.zero;
        Bounds playerBounds = new Bounds();

        for (int i = 0; i < players.Count; i++)
        {
            Vector3 playerPosition = players[i].transform.position;

            if (!levelBounds.worldBounds.Contains(playerPosition))
            {
                float clampedX = Mathf.Clamp(playerPosition.x, levelBounds.worldBounds.min.x, levelBounds.worldBounds.max.x);
                float clampedY = Mathf.Clamp(playerPosition.y, levelBounds.worldBounds.min.y, levelBounds.worldBounds.max.y);
                float clampedZ = Mathf.Clamp(playerPosition.z, levelBounds.worldBounds.min.z, levelBounds.worldBounds.max.z);
                playerPosition = new Vector3(clampedX, clampedY, clampedZ);
            }

            totalPositions += playerPosition;
            playerBounds.Encapsulate(playerPosition);
        }

        averageCenter = totalPositions / players.Count;

        float extentSum = playerBounds.extents.x + playerBounds.extents.y;
        float maxExtent = (levelBounds.halfWidth + levelBounds.halfHeight) / 2;
        float lerpPercent = Mathf.InverseLerp(0, maxExtent, extentSum);

        float depth = Mathf.Lerp(depthMax, depthMin, lerpPercent);
        float angle = Mathf.Lerp(angleMax, angleMin, lerpPercent);

        CameraEulerX = angle;
        cameraPosition = new Vector3(averageCenter.x, averageCenter.y, depth);
    }
}

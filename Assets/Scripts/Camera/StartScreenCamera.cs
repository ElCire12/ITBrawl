using UnityEngine;
using System.Collections.Generic;

public class StartScreenCamera : MonoBehaviour
{
    [System.Serializable]
    public class CameraPoint
    {
        public Vector3 position;
        public Vector3 rotationEuler;
    }

    public List<CameraPoint> pathPoints = new List<CameraPoint>();
    public float transitionDuration = 3f; // Tiempo para moverse entre puntos

    private int currentIndex = 0;
    private int nextIndex = 1;
    private float t = 0f;

    private Transform camTransform;

    void Start()
    {
        if (pathPoints.Count < 2)
        {
            Debug.LogWarning("Se necesitan al menos 2 puntos.");
            enabled = false;
            return;
        }

        camTransform = Camera.main.transform;
        camTransform.position = pathPoints[0].position;
        camTransform.rotation = Quaternion.Euler(pathPoints[0].rotationEuler);
    }

    void Update()
    {
        if (pathPoints.Count < 2) return;

        t += Time.deltaTime / transitionDuration;
        if (t > 1f)
        {
            t = 0f;
            currentIndex = nextIndex;
            nextIndex = (nextIndex + 1) % pathPoints.Count;
        }

        Vector3 startPos = pathPoints[currentIndex].position;
        Vector3 endPos = pathPoints[nextIndex].position;

        Quaternion startRot = Quaternion.Euler(pathPoints[currentIndex].rotationEuler);
        Quaternion endRot = Quaternion.Euler(pathPoints[nextIndex].rotationEuler);

        camTransform.position = Vector3.Lerp(startPos, endPos, t);
        camTransform.rotation = Quaternion.Slerp(startRot, endRot, t);
    }
}

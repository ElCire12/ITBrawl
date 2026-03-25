using UnityEngine;

public class LevelBounds : MonoBehaviour
{
    [Header("Half-size of the level bounds (from center)")]
    public float halfWidth = 20f;
    public float halfHeight = 15f;
    public float halfDepth = 15f;

    [HideInInspector]
    public Bounds worldBounds;

    void Update()
    {
        Vector3 center = transform.position;

        Vector3 min = new Vector3(center.x - halfWidth, center.y - halfHeight, center.z - halfDepth);
        Vector3 max = new Vector3(center.x + halfWidth, center.y + halfHeight, center.z + halfDepth);

        worldBounds = new Bounds();
        worldBounds.Encapsulate(min);
        worldBounds.Encapsulate(max);
    }
}
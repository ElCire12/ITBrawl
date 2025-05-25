using System.Collections.Generic;
using UnityEngine;

public class SupCam2 : MonoBehaviour
{
    public static SupCam2 Instance { get; private set; }

    [Header("Referencias")]
    public LevelBounds levelBounds;
    public List<GameObject> players;

    [Header("Movimiento de la cámara")]
    public float followSpeed = 7f;
    public float zoomSpeed = 5f;
    public float rotationSpeed = 7f;
    public float yOffset = 4f;

    [Header("Profundidad (Zoom)")]
    // Valor de Z cuando los jugadores están muy juntos (más cerca de ellos)
    public float zoomInZ = -10f;
    // Valor de Z cuando los jugadores están muy separados (más lejos)
    public float zoomOutZ = -22f;

    [Header("Inclinación de la cámara (rotación X)")]
    // Ángulo cuando la cámara está alejada (más plana)
    public float angleLow = 3f;
    // Ángulo cuando la cámara está cerca (más inclinada)
    public float angleHigh = 11f;

    // Posición hacia la que se moverá la cámara
    private Vector3 targetPosition;
    // Ángulo X hacia el que rotará la cámara
    private float targetAngleX;


    private void Awake()
    {
        // Singleton no persistente
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        // Limpiar referencia si esta era la instancia activa
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Update()
    {
        UpdateCameraTarget();
        MoveCamera();
    }

    private void UpdateCameraTarget()
    {
        if (players.Count == 0) return;

        // Suma total de las posiciones de los jugadores
        Vector3 totalPosition = Vector3.zero;

        // Crea un Bounds que irá envolviendo a todos los jugadores
        Bounds playersBounds = new Bounds(players[0].transform.position, Vector3.zero);

        foreach (GameObject player in players)
        {
            // Corrige la posición del jugador si está fuera de los límites del nivel
            // Ej: si el jugador está en (25, 0, 5) y el límite es X: -20 a 20 → se corrige a (20, 0, 5)
            Vector3 pos = ClampToLevel(player.transform.position);

            // Suma su posición al total para luego calcular la media
            totalPosition += pos;
            // Expande el bounds para incluir este nuevo jugador
            playersBounds.Encapsulate(pos);
        }

        // Calcula el centro promedio de todos los jugadores
        Vector3 averagePosition = totalPosition / players.Count;

        // Calcula qué tan dispersos están los jugadores sumando la mitad del ancho + alto del bounding box que engloba todos los jugadores
        float playersSpread = playersBounds.extents.x + playersBounds.extents.y;

        // Define una separación máxima "razonable" basada en el tamaño del nivel
        float maxPlayerSpread = (levelBounds.halfWidth + levelBounds.halfHeight) * 0.5f;

        // Calcula un valor de interpolación (0 a 1) según cuán separados están los jugadores
        float proximityPercentage = Mathf.InverseLerp(0f, maxPlayerSpread, playersSpread);

        // Interpola la profundidad (zoom) según la separación
        float zoomZ = Mathf.Lerp(zoomInZ, zoomOutZ, proximityPercentage);

        // Interpola el ángulo de inclinación X de la cámara
        float angleX = Mathf.Lerp(angleHigh, angleLow, proximityPercentage); // Ojo: invertido (más alto = más plano)

        // Guarda la posición objetivo de la cámara, con un offset vertical
        targetPosition = new Vector3(averagePosition.x, averagePosition.y + yOffset, zoomZ);

        // Guarda el ángulo de rotación en X al que debe llegar la cámara
        targetAngleX = angleX;
    }

    // Asegura que una posición esté dentro de los límites del nivel
    private Vector3 ClampToLevel(Vector3 pos)
    {
        // Obtiene los límites del nivel
        Bounds bounds = levelBounds.worldBounds;

        // Clampa cada componente a los límites establecidos
        return new Vector3(
            Mathf.Clamp(pos.x, bounds.min.x, bounds.max.x),
            Mathf.Clamp(pos.y, bounds.min.y, bounds.max.y),
            Mathf.Clamp(pos.z, bounds.min.z, bounds.max.z)
        );
    }

    // Mueve y rota la cámara suavemente hacia los valores calculados
    private void MoveCamera()
    {
        // Desplaza la posición actual hacia la posición objetivo usando MoveTowards (suavizado lineal)
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Obtiene la rotación actual de la cámara
        Vector3 currentEuler = transform.localEulerAngles;

        // Define el ángulo deseado (solo cambiamos la rotación X)
        Vector3 targetEuler = new Vector3(targetAngleX, currentEuler.y, currentEuler.z);

        // Rota suavemente hacia el ángulo deseado
        transform.localEulerAngles = Vector3.MoveTowards(currentEuler, targetEuler, rotationSpeed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || players == null || players.Count == 0) return;

        // 🟡 Dibuja los límites del nivel
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(levelBounds.worldBounds.center, levelBounds.worldBounds.size);

        // Calcula bounds de los jugadores (igual que en el código principal)
        Vector3 totalPosition = Vector3.zero;
        Bounds playersBounds = new Bounds(players[0].transform.position, Vector3.zero);

        foreach (GameObject player in players)
        {
            Vector3 pos = ClampToLevel(player.transform.position);
            totalPosition += pos;
            playersBounds.Encapsulate(pos);

            //// 🧭 Dibuja líneas desde cada jugador al centro promedio
            //Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
            //Gizmos.DrawLine(pos, totalPosition / players.Count);
        }

        //// 🔵 Dibuja el bounding box de los jugadores
        //Gizmos.color = Color.cyan;
        //Gizmos.DrawWireCube(playersBounds.center, playersBounds.size);

        // 🔴 Dibuja el centro promedio de los jugadores
        Vector3 averagePosition = totalPosition / players.Count;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(averagePosition, 0.3f);

        //// 🟣 Dibuja la posición objetivo de la cámara (con offset)
        //Vector3 cameraTarget = new Vector3(averagePosition.x, averagePosition.y + yOffset, targetPosition.z);
        //Gizmos.color = Color.magenta;
        //Gizmos.DrawSphere(cameraTarget, 0.3f);
        //Gizmos.DrawLine(averagePosition, cameraTarget);

        //// 🟠 Dibuja la "dispersión" como un vector (extent x + y)
        //Vector3 spreadDir = new Vector3(playersBounds.extents.x, playersBounds.extents.y, 0f);
        //Gizmos.color = new Color(1f, 0.5f, 0f, 0.8f); // Naranja
        //Gizmos.DrawLine(playersBounds.center, playersBounds.center + spreadDir);


        //// 🟪 Área de dispersión máxima permitida (MaxPlayerExtent)
        //float maxPlayerExtent = (levelBounds.halfWidth + levelBounds.halfHeight) * 0.5f;
        //float size = maxPlayerExtent * 2f; // Lo multiplicamos por 2 porque maxExtent es "la mitad"

        //Gizmos.color = new Color(1f, 0f, 1f, 0.25f); // Rosa translúcido
        //Gizmos.DrawWireCube(levelBounds.worldBounds.center, new Vector3(size, size, 0.1f));
    }
}

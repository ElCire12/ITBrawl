using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameInfo : MonoBehaviour
{
    public static GameInfo Instance { get; private set; }

    public List<PlayerInfo> players = new List<PlayerInfo>();

    public List<PlayerInfo> playersPodiumPositions = new List<PlayerInfo>();

    private void Awake()
    {
        // Configurar singleton
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Hay más de un GameInfo en escena. Destruyendo duplicado.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Solo si querés que sobreviva entre escenas
    }
}

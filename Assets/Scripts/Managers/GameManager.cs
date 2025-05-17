using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<PlayerInfo> players = new List<PlayerInfo>();

    public List<PlayerInfo> playersPodiumPositions = new List<PlayerInfo>();

    private void Awake()
    {
        // Configurar singleton
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Hay más de un CharacterSelectManager en escena. Destruyendo duplicado.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Solo si querés que sobreviva entre escenas
    }
}

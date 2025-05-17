using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controlador principal de l'escena de joc: gestiona la instanciació de jugadors,
/// configuració de càmera, barres de vida i la transició cap a l'escena de victòria.
/// </summary>
public class GamePlaySceneManager : MonoBehaviour
{
    public static GamePlaySceneManager Instance { get; private set; }

    [Header("UI")]
    public Image[] healthBars;

    [Header("Gameplay")]
    public List<Transform> spawnPoints;
    public List<GameObject> playersGameplaySceneObjects = new List<GameObject>();
    public int numberOfPlayersDied = 0;

    [Header("Referències externes")]
    public SupCam camera;

    int playerIndex;

    void Awake()
    {
        // Assegura que només hi hagi una instància d'aquest script (patró Singleton)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Actualitza la il·luminació global
        DynamicGI.UpdateEnvironment();

        // Activa tots els dispositius d'entrada (per si algun s'ha desactivat)
        foreach (var device in InputSystem.devices)
        {
            InputSystem.EnableDevice(device);
        }

        int spawnIndex = 0;
        foreach (var player in GameManager.Instance.players)
        {
            // Detecta el tipus de dispositiu del jugador
            string scheme = player.device is Gamepad ? "Gamepad" : "Keyboard";

            // Instància del jugador amb el seu esquema de control
            PlayerInput temp = PlayerInput.Instantiate(
                player.character,
                controlScheme: scheme,
                pairWithDevice: player.device
            );

            // Guarda una referència al component PlayerStateManager
            PlayerStateManager stateManager = temp.GetComponent<PlayerStateManager>();

            // Situa el jugador en el punt d'aparició corresponent
            stateManager.rb.position = spawnPoints[spawnIndex].position;
            stateManager.visuals.eulerAngles = new Vector3(0, spawnPoints[spawnIndex].eulerAngles.y, 0);

            // Assigna la informació del jugador i la seva UI
            stateManager.playerInfo = player;
            stateManager.playerNumberText.text = $"P{playerIndex + 1}";

            // Afegeix el jugador a les llistes de seguiment i control de càmera
            playersGameplaySceneObjects.Add(temp.gameObject);
            camera.players.Add(temp.gameObject);

            // Assigna la barra de vida del jugador
            PlayerLive playerLive = temp.GetComponent<PlayerLive>();
            if (playerLive == null)
                Debug.LogWarning("playerLive és null");

            AssignHealthBar(healthBars[playerIndex], playerLive);

            playerIndex++;
            spawnIndex++;
        }
    }

    bool hasBeenSelectedWinner = false;

    private void Update()
    {
        // Comprova si només queda un jugador viu
        if (numberOfPlayersDied >= GameManager.Instance.players.Count - 1 && !hasBeenSelectedWinner)
        {
            PlayerInfo winner = null;

            foreach (var player in GameManager.Instance.players)
            {
                if (!player.hasDied)
                {
                    winner = player;
                    GameManager.Instance.playersPodiumPositions.Insert(0, winner);
                    hasBeenSelectedWinner = true;
                    break; // Ja s'ha trobat el guanyador
                }
            }

            Debug.LogWarning($"El guanyador és {winner.character.gameObject.name} amb el dispositiu {winner.device.name} i playerIndex {winner.playerIndex}");

            StartCoroutine(WinSceneTransition());
        }
    }

    /// <summary>
    /// Fa una pausa breu en càmera lenta abans de carregar l'escena de victòria.
    /// </summary>
    IEnumerator WinSceneTransition()
    {
        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1;

        Destroy(this.gameObject); // Evita duplicats si es torna a aquesta escena
        SceneManager.LoadScene("WinScene");
    }

    /// <summary>
    /// Associa la barra de vida del jugador amb el seu component de UI.
    /// </summary>
    void AssignHealthBar(Image healthBar, PlayerLive player)
    {
        // Assegura que la barra i el seu contenidor estiguin actius
        healthBar.transform.parent.gameObject.SetActive(true);
        Image healthBarImage = healthBar.GetComponent<Image>();
        if (healthBarImage != null)
        {
            player.healthBar = healthBarImage;
        }
        else
        {
            Debug.LogWarning("El GameObject no té un component Image.");
        }
    }
}
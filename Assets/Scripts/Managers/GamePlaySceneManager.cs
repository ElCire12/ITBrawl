using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controlador principal de l'escena de joc: gestiona la instanciaci� de jugadors,
/// configuraci� de c�mera, barres de vida i la transici� cap a l'escena de vict�ria.
/// </summary>
public class GamePlaySceneManager : MonoBehaviour
{
    public static GamePlaySceneManager Instance { get; private set; }

    [Header("UI")]
    public Image[] healthBars;
    public Image[] profileImages;

    [Header("Gameplay")]
    public List<Transform> spawnPoints;
    public List<GameObject> playersGameplaySceneObjects = new List<GameObject>();
    public int numberOfPlayersDied = 0;

    [Header("Refer�ncies externes")]
    public SupCam2 camera;

    int playerIndex;

    void Awake()
    {
        // Assegura que nom�s hi hagi una inst�ncia d'aquest script (patr� Singleton)
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
        // Actualitza la il�luminaci� global
        DynamicGI.UpdateEnvironment();

        // Activa tots els dispositius d'entrada (per si algun s'ha desactivat)
        foreach (var device in InputSystem.devices)
        {
            InputSystem.EnableDevice(device);
        }

        int spawnIndex = 0;
        foreach (var player in GameInfo.Instance.players)
        {
            // Detecta el tipus de dispositiu del jugador
            string scheme = player.device is Gamepad ? "Gamepad" : "Keyboard";

            // Inst�ncia del jugador amb el seu esquema de control
            PlayerInput temp = PlayerInput.Instantiate(
                player.character,
                controlScheme: scheme,
                pairWithDevice: player.device
            );

            // Guarda una refer�ncia al component PlayerStateManager
            PlayerStateManager stateManager = temp.GetComponent<PlayerStateManager>();

            // Situa el jugador en el punt d'aparici� corresponent
            stateManager.rb.position = spawnPoints[spawnIndex].position;
            stateManager.visuals.eulerAngles = new Vector3(0, spawnPoints[spawnIndex].eulerAngles.y, 0);

            // Assigna la informaci� del jugador i la seva UI
            stateManager.playerInfo = player;
            stateManager.playerNumberText.text = $"P{playerIndex + 1}";

            // Afegeix el jugador a les llistes de seguiment i control de c�mera
            playersGameplaySceneObjects.Add(temp.gameObject);
            camera.players.Add(temp.gameObject);

            // Assigna la barra de vida del jugador
            PlayerLive playerLive = temp.GetComponent<PlayerLive>();
            if (playerLive == null)
                Debug.LogWarning("playerLive �s null");

            AssignHealthBar(healthBars[playerIndex], playerLive, profileImages[playerIndex]);

            playerIndex++;
            spawnIndex++;
        }
    }

    bool hasBeenSelectedWinner = false;

    private void Update()
    {
        // Comprova si nom�s queda un jugador viu
        if (numberOfPlayersDied >= GameInfo.Instance.players.Count - 1 && !hasBeenSelectedWinner)
        {
            PlayerInfo winner = null;

            foreach (var player in GameInfo.Instance.players)
            {
                if (!player.hasDied)
                {
                    winner = player;
                    GameInfo.Instance.playersPodiumPositions.Insert(0, winner);
                    hasBeenSelectedWinner = true;
                    break; // Ja s'ha trobat el guanyador
                }
            }

            Debug.LogWarning($"El guanyador �s {winner.character.gameObject.name} amb el dispositiu {winner.device.name} i playerIndex {winner.playerIndex}");

            StartCoroutine(WinSceneTransition());
        }
    }

    /// <summary>
    /// Fa una pausa breu en c�mera lenta abans de carregar l'escena de vict�ria.
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
    void AssignHealthBar(Image healthBar, PlayerLive player, Image profileImage)
    {
        // Assegura que la barra i el seu contenidor estiguin actius
        healthBar.transform.parent.gameObject.SetActive(true);
        Image healthBarImage = healthBar.GetComponent<Image>();
        if (healthBarImage != null)
        {
            player.healthBar = healthBarImage;
            profileImage.sprite = player.PlayerProfilePicture;
        }
        else
        {
            Debug.LogWarning("El GameObject no t� un component Image.");
        }
    }
}
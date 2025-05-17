using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Controlador principal de la selecció de personatges.
/// Gestiona les fases de: pantalla inicial, selecció de nombre de jugadors,
/// connexió de dispositius i selecció de personatge.
/// </summary>
public class CharacterSelectionManager : MonoBehaviour
{
    public static CharacterSelectionManager Instance { get; private set; }

    public InputDevice selectingDevice;
    PlayerInfo playerSelecting = null;

    [Header("Start UI")]
    public GameObject startUi;

    [Header("Num Players Selection UI")]
    public GameObject numPlayersSelectionUi;

    [Header("Join Devices UI")]
    public GameObject JoinDevicesUI;
    public TextMeshProUGUI devicesJoinedTxt;
    public TextMeshProUGUI playersRemainingTxt;

    [Header("CharacterSelection UI")]
    public GameObject characterSelectionUi;
    public TextMeshProUGUI playerSelectingInfoTxt;
    public Button firstSelectedButton;

    private InputAction joinAction;
    public int maxPlayers = 0;

    public enum selectingStates
    {
        startScreen,
        numPlayersSelecion,
        detectingDevices,
        selectingCharacters
    }

    public selectingStates currrentSelectingState = selectingStates.startScreen;

    private void Awake()
    {
        // Configura singleton
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Hi ha més d'un CharacterSelectManager a l'escena. Destruint duplicat.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Actualitza l'entorn d'il·luminació
        DynamicGI.UpdateEnvironment();
    }

    private void OnEnable()
    {
        // Crea acció per unir-se amb Start (gamepad) o Enter (teclat)
        joinAction = new InputAction(type: InputActionType.Button);
        joinAction.AddBinding("<Gamepad>/start");
        joinAction.AddBinding("<Keyboard>/enter");
        joinAction.Enable();
    }

    void Start()
    {
        // Activa tots els dispositius d'entrada al començar
        foreach (var device in InputSystem.devices)
        {
            InputSystem.EnableDevice(device);
        }
    }

    private void Update()
    {
        // Pantalla inicial, espera que el jugador premi Start/Enter
        if (currrentSelectingState == selectingStates.startScreen)
        {
            if (joinAction.triggered)
            {
                currrentSelectingState = selectingStates.numPlayersSelecion;
                startUi.SetActive(false);
                numPlayersSelectionUi.SetActive(true);
            }
        }

        // La selecció de nombre de jugadors es gestiona amb botons

        // Estat de connexió de dispositius
        if (currrentSelectingState == selectingStates.detectingDevices)
        {
            if (joinAction.triggered)
            {
                var device = joinAction.activeControl?.device;

                // Si el dispositiu no s'ha unit encara i no s'ha arribat al màxim
                if (device != null && GameManager.Instance.players.Count < maxPlayers &&
                    !GameManager.Instance.players.Exists(p => p.device == device))
                {
                    var newPlayer = new PlayerInfo { device = device, playerIndex = GameManager.Instance.players.Count + 1 };
                    GameManager.Instance.players.Add(newPlayer);

                    // Actualitza UI
                    devicesJoinedTxt.text += $"\nPlayer{GameManager.Instance.players.Count} joined with {device.displayName}";
                    playersRemainingTxt.text = $"{maxPlayers - GameManager.Instance.players.Count} restant(s)";

                    // Tots els jugadors s'han unit, comença selecció de personatges
                    if (GameManager.Instance.players.Count >= maxPlayers)
                    {
                        JoinDevicesUI.SetActive(false);
                        characterSelectionUi.SetActive(true);
                        currrentSelectingState = selectingStates.selectingCharacters;
                        StartCoroutine(PlayersSelection());
                    }
                }
            }
        }
    }

    /// <summary>
    /// Coroutine que recorre cada jugador perquè seleccioni un personatge.
    /// </summary>
    IEnumerator PlayersSelection()
    {
        foreach (var player in GameManager.Instance.players)
        {
            playerSelecting = player;

            playerSelectingInfoTxt.text = $"Jugador {playerSelecting.playerIndex} seleccionant personatge...";

            DisableAllDevicesExcept(player.device);
            Debug.Log($"Esperant que el jugador amb {player.device.displayName} seleccioni el seu personatge...");
            yield return StartCoroutine(WaitSelection(player));
        }

        Debug.Log("Tots els jugadors han seleccionat personatge.");
        SceneManager.LoadScene("GameplayScene");
    }

    /// <summary>
    /// Espera que el jugador hagi seleccionat un personatge.
    /// </summary>
    IEnumerator WaitSelection(PlayerInfo player)
    {
        yield return new WaitUntil(() => player.character != null);
    }

    /// <summary>
    /// Desactiva tots els dispositius menys el que toca.
    /// </summary>
    void DisableAllDevicesExcept(InputDevice deviceException)
    {
        foreach (var device in InputSystem.devices)
        {
            if (device == deviceException)
                InputSystem.EnableDevice(device);
            else
                InputSystem.DisableDevice(device);
        }
    }

    /// <summary>
    /// Assigna un personatge al jugador actual.
    /// </summary>
    public void AssignCharacter(GameObject character)
    {
        if (playerSelecting != null)
        {
            playerSelecting.character = character;
            Debug.Log($"{playerSelecting.device.displayName} ha seleccionat: {character.name}");
        }
    }

    /// <summary>
    /// Canvia el nombre de jugadors i comença la detecció de dispositius.
    /// </summary>
    public void changeNumPlayers(int numPlayers)
    {
        maxPlayers = numPlayers;
        numPlayersSelectionUi.SetActive(false);
        JoinDevicesUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
        currrentSelectingState = selectingStates.detectingDevices;
    }
}

/// <summary>
/// Classe que guarda la informació de cada jugador.
/// </summary>
public class PlayerInfo
{
    public InputDevice device;
    public GameObject character = null;
    public int playerIndex;
    public bool hasSelectedCharacter = false;
    public bool hasDied = false;
}

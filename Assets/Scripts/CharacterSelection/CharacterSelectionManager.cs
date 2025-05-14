using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

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
        // Configurar singleton
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Hay más de un CharacterSelectManager en escena. Destruyendo duplicado.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DynamicGI.UpdateEnvironment();
    }

    private void OnEnable()
    {
        joinAction = new InputAction(type: InputActionType.Button);
        joinAction.AddBinding("<Gamepad>/start");
        joinAction.AddBinding("<Keyboard>/enter");
        joinAction.Enable();
    }

    void Start()
    {
        //Enable all devices on start
        foreach (var device in InputSystem.devices)
        {
            InputSystem.EnableDevice(device);
        }
    }

    private void Update()
    {
        if (currrentSelectingState == selectingStates.startScreen) {
            if (joinAction.triggered)
            {
                currrentSelectingState = selectingStates.numPlayersSelecion;
                startUi.SetActive(false);
                numPlayersSelectionUi.SetActive(true);
            }
        }
        
        // selecting state numplayersselection is manged by buttons

        if (currrentSelectingState == selectingStates.detectingDevices)
        {
            if (joinAction.triggered)
            {
                var device = joinAction.activeControl?.device;

                if (device != null && GameManager.Instance.players.Count < maxPlayers && !GameManager.Instance.players.Exists(p => p.device == device))
                {
                    var newPlayer = new PlayerInfo { device = device, playerIndex = GameManager.Instance.players.Count +1};
                    GameManager.Instance.players.Add(newPlayer);

                    //PlayerInput.Instantiate(PlayerInputPrefab);

                    devicesJoinedTxt.text += $"\nPlayer{GameManager.Instance.players.Count} joined with {device.displayName}";
                    playersRemainingTxt.text = $"{maxPlayers - GameManager.Instance.players.Count} remaining";

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

    IEnumerator PlayersSelection()
    {
        foreach (var player in GameManager.Instance.players)
        {
            playerSelecting = player;

            playerSelectingInfoTxt.text = $"Player{playerSelecting.playerIndex} selecting character...";

            DisableAllDevicesExcept(player.device);
            Debug.Log($"Esperando que el jugador con {player.device.displayName} seleccione su personaje...");
            yield return StartCoroutine(WaitSelection(player));
        }

        Debug.Log("Todos los jugadores han seleccionado.");
        SceneManager.LoadScene("GameplayScene");
    }

    IEnumerator WaitSelection(PlayerInfo player)
    {
        yield return new WaitUntil(() => player.character != null);
    }

    void DisableAllDevicesExcept(InputDevice deviceException)
    {
        foreach (var device in InputSystem.devices)
        {
            if (device == deviceException)
            {
                InputSystem.EnableDevice(device);
            }
            else 
            {
                InputSystem.DisableDevice(device);
            }       
        }
    }

    public void AssignCharacter(GameObject character)
    {
        if (playerSelecting != null)
        {
            playerSelecting.character = character;
            Debug.Log($"{playerSelecting.device.displayName} selected: {character.name}");
        }
    }

    public void changeNumPlayers(int numPlayers)
    {
        maxPlayers = numPlayers;
        numPlayersSelectionUi.SetActive(false);
        JoinDevicesUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
        currrentSelectingState = selectingStates.detectingDevices;
    }
}

public class PlayerInfo
{
    public InputDevice device;
    public GameObject character = null;
    public int playerIndex; 
    public bool hasSelectedCharacter = false;
}


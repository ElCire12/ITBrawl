using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;



public class CharacterSelectManager : MonoBehaviour
{
    public static CharacterSelectManager Instance { get; private set; }

    public GameObject player1FirstButton;
    public GameObject player2FirstButton;

    public GameObject PlayerInputPrefab; 

    public InputDevice selectingDevice;

    PlayerInfo playerSelecting = null; 

    private InputAction joinAction;
    public int maxPlayers = 3;

    public enum selectingStates
    {
        detectingDevices,
        selectingCharacters
    }

    public selectingStates currrentSelectingState = selectingStates.detectingDevices; 

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
        foreach (var device in InputSystem.devices)
        {
            InputSystem.EnableDevice(device);
        }
    }

    private void Update()
    {
        if (currrentSelectingState == selectingStates.detectingDevices)
        {
            if (joinAction.triggered)
            {
                var device = joinAction.activeControl?.device;

                if (device != null && GameManager.Instance.players.Count < maxPlayers && !GameManager.Instance.players.Exists(p => p.device == device))
                {
                    var newPlayer = new PlayerInfo { device = device };
                    GameManager.Instance.players.Add(newPlayer);

                    Debug.Log($"Jugador {GameManager.Instance.players.Count} unido con {device.displayName}");

                    PlayerInput.Instantiate(PlayerInputPrefab);
                    
                    if (GameManager.Instance.players.Count >= maxPlayers)
                    {
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
            DisableAllDevicesExcept(player.device);
            Debug.Log($"Esperando que el jugador con {player.device.displayName} seleccione su personaje...");
            yield return StartCoroutine(WaitSelection(player)); // 👈 Espera antes de pasar al siguiente
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
}

public class PlayerInfo
{
    public InputDevice device;
    public GameObject character = null;
    public bool hasSelectedCharacter = false;
}


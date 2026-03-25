using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Video;

public class CharacterSelectionManager : MonoBehaviour
{
    public static CharacterSelectionManager Instance { get; private set; }

    public InputDevice selectingDevice;
    PlayerInfo playerSelecting = null;

    [Header("Start UI")]
    public GameObject startUi;
    public VideoPlayer videoPlayer;
    public RawImage videoImage;

    [Header("Num Players Selection UI")]
    public GameObject numPlayersSelectionUi;
    public Button firstSelectedButtonInNumPlayersSelection;

    [Header("Join Devices UI")]
    public GameObject JoinDevicesUI;
    public TextMeshProUGUI devicesJoinedTxt;
    public TextMeshProUGUI playersRemainingTxt;

    [Header("CharacterSelection UI")]
    public GameObject characterSelectionUi;
    public TextMeshProUGUI playerSelectingInfoTxt;
    public Button firstSelectedButtonInCharacterSelection;

    [Header("Splash Auto Transition")]
    public float splashDuration = 5f;

    [Header("Audio")]
    public AudioSource backgroundMusic;

    private InputAction joinAction;
    private InputAction skipStartAction;
    private InputAction backAction;
    public int maxPlayers = 0;

    private bool splashTimerStarted = false;

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
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Hi ha més d'un CharacterSelectManager a l'escena. Destruint duplicat.");
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

        skipStartAction = new InputAction(type: InputActionType.Button);
        skipStartAction.AddBinding("<Gamepad>/buttonSouth");
        skipStartAction.Enable();

        backAction = new InputAction(type: InputActionType.Button);
        backAction.AddBinding("<Gamepad>/buttonEast");
        backAction.AddBinding("<Keyboard>/escape");
        backAction.Enable();
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
        if (currrentSelectingState == selectingStates.startScreen)
        {
            if (!splashTimerStarted)
            {
                splashTimerStarted = true;
                StartCoroutine(SplashScreenCountdown());
            }

            if (joinAction.triggered || skipStartAction.triggered)
            {
                StopAllCoroutines();
                currrentSelectingState = selectingStates.numPlayersSelecion;
                videoPlayer.enabled = false;
                startUi.SetActive(false);
                numPlayersSelectionUi.SetActive(true);

                if (backgroundMusic != null && !backgroundMusic.isPlaying)
                    backgroundMusic.Play();
            }
        }

        if (currrentSelectingState == selectingStates.numPlayersSelecion)
        {
            if (backAction.triggered)
            {
                currrentSelectingState = selectingStates.startScreen;
                splashTimerStarted = false;
                startUi.SetActive(true);
                videoPlayer.enabled = true;
                numPlayersSelectionUi.SetActive(false);

                // Detener música
                if (backgroundMusic != null && backgroundMusic.isPlaying)
                    backgroundMusic.Stop();

                // Restaurar alpha del video
                if (videoImage != null)
                {
                    var c = videoImage.color;
                    videoImage.color = new Color(c.r, c.g, c.b, 1f);
                }
            }
        }

        if (currrentSelectingState == selectingStates.detectingDevices)
        {
            playersRemainingTxt.text = $"{maxPlayers - GameInfo.Instance.players.Count} restant(s)";
            if (backAction.triggered)
            {
                currrentSelectingState = selectingStates.numPlayersSelecion;
                JoinDevicesUI.SetActive(false);
                numPlayersSelectionUi.SetActive(true);

                EventSystem.current.SetSelectedGameObject(firstSelectedButtonInNumPlayersSelection.gameObject);

                GameInfo.Instance.players.Clear();
                devicesJoinedTxt.text = "";
                playersRemainingTxt.text = $"{maxPlayers} restant(s)";
            }

            if (joinAction.triggered)
            {
                var device = joinAction.activeControl?.device;

                if (device != null && GameInfo.Instance.players.Count < maxPlayers &&
                    !GameInfo.Instance.players.Exists(p => p.device == device))
                {
                    var newPlayer = new PlayerInfo { device = device, playerIndex = GameInfo.Instance.players.Count + 1 };
                    GameInfo.Instance.players.Add(newPlayer);

                    devicesJoinedTxt.text += $"\nEl jugador{GameInfo.Instance.players.Count} s'ha unit amb el dispositiu: {device.displayName}";
                    playersRemainingTxt.text = $"{maxPlayers - GameInfo.Instance.players.Count} restant(s)";

                    if (GameInfo.Instance.players.Count >= maxPlayers)
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

    IEnumerator SplashScreenCountdown()
    {
        yield return new WaitForSeconds(splashDuration);

        yield return StartCoroutine(FadeOutVideo(1f));

        currrentSelectingState = selectingStates.numPlayersSelecion;
        videoPlayer.enabled = false;
        startUi.SetActive(false);
        numPlayersSelectionUi.SetActive(true);

        if (backgroundMusic != null && !backgroundMusic.isPlaying)
            backgroundMusic.Play();
    }

    IEnumerator FadeOutVideo(float duration)
    {
        if (videoImage == null)
            yield break;

        Color startColor = videoImage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            videoImage.color = Color.Lerp(startColor, endColor, elapsed / duration);
            yield return null;
        }

        videoImage.color = endColor;
    }

    IEnumerator PlayersSelection()
    {
        foreach (var player in GameInfo.Instance.players)
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

    IEnumerator WaitSelection(PlayerInfo player)
    {
        yield return new WaitUntil(() => player.character != null);
    }

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

    public void AssignCharacter(GameObject character)
    {
        if (playerSelecting != null)
        {
            playerSelecting.character = character;
            Debug.Log($"{playerSelecting.device.displayName} ha seleccionat: {character.name}");
        }

        EventSystem.current.SetSelectedGameObject(firstSelectedButtonInCharacterSelection.gameObject);
    }

    public void changeNumPlayers(int numPlayers)
    {
        maxPlayers = numPlayers;
        numPlayersSelectionUi.SetActive(false);
        JoinDevicesUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedButtonInCharacterSelection.gameObject);
        currrentSelectingState = selectingStates.detectingDevices;
    }
}

public class PlayerInfo
{
    public InputDevice device;
    public GameObject character = null;
    public int playerIndex;
    public bool hasSelectedCharacter = false;
    public bool hasDied = false;
}

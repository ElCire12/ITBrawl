using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class WinSceneManager : MonoBehaviour
{
    [Header("Posicions de spawn per al podi")]
    public Transform[] spawnPositions;

    InputAction skipAction;
    private float tiempoDesdeInicio;
    public float tiempoMinimoAntesDeSaltar = 5f;

    private void Start()
    {
        skipAction = new InputAction(type: InputActionType.Button);
        skipAction.AddBinding("<Gamepad>/start");
        skipAction.AddBinding("<Gamepad>/buttonSouth");
        skipAction.AddBinding("<Keyboard>/enter");
        skipAction.Enable();

        tiempoDesdeInicio = 0f;

        DynamicGI.UpdateEnvironment();

        // Instancia cada jugador en la seva posició corresponent al podi
        for (int i = 0; i < GameInfo.Instance.playersPodiumPositions.Count; i++)
        {
            GameObject playerPrefab = GameInfo.Instance.playersPodiumPositions[i].character.GetComponent<PlayerStateManager>().playerWinPrefab;
            Transform spawnPosition = spawnPositions[i].transform;

            GameObject temp = Instantiate(playerPrefab, spawnPosition.position, playerPrefab.transform.rotation);
            temp.transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void Update()
    {
        tiempoDesdeInicio += Time.deltaTime;

        if (tiempoDesdeInicio >= tiempoMinimoAntesDeSaltar && skipAction.triggered)
        {
            Destroy(GameInfo.Instance.gameObject);
            SceneManager.LoadScene("StartScene");
        }
    }
}

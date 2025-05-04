using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePlaySceneManager : MonoBehaviour
{
    void Start()
    {

        foreach (var device in InputSystem.devices)
        {
            InputSystem.EnableDevice(device);
        }

        foreach (var player in GameManager.Instance.players)
        {
            string scheme = player.device is Gamepad ? "Gamepad" : "Keyboard";

            PlayerInput.Instantiate(
            player.character,
            controlScheme: scheme,
            pairWithDevice: player.device
            );

            Debug.Log($"Instanciated {player.character.name} pared to device {player.device.name}");
        }       
    }
}

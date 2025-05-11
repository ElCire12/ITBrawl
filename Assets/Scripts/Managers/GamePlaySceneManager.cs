using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GamePlaySceneManager : MonoBehaviour
{
    public Image[] healthBars; 

    public List<GameObject> playersObjects = new List<GameObject>();

    public SupCam camera;

    int playerIndex;

    void Start()
    {
        if (healthBars == null) Debug.Log("healthBars es null");
        DynamicGI.UpdateEnvironment();
        //foreach (var healthbar in healthBars)
        //{
        //    healthbar.gameObject.transform.parent.gameObject.SetActive(false);
        //}

        foreach (var device in InputSystem.devices)
        {
            InputSystem.EnableDevice(device);
        }

        foreach (var player in GameManager.Instance.players)
        {
            string scheme = player.device is Gamepad ? "Gamepad" : "Keyboard";

            PlayerInput temp = PlayerInput.Instantiate(
                player.character,
                controlScheme: scheme,
                pairWithDevice: player.device
            );

            playersObjects.Add(temp.gameObject);
            camera.players.Add(temp.gameObject);

            Debug.Log($"Añadiendo healthbar al jugador {playersObjects[playerIndex]}");

            PlayerLive playerLive = playersObjects[playerIndex].GetComponent<PlayerLive>();
            if (playerLive == null) Debug.Log("playerLive es null"); 
            AssignHealthBar(healthBars[playerIndex],playerLive);

            playerIndex++;
        }
    }

    void AssignHealthBar(Image healthBar, PlayerLive player)
    {
        // Asegúrate de que el GameObject tiene un componente Image
        healthBar.gameObject.transform.parent.gameObject.SetActive(true);
        Image healthBarImage = healthBar.GetComponent<Image>();
        if (healthBarImage != null)
        {
            player.healthBar = healthBarImage;
        }
        else
        {
            Debug.LogWarning("El GameObject no tiene un componente Image.");
        }
    }
}

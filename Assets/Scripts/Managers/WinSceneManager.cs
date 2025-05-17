using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestiona la instanciació dels jugadors a la escena de victòria.
/// Situa els jugadors en el podi i activa una animació d'atac.
/// </summary>
public class WinSceneManager : MonoBehaviour
{
    [Header("Posicions de spawn per al podi")]
    public Transform[] spawnPositions;

    private void Start()
    {
        // Instancia cada jugador en la seva posició corresponent al podi
        for (int i = 0; i < GameManager.Instance.playersPodiumPositions.Count; i++)
        {
            GameObject playerPrefab = GameManager.Instance.playersPodiumPositions[i].character;
            Transform spawnPosition = spawnPositions[i].transform;

            // Instancia el personatge guanyador en la posició assignada
            GameObject temp = Instantiate(playerPrefab, spawnPosition.position, spawnPosition.rotation);

            // Activa una animació o estat d'"atac" com a celebració
            temp.GetComponent<PlayerStateManager>().isAttacking = true;

            // Reorienta el jugador perquè miri cap endavant del podi
            temp.transform.eulerAngles = new Vector3(0f, -270f, 0f);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLive : MonoBehaviour
{
    public int maxLive;
    public int currentLive;
    public PlayerStateManager playerStateManager;
    public Image healthBar; 

    private void Awake()
    {
        currentLive = maxLive;
    }

    public void Heal(int liveAmount)
    {
        if ((currentLive + liveAmount) > maxLive) {
            currentLive = maxLive;
        }
        else { currentLive += liveAmount; }

        UpdateHealthBar();
    }

    public void TakeDamage(int liveAmount, float stunTime = 0, Transform attackerPosition = null)
    {
        playerStateManager.ApplyStun(stunTime);

        if (attackerPosition != null) {
            
            float direction;
            if (attackerPosition.position.x < transform.position.x) direction = 1; else direction = -1;

            playerStateManager.visuals.eulerAngles = new Vector3(0f, 90f * -direction, 0f);

            playerStateManager.rb.AddForce(new Vector2(direction * 300, 1 * 50), ForceMode.Impulse);
        }

        if ((currentLive - liveAmount) <= 0f) //Comprobar si ha deixat sense vida al jugador
        {
            currentLive = 0; 
            Die();
        }
        else
        {
            currentLive -= liveAmount;
        }

        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        float health_percentatge = currentLive / (float)maxLive;

        healthBar.fillAmount = health_percentatge;

        Debug.Log($"Health bar percentatge {health_percentatge}");
    }

    void Die()
    {
        Debug.Log("Player Died"); 
    }
}

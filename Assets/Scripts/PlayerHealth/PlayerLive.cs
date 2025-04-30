using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLive : MonoBehaviour
{
    public int maxLive;
    public int currentLive;

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
    }

    public void TakeDamage(int liveAmount)
    {
        if ((currentLive - liveAmount) <= 0f)
        {
            currentLive = 0; 
            Die();
        }
        else
        {
            currentLive -= liveAmount;
        }
    }

    void Die()
    {
        Debug.Log("Player Died"); 
    }
}

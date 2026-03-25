using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLive : MonoBehaviour
{
    public ParticleSystem diePartycles;
    public int maxLive;
    public int currentLive;
    public PlayerStateManager context;
    public Image healthBar;
    public Sprite PlayerProfilePicture;

    private void Awake()
    {
        currentLive = maxLive;
    }

    private void OnEnable()
    {
        
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
        SoundManager.Instance.PlayRandomSound(context.hittedSounds);
        context.ApplyStun(stunTime);

        if (attackerPosition != null) {
            
            float direction;
            if (attackerPosition.position.x < transform.position.x) direction = 1; else direction = -1;

            context.visuals.eulerAngles = new Vector3(0f, 90f * -direction, 0f);

            context.rb.velocity = Vector3.zero;
            context.rb.AddForce(new Vector2(direction * 300, 1 * 50), ForceMode.Impulse);
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
    }

    void Die()
    {
        SupCam2.Instance.players.Remove(this.gameObject);

        if (GamePlaySceneManager.Instance.numberOfPlayersDied >= GameInfo.Instance.players.Count - 1 == false)
        {
            GameInfo.Instance.playersPodiumPositions.Insert(0, context.playerInfo);
        }

        GameObject temp = Instantiate(diePartycles.gameObject, transform.position, Quaternion.identity);
        temp.GetComponent<ParticleSystem>().Play();

        this.gameObject.SetActive(false);
        context.playerInfo.hasDied = true;

        GamePlaySceneManager.Instance.numberOfPlayersDied++;
    }
}

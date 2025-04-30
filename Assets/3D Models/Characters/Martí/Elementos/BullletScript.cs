using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullletScript : MonoBehaviour
{
    public float liveTime;
    public int damage;

    public LayerMask afectMask;

    void OnEnable()
    {
        IEnumerator Coroutine()
        {
            yield return new WaitForSeconds(liveTime);
            Destroy(this.gameObject); 
        }
        StartCoroutine(Coroutine());
    }

    private void OnCollisionEnter(Collision other)
    {
        PlayerLive playerLiveScript = other.gameObject.GetComponent<PlayerLive>();

        if (playerLiveScript != null)
        {
            playerLiveScript.TakeDamage(damage);
        }
        Destroy(this.gameObject);
    }
}

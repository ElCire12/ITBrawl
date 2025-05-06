using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullletScript : MonoBehaviour
{
    public GameObject parent;
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
        if (other.gameObject != parent)
        {
            PlayerLive playerLiveScript = other.gameObject.GetComponent<PlayerLive>();

            if (playerLiveScript != null)
            {
                playerLiveScript.TakeDamage(damage, 0.3f, parent.transform);
            }
            Destroy(this.gameObject);
        }
    }
}

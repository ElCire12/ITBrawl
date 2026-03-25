using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public GameObject parent;
    public float liveTime;
    public int damage;
    public bool destroyOnTouchSomething;

    public bool limitVelocity = false;
    public float maxVelocity;

    private Rigidbody rb;

    public LayerMask afectMask;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        IEnumerator Coroutine()
        {
            yield return new WaitForSeconds(liveTime);
            Destroy(this.gameObject); 
        }
        StartCoroutine(Coroutine());
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && other.gameObject != parent.gameObject) {
            PlayerLive playerLiveScript = other.gameObject.GetComponent<PlayerLive>();

            if (playerLiveScript != null)
            {
                playerLiveScript.TakeDamage(damage, 0.3f, parent.transform);
            }

            Destroy(this.gameObject);
        }
        else if (destroyOnTouchSomething)
        {
            Destroy(this.gameObject);
        }

        if (limitVelocity)
        {
            if (rb.velocity.magnitude > maxVelocity)
            {
                rb.velocity = rb.velocity.normalized * maxVelocity;
            }
        }
    }
}

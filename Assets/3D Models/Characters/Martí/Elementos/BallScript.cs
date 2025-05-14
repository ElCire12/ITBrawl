using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public enum BallStates
    {
        launched,
        hitted
    }

    public GameObject thrower;
    public float liveTime;
    public int damage;
    public bool destroyOnTouchSomething;

    public bool limitVelocity = false;
    public float maxVelocity;

    private Rigidbody rb;
    private Collider col;
    private BallStates currentState = BallStates.launched;

    public LayerMask afectMask;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        IEnumerator Coroutine()
        {
            yield return new WaitForSeconds(liveTime);
            Destroy(this.gameObject);
        }
        StartCoroutine(Coroutine());
    }

    private void OnCollisionStay(Collision other)
    {
        if (currentState == BallStates.hitted)
        {   
            if (other.gameObject.CompareTag("ground"))
            {
                rb.velocity = Vector3.zero;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // 🔁 Si ya está hitted, solo reaccionamos a suelo o lanzador
        if (currentState == BallStates.hitted)
        {
            if (other.gameObject == thrower)
            {
                PlayerLive playerLive = thrower.GetComponent<PlayerLive>();
                playerLive.Heal(15);
                Destroy(this.gameObject);
            }
            else if (other.gameObject.CompareTag("ground"))
            {
                rb.velocity = Vector3.zero;
            }

            return; // Salimos del método aquí
        }

        // 🧍 Dañar a jugadores que no son el lanzador
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && other.gameObject != thrower.gameObject)
        {
            PlayerLive playerLiveScript = other.gameObject.GetComponent<PlayerLive>();

            if (playerLiveScript != null)
            {
                playerLiveScript.TakeDamage(damage, 0.3f, thrower.transform);
                EnterHittedState();
            }
        }
        else if (destroyOnTouchSomething)
        {
            Destroy(this.gameObject);
        }

        // ⚡ Limitar velocidad
        if (limitVelocity && rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }
    }

    private void EnterHittedState()
    {
        currentState = BallStates.hitted;
        rb.velocity = Vector3.zero;
        rb.useGravity = true;

        col.material = null;
        col.isTrigger = false;

        // Ignorar colisiones con todo menos suelo y jugador lanzador
        Collider[] allColliders = FindObjectsOfType<Collider>();
        foreach (var otherCol in allColliders)
        {
            GameObject go = otherCol.gameObject;
            bool isSelf = go == this.gameObject;
            bool isParent = go == thrower;
            bool isGround = go.CompareTag("ground");

            if (!isSelf && !isParent && !isGround)
            {
                Physics.IgnoreCollision(col, otherCol, true);
            }
        }
    }
}

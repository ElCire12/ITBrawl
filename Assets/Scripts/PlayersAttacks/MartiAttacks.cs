using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MartiAttacks : CharacterAttack
{
    [Header("Special Up / Gorra")]
    public float gorraJumpForce; 

    [Header("Special Frontal / Bocata")]
    public float bocataForce; 
    public GameObject bocataPrefab;

    [Header("Special Down / Ataque Lego")]
    public Transform bocataSpawnPoint; 
    public ParticleSystem legoParticles;

    public float specialDownaAffectArea = 3f;
    public int specialDownDamage = 50;
    public LayerMask enemyLayer;


    bool alredyJumped = false; 
    public override void FrontalAttack()
    {
        IEnumerator Coroutine()
        {
            context.isAttacking = true;
            context.rb.velocity = Vector2.zero;
            context.animator.SetTrigger("special_front");

            yield return new WaitForSeconds(0.18f + 0.25f);
            GameObject bocata = Instantiate(bocataPrefab, bocataSpawnPoint.position, Quaternion.identity);
            Rigidbody rbBocata = bocata.GetComponent<Rigidbody>();
            rbBocata.AddForce(new Vector3(context.GetActualPlayerDirection(), 0, 0) * bocataForce, ForceMode.Impulse);

            yield return new WaitForSeconds(1f);
            context.isAttacking = false;
        }
        StartCoroutine(Coroutine());
    }

    public override void UpAttack()
    {
        IEnumerator Coroutine()
        {
            context.isAttacking = true;
            context.rb.velocity = Vector2.zero;

            context.rb.AddForce(new Vector2(0, 1 * gorraJumpForce), ForceMode.Impulse);

            yield return new WaitForSeconds(1f);
            context.isAttacking = false;
        }
        StartCoroutine(Coroutine());
    }

    public override void DownAttack()
    {
        IEnumerator Coroutine()
        {
            context.isAttacking = true;
            context.rb.velocity = Vector2.zero;
            context.animator.SetTrigger("special_down");

            yield return new WaitForSeconds(0.5f + 0.25f);

            legoParticles.transform.position = transform.position;
            legoParticles.Play();

            //Activar hitted area
            Collider[] hits = Physics.OverlapSphere(transform.position, specialDownaAffectArea, enemyLayer);
            
            foreach (Collider hit in hits)
            {
                if (hit.gameObject != this.gameObject)
                {
                    hit.GetComponent<PlayerLive>()?.TakeDamage(specialDownDamage);
                }
            }

            //Dejar de estar en estado atacando 
            yield return new WaitForSeconds(1f);
            context.isAttacking = false;
        }

        StartCoroutine(Coroutine()); 
    }
    public override void BaseFrontalAttack()
    {
        
    }

    public override void BaseUpAttack()
    {

    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, specialDownaAffectArea);
    //}
}

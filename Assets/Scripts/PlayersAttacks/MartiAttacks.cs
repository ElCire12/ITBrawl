using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MartiAttacks : CharacterAttack
{
    [Header("Special Up / Gorra")]
    public float gorraJumpForce;
    [SerializeField] private int specialUpDamage;
    public AudioClip[] specialUpSounds;

    public override int SpecialUpDamage => specialUpDamage;
    public Collider specialUpHurtBox;

    [Header("Special Frontal / Bocata")]
    public float bocataForce; 
    public GameObject bocataPrefab;

    [Header("Special Down / Ataque Lego")]
    public Transform bocataSpawnPoint; 
    public ParticleSystem legoParticles;

    public float specialDownaAffectArea = 3f;
    public float stunTime = 1f;
    public int specialDownDamage = 50;
    public LayerMask enemyLayer;
    

    public override void FrontalAttack()
    {
        IEnumerator Coroutine()
        {
            context.isAttacking = true;
            context.rb.velocity = Vector2.zero;
            context.animator.SetTrigger("special_front");

            yield return new WaitForSeconds(0.18f + 0.25f);
            GameObject bocata = Instantiate(bocataPrefab, bocataSpawnPoint.position, Quaternion.identity);
            bocata.GetComponent<BullletScript>().parent = this.gameObject;
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
            //Start animation
            context.isAttacking = true;
            context.rb.velocity = Vector2.zero;
            context.animator.SetTrigger("special_up");
            context.rb.constraints |= RigidbodyConstraints.FreezePositionY;

            yield return new WaitForSeconds(0.58f + 0.25f);
            context.rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
            //Add jump and enable hurtbox
            specialUpHurtBox.enabled = true;
            context.rb.AddForce(new Vector2(0, 1 * gorraJumpForce), ForceMode.Impulse);
            SoundManager.Instance.PlayRandomSound(specialUpSounds); 

            yield return new WaitForSeconds(1f);
            specialUpHurtBox.enabled = false;
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
                    hit.GetComponent<PlayerLive>()?.TakeDamage(specialDownDamage, stunTime, transform);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MartiAttacks : CharacterAttack
{
    [Header("Special Up / Gorra")]
    public float gorraJumpForce;
    public GameObject gorraPrefab;
    public GameObject gorraFake;
    [SerializeField] private int specialUpDamage;
    public AudioClip[] specialUpSounds;

    [Header("Special Up AirControl")]
    public float upAttackAirAcceleration = 5f;
    public float upAttackAirMaxSpeed = 3f;
    public float upAttackMoveDuration = 1f;

    public override int SpecialUpDamage => specialUpDamage;
    public Collider specialUpHurtBox;

    [Header("Special Frontal / Bocata")]
    public float bocataForce; 
    public GameObject bocataPrefab;
    public float specialFrontRecoveryTime;
    public Transform bocataSpawnPoint;

    [Header("Special Down / Ataque Lego")]
    public ParticleSystem legoParticles;
    public float specialDownRecoveryTime;

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
            //context.animator.SetTrigger("special_front");
            context.animator.CrossFadeInFixedTime("FrontSpecialAttack", 0f);


            yield return new WaitForSeconds(0.18f + 0.25f);
            GameObject bocata = Instantiate(bocataPrefab, bocataSpawnPoint.position, Quaternion.identity);
            bocata.GetComponent<BulletScript>().parent = this.gameObject;
            Rigidbody rbBocata = bocata.GetComponent<Rigidbody>();
            rbBocata.AddForce(new Vector3(context.GetActualPlayerDirection(), 0, 0) * bocataForce, ForceMode.Impulse);

            context.animator.CrossFadeInFixedTime("Idle", specialFrontRecoveryTime);
            yield return new WaitForSeconds(specialFrontRecoveryTime);
            context.isAttacking = false;
        }
        StartAttackCoroutine(Coroutine());
    }

    public override void UpAttack()
    {
        IEnumerator Coroutine()
        {
            context.isAttacking = true;
            context.rb.velocity = Vector2.zero;
            context.animator.CrossFadeInFixedTime("SpecialUp", 0f);
            context.rb.constraints |= RigidbodyConstraints.FreezePositionY;
            gorraFake.SetActive(true);

            yield return new WaitForSeconds(0.58f + 0.25f);
            gorraFake.SetActive(false);
            Instantiate(gorraPrefab, bocataSpawnPoint.position, gorraPrefab.transform.rotation);

            context.rb.constraints &= ~RigidbodyConstraints.FreezePositionY;

            // Añadir salto y activar hurtbox
            specialUpHurtBox.enabled = true;
            context.rb.AddForce(new Vector2(0, gorraJumpForce), ForceMode.Impulse);
            SoundManager.Instance.PlayRandomSound(specialUpSounds);

            float moveDuration = 1f;
            float timer = 0f;

            while (timer < moveDuration)
            {
                timer += Time.deltaTime;

                Debug.Log($"RB XVelocity {context.rb.velocity.x}");

                // === Movimiento aéreo ===
                if (context.movementInput.x != 0)
                {
                    Vector3 force = new Vector3(context.movementInput.x * context.airAcceleration, 0f, 0f);
                    context.rb.AddForce(force, ForceMode.Force);
                }

                // === Límite de velocidad en el aire ===
                if (Mathf.Abs(context.rb.velocity.x) > upAttackAirMaxSpeed)
                {
                    context.rb.velocity = new Vector2(
                        upAttackAirMaxSpeed * Mathf.Sign(context.rb.velocity.x),
                        context.rb.velocity.y
                    );
                }

                yield return null; // Esperar al siguiente frame
            }

            // Finalizar ataque
            specialUpHurtBox.enabled = false;
            context.isAttacking = false;
        }
        StartAttackCoroutine(Coroutine());
    }

    public override void DownAttack()
    {
        IEnumerator Coroutine()
        {
            context.isAttacking = true;
            context.rb.velocity = Vector2.zero;
            //context.animator.SetTrigger("special_down");
            context.animator.CrossFadeInFixedTime("SpecialDown", 0f);

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

            context.animator.CrossFadeInFixedTime("Idle", specialDownRecoveryTime);
            //Dejar de estar en estado atacando 
            yield return new WaitForSeconds(specialDownRecoveryTime);
            context.isAttacking = false;
        }

        StartAttackCoroutine(Coroutine());
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

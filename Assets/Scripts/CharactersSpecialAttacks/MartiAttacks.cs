using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MartiAttacks : CharacterAttack
{
    [Header("Special Up / Gorra")]
    [SerializeField] private int specialUpDamage;
    public float specialUpStunTime = 1f;
    public float gorraJumpForce;
    public GameObject gorraPrefab;
    public GameObject gorraFake;
    public Collider specialUpHurtBox;

    [Header("Special Up AirControl")]
    public float upAttackAirAcceleration = 5f;
    public float upAttackAirMaxSpeed = 3f;
    public float upAttackMoveDuration = 1f;

    public override int SpecialUpDamage => specialUpDamage;

    [Header("Special Frontal / Bocata")]
    public float bocataForce;
    public float specialFrontStunTime = 1f;
    public float specialFrontRecoveryTime;
    public GameObject bocataPrefab;
    public Transform bocataSpawnPoint;

    [Header("Special Down / Ataque Lego")]
    public int specialDownDamage = 50;
    public float specialDownStunTime = 1f;
    public float specialDownaAffectArea = 3f;
    public float specialDownRecoveryTime;
    public ParticleSystem legoParticles;

    [Header("Basic Frontal")]
    public int basicFrontAttackDamage;
    public float basicFrontStunTime = 1f;
    public float basicFrontAttackArea;
    public float basicFrontRecoveryTime;
    public Transform basicFrontAttackPosition;

    [Header("Basic Up")]
    public int basicUpAttackDamage;
    public float basicUpStunTime;
    public float basicUpAttackArea;
    public float basicUpRecoveryTime;
    public Transform basicUpAttackPosition;

    [Header("Sounds")]
    public AudioClip[] specialUpSounds;
    public AudioClip[] specialDownSounds;
    public AudioClip[] specialFrontSounds;

    [Header("General")]
    public LayerMask enemyLayer;

    public override void SpecialFrontalAttack()
    {
        IEnumerator Coroutine()
        {
            context.isAttacking = true;
            context.rb.velocity = Vector2.zero;
            context.animator.CrossFadeInFixedTime("FrontSpecialAttack", 0f);
            SoundManager.Instance.PlayRandomSound(specialFrontSounds);

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

    public override void SpecialUpAttack()
    {
        IEnumerator Coroutine()
        {
            context.isAttacking = true;
            context.rb.velocity = Vector2.zero;
            context.animator.CrossFadeInFixedTime("SpecialUp", 0f);
            context.rb.constraints |= RigidbodyConstraints.FreezePositionY;
            gorraFake.SetActive(true);

            yield return new WaitForSeconds(0.58f + 0.25f);
            SoundManager.Instance.PlayRandomSound(specialUpSounds);

            gorraFake.SetActive(false);
            Instantiate(gorraPrefab, bocataSpawnPoint.position, gorraPrefab.transform.rotation);
            context.rb.constraints &= ~RigidbodyConstraints.FreezePositionY;

            specialUpHurtBox.enabled = true;
            context.rb.AddForce(new Vector2(0, gorraJumpForce), ForceMode.Impulse);

            float timer = 0f;
            while (timer < upAttackMoveDuration)
            {
                timer += Time.deltaTime;

                if (context.movementInput.x != 0)
                {
                    Vector3 force = new Vector3(context.movementInput.x * context.airAcceleration, 0f, 0f);
                    context.rb.AddForce(force, ForceMode.Force);
                }

                if (Mathf.Abs(context.rb.velocity.x) > upAttackAirMaxSpeed)
                {
                    context.rb.velocity = new Vector2(
                        upAttackAirMaxSpeed * Mathf.Sign(context.rb.velocity.x),
                        context.rb.velocity.y
                    );
                }

                yield return null;
            }

            specialUpHurtBox.enabled = false;
            context.isAttacking = false;
        }
        StartAttackCoroutine(Coroutine());
    }

    public override void SpecialDownAttack()
    {
        IEnumerator Coroutine()
        {
            context.isAttacking = true;
            context.rb.velocity = Vector2.zero;
            context.animator.CrossFadeInFixedTime("SpecialDown", 0f);

            yield return new WaitForSeconds(0.5f + 0.25f);

            SoundManager.Instance.PlayRandomSound(specialDownSounds);

            legoParticles.transform.position = transform.position;
            legoParticles.Play();

            Collider[] hits = Physics.OverlapSphere(transform.position, specialDownaAffectArea, enemyLayer);
            foreach (Collider hit in hits)
            {
                if (hit.gameObject != this.gameObject)
                {
                    hit.GetComponent<PlayerLive>()?.TakeDamage(specialDownDamage, specialDownStunTime, transform);
                }
            }

            context.animator.CrossFadeInFixedTime("Idle", specialDownRecoveryTime);
            yield return new WaitForSeconds(specialDownRecoveryTime);
            context.isAttacking = false;
        }
        StartAttackCoroutine(Coroutine());
    }

    public override void BasicFrontalAttack()
    {
        IEnumerator Coroutine()
        {
            context.isAttacking = true;
            context.rb.velocity = Vector2.zero;
            context.animator.CrossFadeInFixedTime("BasicFront", 0.2f);

            yield return new WaitForSeconds(0.13f + 0.2f);

            Collider[] hits = Physics.OverlapSphere(basicFrontAttackPosition.position, basicFrontAttackArea, enemyLayer);
            foreach (Collider hit in hits)
            {
                if (hit.gameObject != this.gameObject)
                {
                    hit.GetComponent<PlayerLive>()?.TakeDamage(basicFrontAttackDamage, basicFrontStunTime, transform);
                }
            }

            context.animator.CrossFadeInFixedTime("Idle", basicFrontRecoveryTime);
            yield return new WaitForSeconds(basicFrontRecoveryTime);
            context.isAttacking = false;
        }
        StartAttackCoroutine(Coroutine());
    }

    public override void BasicUpAttack()
    {
        IEnumerator Coroutine()
        {
            context.isAttacking = true;
            context.rb.velocity = Vector2.zero;
            context.animator.CrossFadeInFixedTime("BasicUp", 0.2f);

            yield return new WaitForSeconds(0.13f + 0.2f);

            Collider[] hits = Physics.OverlapSphere(basicUpAttackPosition.position, basicUpAttackArea, enemyLayer);
            foreach (Collider hit in hits)
            {
                if (hit.gameObject != this.gameObject)
                {
                    hit.GetComponent<PlayerLive>()?.TakeDamage(basicUpAttackDamage, basicUpStunTime, transform);
                }
            }

            context.animator.CrossFadeInFixedTime("Idle", basicUpRecoveryTime);
            yield return new WaitForSeconds(basicUpRecoveryTime);
            context.isAttacking = false;
        }
        StartAttackCoroutine(Coroutine());
    }

    public override void ResetAttackState()
    {
        specialUpHurtBox.enabled = false;
        context.rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(basicFrontAttackPosition.position, basicFrontAttackArea);

        Gizmos.color = Color.blue;
        if (basicUpAttackPosition != null)
            Gizmos.DrawWireSphere(basicUpAttackPosition.position, basicUpAttackArea);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, specialDownaAffectArea);

    }
}

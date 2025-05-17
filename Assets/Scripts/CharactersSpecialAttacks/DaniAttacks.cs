using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class DaniAttacks : CharacterAttack
{
    [Header("Special Up / Escridassada")]
    public float gorraJumpForce;
    [SerializeField] private int specialUpDamage;
    public override int SpecialUpDamage => specialUpDamage;
    public Collider specialUpHurtBox;
    public AudioClip[] specialUpSounds;
    public float specialUpStunTime = 1f;
    public float specialUpDuration = 1f;

    [Header("Special Up AirControl")]
    public float upAttackAirAcceleration = 5f;
    public float upAttackAirMaxSpeed = 3f;
    public float upAttackMoveDuration = 1f;

    [Header("Special Frontal / Pilota")]
    public float ballForce;
    public GameObject ballPrefab;
    public float specialFrontRecoveryTime;
    public Transform ballSpawnPoint;
    public int specialFrontDamage = 20;
    public float specialFrontStunTime = 0.5f;

    [Header("Special Down / Agarre")]
    public float grabDuration = 2f;
    public float grabRadius = 1.5f;
    public Transform grabFollowPoint;
    public int grabDamage = 30;
    public float grabStunTime = 1.5f;
    public Transform grabPosition;

    [Header("Basic Frontal")]
    public int basicFrontAttackDamage;
    public float basicFrontStunTime;
    public float basicFrontAttackArea;
    public float basicFrontRecoveryTime;
    public Transform basicFrontAttackPosition;

    [Header("Basic Up")]
    public int basicUpAttackDamage;
    public float basicUpStunTime;
    public float basicUpAttackArea;
    public float basicUpRecoveryTime;
    public Transform basicUpAttackPosition;

    public float stunTime = 1f;
    public LayerMask enemyLayer;

    public override void SpecialFrontalAttack()
    {
        IEnumerator Coroutine()
        {
            context.isAttacking = true;
            context.rb.velocity = Vector2.zero;
            context.animator.CrossFadeInFixedTime("FrontSpecialAttack", 0f);
            context.animator.speed = 0.4f;

            yield return new WaitForSeconds(0.18f + 0.25f);
            GameObject bocata = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
            bocata.GetComponent<BallScript>().thrower = this.gameObject;
            Rigidbody rbBocata = bocata.GetComponent<Rigidbody>();
            rbBocata.AddForce(new Vector3(context.GetActualPlayerDirection(), 1, 0) * ballForce, ForceMode.Force);

            context.animator.CrossFadeInFixedTime("Idle", specialFrontRecoveryTime);
            context.animator.speed = 1f;
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
            context.animator.speed = 0.5f;

            specialUpHurtBox.enabled = true;
            context.rb.AddForce(new Vector2(0, gorraJumpForce), ForceMode.Impulse);
            SoundManager.Instance.PlayRandomSound(specialUpSounds);

            yield return new WaitForSeconds(specialUpDuration);
            
            context.animator.speed = 1f;
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
            context.canAttack = false;
            context.canJump = false;

            context.rb.velocity = Vector2.zero;
            context.animator.CrossFadeInFixedTime("SpecialDown", 0f);
            context.animator.speed = 0.5f;

            yield return new WaitForSeconds(0.2f);

            Collider[] hits = Physics.OverlapSphere(grabPosition.position, grabRadius);

            foreach (Collider hit in hits)
            {
                if (hit.gameObject == this.gameObject) continue;

                PlayerLive enemy = hit.GetComponent<PlayerLive>();
                if (enemy != null)
                {
                    Debug.Log($"Hitted {enemy.name}");
                    enemy.TakeDamage(grabDamage, grabStunTime);
                    StartCoroutine(GrabFollowCoroutine(enemy));
                    break;
                }
                else
                {
                    context.canAttack = true;
                    context.canJump = true;
                }
            }

            context.animator.speed = 1f;

            // Visualmente pasar a Idle, pero internamente seguir en "attack mode"
            context.animator.CrossFadeInFixedTime("Idle", grabDuration);

            context.isAttacking = false;

            yield return new WaitForSeconds(grabDuration);

            // Ahora sí, liberar completamente
            
        }

        StartAttackCoroutine(Coroutine());
    }

    private IEnumerator GrabFollowCoroutine(PlayerLive enemy)
    {
        Transform enemyTransform = enemy.transform;
        Transform followTarget = grabFollowPoint;

        float elapsed = 0f;

        while (elapsed < grabDuration)
        {
            if (enemyTransform != null && followTarget != null)
            {
                enemyTransform.position = followTarget.position;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        context.canAttack = true;
        context.canJump = true;
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

    public override void BasicUpAttack() { 
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

    }

    private void OnDrawGizmos()
    {
        // Special Down 
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(grabPosition.position, grabRadius);

        // Basic Frontal Attack
        if (basicFrontAttackPosition != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(basicFrontAttackPosition.position, basicFrontAttackArea);
        }

        // Basic Up Attack
        if (basicUpAttackPosition != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(basicUpAttackPosition.position, basicUpAttackArea);
        }
    }

}

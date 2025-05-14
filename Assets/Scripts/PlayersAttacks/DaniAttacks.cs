using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaniAttacks : CharacterAttack
{
    [Header("Special Up / Escridassada")]
    public float gorraJumpForce;
    [SerializeField] private int specialUpDamage;
    public Collider specialUpHurtBox;
    public AudioClip[] specialUpSounds;

    [Header("Special Up AirControl")]
    public float upAttackAirAcceleration = 5f;
    public float upAttackAirMaxSpeed = 3f;
    public float upAttackMoveDuration = 1f;

    public override int SpecialUpDamage => specialUpDamage;

    [Header("Special Frontal / Pilota")]
    public float ballForce;
    public GameObject ballPrefab;
    public float specialFrontRecoveryTime;
    public Transform ballSpawnPoint;

    [Header("Special Down / Agarre")]
    public float grabDuration = 2f;
    public float grabRadius = 1.5f;
    public Transform grabFollowPoint; // Punto donde el enemigo agarrado será colocado
    public int grabDamage = 30;

    public override void FrontalAttack()
    {
        IEnumerator Coroutine()
        {
            context.isAttacking = true;
            context.rb.velocity = Vector2.zero;
            context.animator.CrossFadeInFixedTime("FrontSpecialAttack", 0f);

            yield return new WaitForSeconds(0.18f + 0.25f);
            GameObject bocata = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
            bocata.GetComponent<BallScript>().thrower = this.gameObject;
            Rigidbody rbBocata = bocata.GetComponent<Rigidbody>();
            rbBocata.AddForce(new Vector3(context.GetActualPlayerDirection(), 1, 0) * ballForce, ForceMode.Force);

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

            specialUpHurtBox.enabled = true;
            context.rb.AddForce(new Vector2(0, gorraJumpForce), ForceMode.Impulse);
            SoundManager.Instance.PlayRandomSound(specialUpSounds);

            yield return new WaitForSeconds(1f);

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
            context.animator.CrossFadeInFixedTime("GrabAttack", 0f);

            yield return new WaitForSeconds(0.2f); // Delay antes de aplicar el agarre

            Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * grabRadius, grabRadius);

            foreach (Collider hit in hits)
            {
                if (hit.gameObject == this.gameObject) continue; // No agarrarse a uno mismo

                PlayerLive enemy = hit.GetComponent<PlayerLive>();
                if (enemy != null)
                {
                    Debug.Log($"Hitted {enemy.name}");
                    enemy.TakeDamage(grabDamage, grabDuration);
                    StartCoroutine(GrabFollowCoroutine(enemy));
                    break; // Solo agarramos a uno
                }
            }
            context.isAttacking = false;
            
            context.animator.CrossFadeInFixedTime("Idle", grabDuration);
            yield return new WaitForSeconds(grabDuration);

            context.isAttacking = false;
        }

        StartAttackCoroutine(Coroutine());
    }

    private IEnumerator GrabFollowCoroutine(PlayerLive enemy)
    {
        Transform enemyTransform = enemy.transform;
        Transform followTarget = grabFollowPoint;

        float elapsed = 0f;

        // Desactiva control del enemigo si tiene un controlador (opcional)

        while (elapsed < grabDuration)
        {
            if (enemyTransform != null && followTarget != null)
            {
                enemyTransform.position = followTarget.position;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public override void BaseFrontalAttack() { }
    public override void BaseUpAttack() { }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * grabRadius, grabRadius);
    }
}

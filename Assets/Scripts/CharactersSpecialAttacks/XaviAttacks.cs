using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XaviAttacks : CharacterAttack
{
    [Header("Special Up / Music")]
    [SerializeField] private int specialUpDamage;
    public float specialUpStunTime;
    public float musicJumpForce;
    public Collider specialUpHurtBox;
    public ParticleSystem musicParticles;
    public Transform musicParticlesPosition; 

    public override int SpecialUpDamage => specialUpDamage;

    [Header("Special Frontal / Puñetazo")]
    public int frontalAttackDamage;
    public float frontalAttackStunTime;
    public float specialFrontRecoveryTime; 
    public Transform frontalAttackPosition;
    public float frontalAttackArea = 3f;
    public ParticleSystem groundParticles;

    [Header("Special Down / Ataque Laser")]
    public int laserDamage = 30;
    public float laserStunTime;
    public float laserDuration = 0.5f;
    public float laserLength = 20f;
    public float laserNoise = 0.2f;
    public float laserRecoveryTime = 1.5f;
    public Transform laserOrigin;
    public LineRenderer laserLine;

    [Header("Basic Frontal")]
    public int basicFrontAttackDamage;
    public float basicFrontStunTime;
    public float basicFrontAttackArea;
    public float basicFrontRecoveryTime;
    public Transform basicAttackPosition;

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
        void SendLaser(float xDistanceMultiplier)
        {
            Vector3 direction = new Vector3(context.GetActualPlayerDirection() * xDistanceMultiplier, -1, 0).normalized;
            Vector3 rayOrigin = laserOrigin.position;
            Vector3 hitPoint = rayOrigin + direction * laserLength;

            // Raycast para daño
            if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, laserLength))
            {
                hitPoint = hit.point;

                if (hit.collider.TryGetComponent(out PlayerLive playerLive))
                {
                    playerLive.TakeDamage(laserDamage, laserStunTime, transform);
                }
            }

            // Raycast visual solo contra Ground
            int groundLayerMask = LayerMask.GetMask("Ground");
            if (Physics.Raycast(rayOrigin, direction, out RaycastHit visualHit, laserLength, groundLayerMask))
            {
                hitPoint = visualHit.point;
            }

            // Dibujar rayo visual
            laserLine.SetPosition(0, rayOrigin);
            laserLine.SetPosition(1, hitPoint);
            laserLine.gameObject.SetActive(true);
            laserLine.enabled = true;
        }

        void DisableLaser()
        {
            laserLine.enabled = false;
            laserLine.gameObject.SetActive(false);
        }

        IEnumerator Coroutine()
        {
            context.isAttacking = true;
            context.rb.velocity = Vector2.zero;

            SoundManager.Instance.PlayRandomSound(specialFrontSounds);

            context.animator.CrossFadeInFixedTime("SpecialDown", 0f);

            SendLaser(2.5f);
            yield return new WaitForSeconds(laserDuration);
            DisableLaser();

            SendLaser(1.5f);
            yield return new WaitForSeconds(laserDuration);
            DisableLaser();

            SendLaser(1);
            yield return new WaitForSeconds(laserDuration);
            DisableLaser();

            context.animator.CrossFadeInFixedTime("Idle", laserRecoveryTime);
            yield return new WaitForSeconds(laserRecoveryTime);
            
            context.isAttacking = false;
        }

        StartAttackCoroutine(Coroutine());
    }

    //Música
    public override void SpecialUpAttack()
    {
        IEnumerator Coroutine()
        {
            context.isAttacking = true;
            specialUpHurtBox.enabled = true;
            context.rb.velocity = Vector3.zero;
            context.animator.CrossFadeInFixedTime("SpecialUp", 0f);

            SoundManager.Instance.PlayRandomSound(specialUpSounds);

            int xDirection = context.GetActualPlayerDirection();
            context.rb.AddForce(new Vector2(xDirection * musicJumpForce, musicJumpForce), ForceMode.Impulse);

            musicParticles.transform.position = musicParticlesPosition.position;
            musicParticles.Play();

            yield return new WaitForSeconds(1f);
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
            context.animator.CrossFadeInFixedTime("FrontSpecialAttack", 0f);
            SoundManager.Instance.PlayRandomSound(specialDownSounds);

            yield return new WaitForSeconds(0.26f + 0.25f);

            Collider[] hits = Physics.OverlapSphere(frontalAttackPosition.position, frontalAttackArea, enemyLayer);
            groundParticles.transform.position = frontalAttackPosition.position;
            groundParticles.Play();

            foreach (Collider hit in hits)
            {
                if (hit.gameObject != this.gameObject)
                {
                    hit.GetComponent<PlayerLive>()?.TakeDamage(frontalAttackDamage, frontalAttackStunTime, transform);
                }
            }
            context.animator.CrossFadeInFixedTime("Idle", specialFrontRecoveryTime);
            yield return new WaitForSeconds(specialFrontRecoveryTime);
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

            Collider[] hits = Physics.OverlapSphere(basicAttackPosition.position, basicFrontAttackArea, enemyLayer);
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
        laserLine.enabled = false;
        laserLine.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        // Special Down (Puñetazo)
        Gizmos.color = Color.red;
        if (frontalAttackPosition != null)
            Gizmos.DrawWireSphere(frontalAttackPosition.position, frontalAttackArea);

        // Basic Frontal Attack
        Gizmos.color = Color.yellow;
        if (basicAttackPosition != null)
            Gizmos.DrawWireSphere(basicAttackPosition.position, basicFrontAttackArea);

        // Basic Up Attack
        Gizmos.color = Color.cyan;
        if (basicUpAttackPosition != null)
            Gizmos.DrawWireSphere(basicUpAttackPosition.position, basicUpAttackArea);
    }
}

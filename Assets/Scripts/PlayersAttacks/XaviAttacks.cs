using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XaviAttacks : CharacterAttack
{
    [Header("Special Up / Music")]
    public float musicJumpForce;
    [SerializeField] private int specialUpDamage;
    public AudioClip[] specialUpSounds;

    public override int SpecialUpDamage => specialUpDamage;
    public Collider specialUpHurtBox;

    [Header("Special Frontal / Puñetazo")]
    public float specialFrontRecoveryTime; 
    public int frontalAttackDamage;
    public Transform frontalAttackPosition;
    public float frontalAttackArea = 3f;
    public ParticleSystem groundParticles;

    [Header("Special Down / Ataque Laser")]
    public Transform laserOrigin;
    public LineRenderer laserLine;
    public float laserLength = 20f;
    public float laserDuration = 0.5f;
    public int laserDamage = 30;
    public float laserNoise = 0.2f;
    public float rayDistance;

    //public float specialDownaAffectArea = 3f;
    public float stunTime = 1f;
    //public int specialDownDamage = 50;
    public LayerMask enemyLayer;

    public override void FrontalAttack()
    {
        void SendLaser(float xDistanceMultiplier)
        {
            Vector3 direction = new Vector3(context.GetActualPlayerDirection() * xDistanceMultiplier, -1, 0);
            RaycastHit hit;
            Vector3 endPoint;

            //Hit laser
            if (Physics.Raycast(laserOrigin.position, direction, out hit, laserLength))
            {
                endPoint = hit.point;

                PlayerLive playerLiveComponent = hit.collider.gameObject.GetComponent<PlayerLive>();
                if (playerLiveComponent != null)
                {
                    playerLiveComponent.TakeDamage(frontalAttackDamage, stunTime, transform);
                }
            }

            //Visual laser
            int groundLayerMask = LayerMask.GetMask("Ground");
            if (Physics.Raycast(laserOrigin.position, direction, out hit, laserLength, groundLayerMask))
            {
                endPoint = hit.point;
                laserLine.SetPosition(0, laserOrigin.position);
                laserLine.SetPosition(1, endPoint);
                laserLine.gameObject.SetActive(true);
                laserLine.enabled = true;
            }
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

            context.animator.CrossFadeInFixedTime("SpecialDown", 0f);

            SendLaser(3);
            yield return new WaitForSeconds(laserDuration);
            DisableLaser();

            SendLaser(2);
            yield return new WaitForSeconds(laserDuration);
            DisableLaser();

            SendLaser(1);
            yield return new WaitForSeconds(laserDuration);
            DisableLaser();

            context.animator.CrossFadeInFixedTime("Idle", 0f);


            context.isAttacking = false;
        }

        StartAttackCoroutine(Coroutine());
    }

    public override void UpAttack()
    {
        IEnumerator Coroutine()
        {
            context.isAttacking = true;
            specialUpHurtBox.enabled = true;
            context.rb.velocity = Vector3.zero;
            context.animator.CrossFadeInFixedTime("SpecialUp", 0f);

            int xDirection;
            //if (context.movementInput.x > 0)
            //{
            //    xDirection = 1;
            //}
            //else if (context.movementInput.x < 0)
            //{
            //    xDirection = -1;
            //}
            //else
            //{
                xDirection = context.GetActualPlayerDirection();
            //}  
            context.rb.AddForce(new Vector2(xDirection * musicJumpForce, musicJumpForce), ForceMode.Impulse);

            yield return new WaitForSeconds(1f);
            specialUpHurtBox.enabled = false;
            context.isAttacking = false;
        }

        StartAttackCoroutine(Coroutine());
    }

    public override void DownAttack()
    {
        // FRONTAL ATTACK
        IEnumerator Coroutine()
        {
            context.isAttacking = true;
            context.rb.velocity = Vector2.zero;
            context.animator.CrossFadeInFixedTime("FrontSpecialAttack", 0f);

            yield return new WaitForSeconds(0.26f + 0.25f);

            Collider[] hits = Physics.OverlapSphere(frontalAttackPosition.position, frontalAttackArea, enemyLayer);
            groundParticles.transform.position = frontalAttackPosition.position;
            groundParticles.Play();

            foreach (Collider hit in hits)
            {
                if (hit.gameObject != this.gameObject)
                {
                    hit.GetComponent<PlayerLive>()?.TakeDamage(frontalAttackDamage, stunTime, transform);
                }
            }

            context.animator.CrossFadeInFixedTime("Idle", specialFrontRecoveryTime);
            yield return new WaitForSeconds(specialFrontRecoveryTime);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(frontalAttackPosition.position, frontalAttackArea);
    }
}

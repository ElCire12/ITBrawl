using System.Collections;
using UnityEngine;

public class StunnedState : PlayerState
{
    private float stunDuration;
    private Coroutine stunCoroutine;

    public StunnedState(PlayerStateManager context, float duration) : base(context)
    {
        this.stunDuration = duration;
    }

    public override void Enter()
    {
        stunCoroutine = context.StartCoroutine(StunCoroutine());
    }

    private IEnumerator StunCoroutine()
    {
        context.isStunned = true;
        context.isAttacking = false;
        context.animator.SetBool("stuned", true);
        context.animator.Play("stuned");
        yield return new WaitForSeconds(stunDuration);
        context.animator.SetBool("stuned", false);
        context.isStunned = false;

        // Aqu� puedes decidir a qu� estado volver (por ejemplo Idle)
        context.ChangeState("Idle");
    }

    public override void Exit()
    {
        if (stunCoroutine != null)
        {
            context.StopCoroutine(stunCoroutine);
        }
    }

    public override void Update() {
    }

    public override void FixedUpdate() {
        if (context.isGrounded)
        {
            context.rb.AddForce(context.rb.velocity * -context.deceleration, ForceMode.Force);
        }

        if (Mathf.Abs(context.rb.velocity.x) > context.airMaxSpeed) // Limit player velocity
        {
            context.rb.velocity = new Vector2(context.airMaxSpeed * Mathf.Sign(context.rb.velocity.x), context.rb.velocity.y);
        }
    }
}
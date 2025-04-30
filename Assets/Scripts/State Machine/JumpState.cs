using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : PlayerState
{
    public JumpState(PlayerStateManager context) : base(context) { }
    public override void Enter()
    {
        context.rb.AddForce(new Vector2(0, 1 * context.jumpForce), ForceMode.Impulse);
        context.animator.SetTrigger("jump");
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        
    }

    public override void FixedUpdate()
    {

    }
}

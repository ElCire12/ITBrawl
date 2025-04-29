using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class AirState : PlayerState
{
    public AirState(PlayerStateManager context) : base(context) { }

    int jumpsCount = 0;
    bool jumpNextFrame = false;

    public override void Enter()
    {
        //Debug.Log("Enter Air State"); 
        jumpsCount = 0;
    }

    public override void Exit()
    {
        context.animator.SetBool("air_stunned", false); 
    }

    public override void Update()
    {
        if (context.jumpStarted)
        {
            jumpNextFrame = true;
        }
    }

    public override void FixedUpdate()
    {
        //Detect double jump
        if (jumpNextFrame && jumpsCount < context.maxAirJumps && context.previousState != "AttackingState")
        {
            context.rb.velocity = Vector3.zero;
            context.rb.AddForce(new Vector2(0, 1 * context.jumpForce), ForceMode.Impulse);
            jumpsCount++;
            jumpNextFrame= false;   
        }

        // Si ya no tiene mas saltos
        if (jumpsCount >= context.maxAirJumps)
        {
            context.animator.SetBool("air_stunned", true);
        }
        else
        {
            //Rotate player in the air
            if (context.movementInput.x > 0)
            {
                context.visuals.eulerAngles = new Vector3(0f, 90f, 0f);
            }
            else if (context.movementInput.x < 0)
            {
                context.visuals.eulerAngles = new Vector3(0f, -90f, 0f);
            }
        }

        //Add Extra Gravity
        context.rb.AddForce(Vector3.down * context.extraFallGravity, ForceMode.Acceleration);

        //Air Movement
        if (context.movementInput.x != 0)
        {
            Vector3 force = new Vector3(context.movementInput.x * context.airAcceleration, 0f, 0f);
            context.rb.AddForce(force, ForceMode.Force);
        }

        //Limit Air Movement
        if (Mathf.Abs(context.rb.velocity.x) > context.airMaxSpeed) // Limit player velocity
        {
            context.rb.velocity = new Vector2(context.airMaxSpeed * Mathf.Sign(context.rb.velocity.x), context.rb.velocity.y);
            //context.rb.velocity = context.rb.velocity.normalized * context.maxSpeed;
        }

        jumpNextFrame = false;
    }
}

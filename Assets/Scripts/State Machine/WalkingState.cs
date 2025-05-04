using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Windows;

public class WalkingState : PlayerState
{
    public WalkingState(PlayerStateManager context) : base(context) { }

    public override void Enter()
    {
        context.animator.SetFloat("moving", 1f);
    }

    public override void Exit()
    {
        context.animator.SetFloat("moving", 0f);
    }

    public override void Update()   
    {
        
    }

    public override void FixedUpdate()
    {
        if (Mathf.Abs(context.movementInput.x) > 0.3f) //El jugador está en moviment
        {
            if (DidPlayerFlip()) // Codi a executar quan el jugador fa un flip 
            {
                context.rb.velocity = Vector2.zero; //Stop Player

                context.animator.Play("PlayerFlip");

                //Girar al jugador
                if (context.movementInput.x > 0)
                {
                    context.visuals.eulerAngles = new Vector3(0f, 90f, 0f);
                }
                else if (context.movementInput.x < 0)
                {
                    context.visuals.eulerAngles = new Vector3(0f, -90f, 0f);
                }

                if (context.movementInput.x > 0)
                {
                    context.visuals.eulerAngles = new Vector3(0f, 90f, 0f);
                }
                else if (context.movementInput.x < 0)
                {
                    context.visuals.eulerAngles = new Vector3(0f, -90f, 0f);
                }
            }

            context.rb.AddForce(new Vector2 (context.movementInput.x, 0) * context.acceleration, ForceMode.Force);

            if (context.rb.velocity.magnitude > context.maxSpeed) // Limit player velocity
            {
                context.rb.velocity = context.rb.velocity.normalized * context.maxSpeed;
            }
        }

        else { // if player is not pressing movement decelerate
            context.rb.AddForce(context.rb.velocity * -context.deceleration, ForceMode.Force);
        }
    }

    
    bool DidPlayerFlip()
    {
        int actualPlayerDirection = context.GetActualPlayerDirection();
        float xInput = context.movementInput.x;
        float flipPoint = 0.3f; 

        if (actualPlayerDirection == 1 && xInput < -flipPoint || actualPlayerDirection == -1 && xInput > flipPoint)
        {
            return true;
        }

        else { return false; }
    }
}

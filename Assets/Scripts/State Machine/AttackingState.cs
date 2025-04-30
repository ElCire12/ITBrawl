using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : PlayerState
{
    public AttackingState(PlayerStateManager context) : base(context) { }
    
    public override void Enter()
    {
        //Ataque arriba
        if (context.movementInput.y > 0.5f)
        {
            context.characterAttacks.UpAttack();
        }

        //Ataque Frontal
        else if (context.movementInput.y > -0.5f && context.movementInput.y < 0.5f) 
        {   
            //Reproducir logica
            context.characterAttacks.FrontalAttack(); 
        }
        //Ataque Abajo
        else if (context.movementInput.y < -0.5f)
        {
            context.rb.velocity = Vector2.zero;

            //Reproducir logica
            context.characterAttacks.DownAttack();
        }
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        //AnimatorStateInfo state = context.animator.GetCurrentAnimatorStateInfo(0);

        //Debug.Log(state.ToString());
        //if (state.IsName("SpecialUp"))
        //{
        //    if (state.normalizedTime >= 1f)
        //    {
        //        Debug.Log("Animation finished"); 
        //        context.isAttacking = false;
        //    }
        //}
    }

    public override void FixedUpdate()
    {

    }
}

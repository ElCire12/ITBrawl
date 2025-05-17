using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttackingState : PlayerState
{
    public SpecialAttackingState(PlayerStateManager context) : base(context) { }

    private Coroutine currentCoroutine;

    public override void Enter()
    {
        //Ataque arriba
        if (context.movementInput.y > 0.5f)
        {
            context.characterAttacks.SpecialUpAttack();
        }

        //Ataque Frontal
        else if (context.movementInput.y > -0.5f && context.movementInput.y < 0.5f) 
        {   
            //Reproducir logica
            context.characterAttacks.SpecialFrontalAttack(); 
        }
        //Ataque Abajo
        else if (context.movementInput.y < -0.5f)
        {
            context.rb.velocity = Vector2.zero;

            //Reproducir logica
            context.characterAttacks.SpecialDownAttack();
        }
    }

    public override void Exit()
    {
        context.characterAttacks.StopCurrentAttackCoroutine();
    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {

    }
}

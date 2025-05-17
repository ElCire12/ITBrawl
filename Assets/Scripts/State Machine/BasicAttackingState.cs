using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttackingState : PlayerState
{
    public BasicAttackingState(PlayerStateManager context) : base(context) { }

    private Coroutine currentCoroutine;

    public override void Enter()
    {
        //Ataque arriba
        if (context.movementInput.y > 0.5f)
        {
            context.characterAttacks.BasicUpAttack();

        }

        //Ataque Frontal
        else if (context.movementInput.y > -0.5f && context.movementInput.y < 0.5f)
        {
            context.characterAttacks.BasicFrontalAttack();
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

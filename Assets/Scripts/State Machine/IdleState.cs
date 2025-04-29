using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerStateManager context) : base(context) { }
    public override void Enter()
    {
        //Debug.Log("Entró a Idle");
    }

    public override void Exit()
    {
        //Debug.Log("Salió de Idle");
    }

    public override void Update()
    {
        // Aquí solo lógica de caminar, sin cambiar de estado
    }

    public override void FixedUpdate()
    {
        context.rb.AddForce(context.rb.velocity * -context.deceleration, ForceMode.Force);
    }
}

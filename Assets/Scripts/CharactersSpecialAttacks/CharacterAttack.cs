using System.Collections;
using UnityEngine;

public abstract class CharacterAttack : MonoBehaviour
{
    protected PlayerStateManager context;
    private Coroutine activeAttackCoroutine;

    public abstract int SpecialUpDamage { get; }

    protected virtual void Awake()
    {
        context = GetComponent<PlayerStateManager>();
    }

    protected void StartAttackCoroutine(IEnumerator routine)
    {
        StopCurrentAttackCoroutine();
        activeAttackCoroutine = StartCoroutine(routine);
    }

    public void StopCurrentAttackCoroutine()
    {
        if (activeAttackCoroutine != null)
        {
            StopCoroutine(activeAttackCoroutine);
            activeAttackCoroutine = null;
            context.isAttacking = false; 
            ResetAttackState();
        }
    }

    public abstract void SpecialFrontalAttack();
    public abstract void SpecialUpAttack();
    public abstract void SpecialDownAttack();
    public abstract void BasicFrontalAttack();
    public abstract void BasicUpAttack();

    public abstract void ResetAttackState();
}

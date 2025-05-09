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
            context.isAttacking = false; // Reset estado por si quedó a true
        }
    }

    public abstract void FrontalAttack();
    public abstract void UpAttack();
    public abstract void DownAttack();
    public abstract void BaseFrontalAttack();
    public abstract void BaseUpAttack();
}

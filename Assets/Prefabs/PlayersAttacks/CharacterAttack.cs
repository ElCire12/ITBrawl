using UnityEngine;

public abstract class CharacterAttack : MonoBehaviour
{
    protected PlayerStateManager context;
    public abstract int SpecialUpDamage { get; }
    protected virtual void Awake()
    {
        context = GetComponent<PlayerStateManager>();
    }

    public abstract void FrontalAttack();
    public abstract void UpAttack();
    public abstract void DownAttack();
    public abstract void BaseFrontalAttack();
    public abstract void BaseUpAttack();
}
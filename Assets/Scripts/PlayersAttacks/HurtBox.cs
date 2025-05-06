using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    public CharacterAttack CharacterAttack;
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerLive enemyPlayerLive = other.GetComponent<PlayerLive>();
        if (enemyPlayerLive)
        {
            enemyPlayerLive.TakeDamage(CharacterAttack.SpecialUpDamage, 0.5f,transform.parent.transform);
        }
    }
}

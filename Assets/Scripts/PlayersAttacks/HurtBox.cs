using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    public CharacterAttack CharacterAttack;
    
    //private void OnCollisionEnter(Collision other)
    //{
    //    Debug.Log($"Collisioned with {other.gameObject.name}");
    //    other.gameObject.GetComponent<PlayerLive>().TakeDamage(CharacterAttack.SpecialUpDamage);
    //}

    private void OnTriggerEnter(Collider other)
    {
        PlayerLive enemyPlayerLive = other.GetComponent<PlayerLive>();
        if (enemyPlayerLive)
        {
            enemyPlayerLive.TakeDamage(CharacterAttack.SpecialUpDamage);
        }
    }
}

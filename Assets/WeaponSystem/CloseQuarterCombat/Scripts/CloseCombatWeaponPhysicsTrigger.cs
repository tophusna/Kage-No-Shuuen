using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CloseCombatWeaponPhysicsTrigger : CloseCombatWeaponBase
{
    [SerializeField] GameObject bomb;

    private bool hasSpawnedBomb = false;
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag(tag) && IsSlashing)
        {
            IDamagereceiver damageReceiver = other.GetComponent<IDamagereceiver>();
            PerformDamage(damageReceiver);

            if (name.Contains("Bomb") && !hasSpawnedBomb)
            {
                GameObject bombInstance = Instantiate(bomb, transform.position, Quaternion.identity);
                bombInstance.GetComponentInChildren<SphereCollider>().radius = 5f;
                hasSpawnedBomb = true;
            }
        }
    }
}

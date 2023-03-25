using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEngine.AI;

public class SoldierBehaviour : BehaviourTree.Tree, IObjectPoolNotifier
{
    [SerializeField] SoldierType type = SoldierType.Runner;

    [Header("Tree Sub Roots")]
    [SerializeField] Node localMainRoot;

    // Shared Tree Cached components
    [Header("Shared Tree Cached Components")]
    [SerializeField] WeaponController weaponController;
    public WeaponController WeaponController => weaponController;
    [SerializeField] DecisionMaker decisionMaker;
    public DecisionMaker DecisionMaker => decisionMaker;
    [SerializeField] NavMeshAgent navMeshAgent; 
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    [SerializeField] CharacterAnimator characterAnimator;
    public CharacterAnimator CharacterAnimator => characterAnimator;
    [SerializeField] Transform player;
    public Transform Player => player;
    [SerializeField] DamageableWithLife damageable;
    public DamageableWithLife DamageableWithLife => damageable;

    // Shared Tree Properties
    [SerializeField] float patrolSpeed = 2f;
    public float PatrolSpeed => patrolSpeed;

    protected override Node SetUpTree()
    {
        return localMainRoot;
    }

    public void OnEnqueuedToPool()
    {
        // TODO: for example deactivate the ragdoll, set life again to full points, etc.
    }

    public void OnCreatedOrDequeuedFromPool(bool isCreated)
    {
        // This gets called before spawning, I may assign patrol parent here most probably.
    }
}

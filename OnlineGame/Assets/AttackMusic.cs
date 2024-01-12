using System.Collections;
using System.Collections.Generic;
using GamePlay;
using UnityEngine;

public class AttackMusic : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        AudioManager.Singleton.PlayAttack();
    }
}

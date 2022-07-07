using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gNox.BehaviorTreeVisualizer
{
    public class AttackNode : Node
    {
        private Transform playerTransform;
        private MeleeBasico enemyScript;
        private string ANIMATOR_STRING;

        public AttackNode(MeleeBasico script, string animatorString, Transform player)
        {
            Name = "Attack Node";

            enemyScript = script;
            ANIMATOR_STRING = animatorString;
            playerTransform = player;
        }

        protected override void OnReset()
        {

        }
        protected override NodeStatus OnRun()
        {
            if (!enemyScript.isAttacking)
            {
                enemyScript.isAttacking = true;
                enemyScript.agent.isStopped = true;

                enemyScript.gameObject.transform.LookAt(playerTransform);

                enemyScript.ChangeAnimationState(ANIMATOR_STRING);

                return NodeStatus.Success;
            }

            return NodeStatus.Running;
        }
    }

}
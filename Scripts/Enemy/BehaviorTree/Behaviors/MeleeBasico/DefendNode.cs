using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gNox.BehaviorTreeVisualizer
{
    public class DefendNode : Node
    {
        private Transform playerTransform;
        private MeleeBasico enemyScript;
        private string ANIMATOR_STRING;

        public DefendNode(MeleeBasico script, string animatorString, Transform player)
        {
            Name = "Defend Node";

            enemyScript = script;
            ANIMATOR_STRING = animatorString;
            playerTransform = player;
        }

        protected override void OnReset()
        {

        }
        protected override NodeStatus OnRun()
        {
            if (enemyScript.isTakingDamage)
            {
                enemyScript.agent.isStopped = true;

                enemyScript.gameObject.transform.LookAt(playerTransform);

                enemyScript.ChangeAnimationState(ANIMATOR_STRING);
            }
            else
            {
                return NodeStatus.Success;
            }

            return NodeStatus.Running;
        }
    }

}
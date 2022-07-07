namespace gNox.BehaviorTreeVisualizer
{
    public class canAttackPlayerCheck : Condition
    {
        private MeleeBasico enemyScript;
        private canAttackPlayer m_ActivityToCheckFor;

        public canAttackPlayerCheck(canAttackPlayer activity, MeleeBasico script) : base($"canSeePlayer Activity is: {activity}")
        {
            m_ActivityToCheckFor = activity;
            enemyScript = script;
        }

        protected override void OnReset()
        {

        }
        protected override NodeStatus OnRun()
        {
            StatusReason = $"Enemy {m_ActivityToCheckFor} the Player";

            return enemyScript.canAttackPlayer == m_ActivityToCheckFor ?
                NodeStatus.Success : NodeStatus.Failure;
        }
    }
}
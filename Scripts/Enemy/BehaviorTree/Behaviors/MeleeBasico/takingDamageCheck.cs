namespace gNox.BehaviorTreeVisualizer
{
    public class takingDamageCheck : Condition
    {
        private MeleeBasico enemyScript;
        private takingDamage m_ActivityToCheckFor;

        public takingDamageCheck(takingDamage activity, MeleeBasico script) : base($"canSeePlayer Activity is: {activity}")
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

            return enemyScript.takingDamage == m_ActivityToCheckFor ?
                NodeStatus.Success : NodeStatus.Failure;
        }
    }
}
namespace gNox.BehaviorTreeVisualizer
{
    public class canSeePlayerCheck : Condition
    {
        private MeleeBasico enemyScript;
        private canSeePlayer m_ActivityToCheckFor;

        public canSeePlayerCheck(canSeePlayer activity, MeleeBasico script) : base($"canSeePlayer Activity is: {activity}")
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

            return enemyScript.canSeePlayer == m_ActivityToCheckFor ?
                NodeStatus.Success : NodeStatus.Failure;
        }
    }
}
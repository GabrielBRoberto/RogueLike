using System;

namespace gNox.BehaviorTreeVisualizer
{
    [Serializable]
    public class BTGNodeLinkData
    {
        public string BaseNodeId;
        public string PortName;
        public string TargetNodeId;
    }
}
using System.Collections;
using UnityEngine;

namespace gNox.BehaviorTreeVisualizer
{
    public interface IBehaviorTree
    {
        NodeBase BehaviorTree { get; set; }
    }
}
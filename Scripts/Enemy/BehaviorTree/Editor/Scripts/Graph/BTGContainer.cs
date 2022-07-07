using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using UnityEngine;
using System;

namespace gNox.BehaviorTreeVisualizer
{
    [Serializable]
    public class BTGContainer : ScriptableObject
    {
        public List<BTGNodeLinkData> NodeLinks = new List<BTGNodeLinkData>();
        public List<BTGNodeData> BehaviorTreeNodeData = new List<BTGNodeData>();
    }

}
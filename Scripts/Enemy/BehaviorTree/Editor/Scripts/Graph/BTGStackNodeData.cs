using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using UnityEngine;
using System;

namespace gNox.BehaviorTreeVisualizer
{
    public class BTGStackNodeData : StackNode
    {
        public int ColumnId;
        public List<BTGNodeData> childNodes = new List<BTGNodeData>();
    }
}
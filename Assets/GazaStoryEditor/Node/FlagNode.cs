using System.Linq;
using Gaza.BaseStoryNode;
using Gaza.Story;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace Gaza.Root
{

    [NodeTint(0,128,0)]
    public class FlagNode : Node
    {
        
        [LabelText("节点类型")] public FlagNodeType flagType;

        public enum FlagNodeType
        {
            Start,
            End
        }

        [Input((ShowBackingValue.Never)),LabelText("<<<"), ShowIf("flagType", FlagNodeType.End)]
        public string pre;

        [Output(), LabelText(">>>"), ShowIf("flagType", FlagNodeType.Start)]
        public string next;

        public override object GetValue(NodePort port)
        {
            return null;
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            if (to.node.GetType().IsSubclassOf(typeof(BasicNode))) //判断to是否继承BasicNode
            {
                if (from.ValueType != to.node.GetType())
                {
                    Debug.LogError("Gaza：Flag节点 禁止连接" + to.node.GetType() + "节点");
                    GetPort(from.fieldName).Disconnect(to);
                }
            }
            if (to.node.GetType().IsSubclassOf(typeof(DelayTimeNode))) //判断to是否继承BasicNode
            {
                if (from.ValueType != to.node.GetType())
                {
                    Debug.LogWarning("Gaza：不建议您在Flag节点 后连接Delaytime节点");
                }
            }
        }
    }
    
}

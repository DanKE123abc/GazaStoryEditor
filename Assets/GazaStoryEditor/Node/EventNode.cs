using Gaza.BaseStoryNode;
using Sirenix.OdinInspector;
using XNode;

namespace Gaza.Event
{

    [Node.NodeTintAttribute(139, 0, 0)]
    public class EventNode : BasicNode
    {
        
        [LabelText("事件名称")] public string eventName;

        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }
    }
}

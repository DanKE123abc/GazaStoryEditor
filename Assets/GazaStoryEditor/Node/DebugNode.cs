using Gaza.BaseStoryNode;
using Sirenix.OdinInspector;
using XNode;

namespace Gaza.Event
{
    [Node.NodeTintAttribute(0,0,0)]
    public class DebugNode : BasicNode
    {

        [LabelText("类型")] public DebugModeType mode;
        [LabelText("内容")] public string text;
        
        public enum DebugModeType
        {
            [LabelText("Log")] Log,
            [LabelText("LogWarning")] LogWarning,
            [LabelText("LogError")] LogError
        }
        
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }
    }
}
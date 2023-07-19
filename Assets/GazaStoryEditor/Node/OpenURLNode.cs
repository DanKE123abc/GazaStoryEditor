using Gaza.BaseStoryNode;
using Sirenix.OdinInspector;
using XNode;

namespace Gaza
{
    [NodeTint(142,0,140)]
    public class OpenURLNode : BasicNode
    {
        [LabelText("网页链接")] public string url;
        
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }
    }
}
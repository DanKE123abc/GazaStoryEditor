using Gaza.BaseStoryNode;
using Sirenix.OdinInspector;
using XNode;

namespace Gaza
{
    [NodeTint(0, 139, 139)]
    public class SetActiveNode : BasicNode
    {
        [LabelText("SetActive")] public bool active;

        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }

    }
}
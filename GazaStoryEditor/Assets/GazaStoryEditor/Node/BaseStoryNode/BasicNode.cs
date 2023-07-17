using Sirenix.OdinInspector;
using XNode;

namespace Gaza.BaseStoryNode
{
    /// <summary>
    /// 事件类基础节点
    /// </summary>
    public class  BasicNode: Node
    {
        [Input(ShowBackingValue.Never), LabelText("<<事件")]
        public string triggerObj;
        
    }
}
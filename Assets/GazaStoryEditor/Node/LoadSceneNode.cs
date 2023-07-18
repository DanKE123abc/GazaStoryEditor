using Gaza.BaseStoryNode;
using Sirenix.OdinInspector;

namespace Gaza
{
    [NodeTint(61,30,52)]
    public class LoadSceneNode : BasicNode
    {
        [LabelText("加载场景")] public string scene;
        [LabelText("异步模式")] public bool asyn;
    }
}
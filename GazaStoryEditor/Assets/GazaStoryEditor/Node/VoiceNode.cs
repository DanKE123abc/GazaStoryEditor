using Gaza.BaseStoryNode;
using Sirenix.OdinInspector;
using XNode;

namespace Gaza
{
    [Node.NodeTintAttribute(255,140,0)]
    public class VoiceNode : BasicNode
    {

        [LabelText("文件名称")] public string filename;
        [LabelText("播放模式")] public VoiceModeType mode;
        
        public enum VoiceModeType
        {
            [LabelText("背景音乐")] BK,
            [LabelText("音频")] Sound,
            [LabelText("循环音频")] SoundLoop
        }
        
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }
    }
}
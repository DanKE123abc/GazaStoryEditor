using Gaza.BaseStoryNode;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace Gaza
{
    [Node.NodeTintAttribute(255,140,0)]
    public class VoiceNode : BasicNode
    {

        [LabelText("节点模式")] public VoiceManagerType voiceManagerType;
        public enum VoiceManagerType
        {
            [LabelText("播放")] Play,
            [LabelText("音量")] Value,
            [LabelText("暂停")] Pause,
            [LabelText("停止")] Stop
        }

        #region 播放UI
        public enum VoiceModeType
        {
            [LabelText("背景音乐")] BK,
            [LabelText("音频")] Sound,
            [LabelText("音频(循环)")] SoundLoop
        }
        
        [ShowIf("voiceManagerType", VoiceManagerType.Play),LabelText("文件名称")] 
        public string filename;
        [ShowIf("voiceManagerType", VoiceManagerType.Play),LabelText("播放模式")] 
        public VoiceModeType mode;
        #endregion

        #region 音量UI
        public enum ValueModeType
        {
            [LabelText("背景音乐")] BK,
            [LabelText("音频")] Sound
        }
        [ShowIf("voiceManagerType", VoiceManagerType.Value),LabelText("调整音量")]
        public ValueModeType valueType;
        [ShowIf("voiceManagerType", VoiceManagerType.Value), LabelText("音量大小")]
        public float value;
        #endregion

        #region 暂停UI
        public enum PauseModeType
        {
            [LabelText("背景音乐")] BK
        }
        [ShowIf("voiceManagerType", VoiceManagerType.Pause),LabelText("暂停")]
        public PauseModeType PauseType;
        #endregion
        
        #region 停止UI
        public enum StopModeType
        {
            [LabelText("背景音乐")] BK,
            [LabelText("音频")] Sound
        }
        [ShowIf("voiceManagerType", VoiceManagerType.Stop),LabelText("停止")]
        public StopModeType StopType;

        [ShowIf("StopType", StopModeType.Sound),LabelText("音频")]
        public AudioSource source;
        #endregion
        
        
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }
    }
}
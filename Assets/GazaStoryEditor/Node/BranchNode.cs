using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace Gaza
{
    [NodeTint(102, 0, 153)]
    public class BranchNode : Node
    {
        [LabelText("名字")] public string thename;

        [PreviewField(Alignment = ObjectFieldAlignment.Left), LabelText("头像")]
        public Sprite head;

        [TextArea, LabelText("内容")] public string question;

        [Input(ShowBackingValue.Never), LabelText("<<<")]
        public string pre;

        [Output(dynamicPortList = true), LabelText("选项"), TextArea]
        public List<string> branchs;

        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }

    }
}

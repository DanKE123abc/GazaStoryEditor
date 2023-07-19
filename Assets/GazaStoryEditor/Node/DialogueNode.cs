﻿using System.Collections.Generic;
using System.Linq;
using Gaza.BaseStoryNode;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;
using Node = XNode.Node;

namespace Gaza
{
	[NodeTint(0,0,128)]
	public class DialogueNode : Node
	{
		[LabelText("名字")] public string thename;

		[PreviewField(Alignment = ObjectFieldAlignment.Left), LabelText("头像")]
		public Sprite head;

		[TextArea, LabelText("内容")] public List<string> contents;

		[Input(ShowBackingValue.Never), LabelText("<<<")]
		public string pre;

		[LabelText(">>>")] public NextType nextType;

		[ShowIf("nextType", NextType.Dialogue), Output, LabelText("对话>>")]
		public DialogueNode nextDialogue;

		[ShowIf("nextType", NextType.Branch), Output, LabelText("分支>>")]
		public BranchNode nextBranch;

		[ShowIf("nextType", NextType.Flag), Output, LabelText("标志>>")]
		public FlagNode nextFlag;
		
		[ShowIf("nextType", NextType.DelayTime), Output, LabelText("延时>>")]
        public DelayTimeNode nextDelaytime;

		[Output, LabelText("事件>>")][SerializeReference]public BasicNode trigger;

		//下一个节点类型
		public enum NextType
		{
			[LabelText("对话")] Dialogue,
			[LabelText("分支")] Branch,
			[LabelText("标志")] Flag,
			[LabelText("延时")] DelayTime
		}

		//类型与名字存起来 对连接进行限制时使用
		private Dictionary<NextType, string> singleDt = new Dictionary<NextType, string>()
		{
			{ NextType.Dialogue, nameof(nextDialogue) },
			{ NextType.Branch, nameof(nextBranch) },
			{ NextType.Flag, nameof(nextFlag) },
			{ NextType.DelayTime, nameof(nextDelaytime) }
		};

		public override object GetValue(NodePort port)
		{
			return null;
		}

		//当值更新时 （编辑器下）
		private void OnValidate()
		{
			//切换下一个类型的选项时 对所连接的节点进行限制
			foreach (var s in singleDt)
			{
				if (nextType != s.Key)
				{
					GetPort(s.Value).ClearConnections();
				}
			}
		}

		public override void OnCreateConnection(NodePort from, NodePort to)
		{
			if (!to.node.GetType().IsSubclassOf(typeof(BasicNode))) //判断to是否继承BasicNode，不继承则继续
			{
				//限定连接节点类型
				if (Outputs.Contains(from))
				{

					if (from.ValueType != to.node.GetType())
					{
						Debug.LogError("Gaza："+from.ValueType + "节点 与" + to.node.GetType() + "节点 不匹配");
						GetPort(from.fieldName).Disconnect(to);
					}

				}
			}
		}
	}
}
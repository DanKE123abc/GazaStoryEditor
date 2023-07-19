using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNodeEditor;

namespace Gaza
{
	[CustomNodeGraphEditor(typeof(GazaStoryGraph))]
	public class GazaStoryGraphEditor : NodeGraphEditor
	{
		public override void OnGUI()
		{
			base.OnGUI();
			var style = new GUIStyle
			{
				normal = { textColor = new Color(131 / 255f, 175 / 255f, 155 / 255f) }, fontSize = 30
			};
			GUILayout.BeginVertical();
			GUILayout.Space(50);
			GUILayout.FlexibleSpace();
			GUILayout.Label("故事图纸", style);
			GUILayout.EndVertical();
		}
		
		public override string GetNodeMenuName(System.Type type)
		{
			if (type.Namespace == "Gaza.Root")
			{
				return base.GetNodeMenuName(type).Replace("Gaza/Root/", "根节点/");
			}
			else if (type.Namespace == "Gaza.Story")
			{
				return base.GetNodeMenuName(type).Replace("Gaza/Story/", "故事节点/");
			}
			else if (type.Namespace == "Gaza.Event")
			{
				return base.GetNodeMenuName(type).Replace("Gaza/Event/", "事件节点/");
			}
			else return null;
		}
		
	}

}
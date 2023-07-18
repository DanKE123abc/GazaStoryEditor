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
	}

}
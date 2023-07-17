using System.Collections;
using System.Collections.Generic;
using Gaza;
using UnityEngine;

namespace NewBehaviourScript
{

	///<summary>
	///脚本名称： NewBehaviourScript.cs
	///修改时间：
	///脚本功能：
	///备注：
	///</summary>

	public class NewBehaviourScript : MonoBehaviour
	{
    	void Start()
    	{
	        
    	}

    	void Update()
    	{
	        if (Input.GetKeyDown(KeyCode.Space))
	        {
		        GazaStoryManager storyManager = GetComponent<GazaStoryManager>();
		        storyManager.StartDialogue();
	        }
    	}
	}

}
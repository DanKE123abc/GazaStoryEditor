using System.Collections;
using System.Collections.Generic;
using Gaza;
using UnityEngine;

namespace TestDemo
{

	///<summary>
	///脚本名称： TMPTestStoryObject.cs
	///修改时间：
	///脚本功能：
	///备注：
	///</summary>

	public class TMPTestStoryObject : MonoBehaviour
	{
		private GazaStoryManager storyManager;
    	void Start()
    	{
	        storyManager = GetComponent<GazaStoryManager>();
	        storyManager.StartPlay();
        }

    	void Update()
    	{
	        if (Input.GetKeyDown(KeyCode.Space))
	        {
		        storyManager.Play();
	        }
    	}
	}

}
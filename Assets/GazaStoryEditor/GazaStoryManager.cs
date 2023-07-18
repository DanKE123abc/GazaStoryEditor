using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading;
using Dewity;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using XNode;
using SceneManager = Dewity.SceneManager;

namespace Gaza
{

    [AddComponentMenu("GazaStoryEditor/StoryManager")]
    public class GazaStoryManager : MonoBehaviour
    {
        
        [LabelText("对话框UI")]public GameObject dialogueUi;
        [LabelText("故事剧本")]public GazaStoryGraph storyGraph;

        private bool dialogueLock;
        private Text content;
        private Text speaker;
        private Image head;

        private Node currentNode;
        private List<string> contentList = new List<string>();
        private List<Button> branchBtns = new List<Button>();
        [LabelText("选项按钮预制体")]public GameObject branchPb;

        
        /// <summary>
        /// 按照步骤演绎剧本
        /// </summary>
        public void Play()
        {
            if (!dialogueLock)
            {
                PlayStory();
            }
        }

        /// <summary>
        /// 故事锁
        /// </summary>
        public void SetStoryLock(bool Lock)
        {
            dialogueLock = Lock;
        }
        
        
        
        // 演绎剧本
        private void PlayStory()
        {
            if (currentNode == null)
            {
                dialogueUi.SetActive(true);
                currentNode = storyGraph.nodes[0].GetOutputPort("next").Connection.node;
                //Debug.Log(currentNode);
                UpdateDialogueUi(currentNode);
            }
            else if (currentNode.GetType() == typeof(DialogueNode))
            {
                //第一个节点是DialogueNode
                ShowContent();
            }
            else if (currentNode.GetType() == typeof(BranchNode))
            {
                //第一个节点是BranchNode
                AddBranchClick(currentNode as BranchNode);
            }
            else if (currentNode.GetType() == typeof(DelayTimeNode))
            {
                //第一个节点是DelaytimeNode
                DelayTime(currentNode as DelayTimeNode);
            }
        }

        // 生命周期
        private void Awake()
        {
            content = dialogueUi.transform.Find("Content").GetComponent<Text>();
            speaker = dialogueUi.transform.Find("Name").GetComponent<Text>();
            head = dialogueUi.transform.Find("Head").GetComponent<Image>();
        }
        private void Start()
        {
            dialogueUi.SetActive(false);
        }

        // 刷新对话Ui
        private void UpdateDialogueUi(Node current)
        {
            if (current.GetType() == typeof(DialogueNode))
            {
                var node = current as DialogueNode;
                if (node != null)
                {
                    // 添加对话内容到列表
                    contentList.AddRange(node.contents);
                    content.text = contentList[0];
                    speaker.text = node.speaker;
                    head.sprite = node.head;
                }
            }
            else if (current.GetType() == typeof(BranchNode))
            {
                var branchNode = current as BranchNode;
                if (branchNode != null)
                {
                    contentList.Add(branchNode.question);
                    speaker.text = branchNode.speaker;
                    head.sprite = branchNode.head;
                }
            }
        }

        // Dialogue逻辑处理
        private void ShowContent()
        {
            if (contentList.Count > 0)
            {
                contentList.RemoveAt(0);
                if (contentList.Count == 0)
                {
                    foreach (var connection in currentNode.GetOutputPort("trigger").GetConnections())
                    {
                        if (connection.node.GetType() == typeof(EventNode))
                        {
                            TriggerEvent(connection.node as EventNode);
                        }
                        else if (connection.node.GetType() == typeof(VoiceNode))
                        {
                            PlayVoice(connection.node as VoiceNode);
                        }
                        else if (connection.node.GetType() == typeof(SetActiveNode))
                        {
                            SetActiveNode setActiveNode = connection.node as SetActiveNode;
                            dialogueUi.SetActive(setActiveNode.active);
                        }
                        else if (connection.node.GetType() == typeof(DebugNode))
                        {
                            DebugLog(connection.node as DebugNode);
                        }
                        else if (connection.node.GetType() == typeof(LoadSceneNode))
                        {
                            LoadScene(connection.node as LoadSceneNode);
                        }
                    }

                    switch (currentNode.GetValuesByField("nextType"))
                    {
                        case DialogueNode.NextType.Dialogue:
                            currentNode = currentNode.GetNodeByField("nextDialogue");
                            var node = currentNode as DialogueNode;
                            if (node != null)
                            {
                                contentList.AddRange(node.contents);
                                speaker.text = node.speaker;
                                head.sprite = node.head;
                            }
                            break;
                        case DialogueNode.NextType.Branch:
                            currentNode = currentNode.GetNodeByField("nextBranch");
                            UpdateDialogueUi(currentNode);
                            AddBranchClick(currentNode as BranchNode);
                            break;
                        case DialogueNode.NextType.Flag:
                            currentNode = currentNode.GetNodeByField("nextFlag");
                            FlagNode flagNode = currentNode as FlagNode;
                            if (flagNode != null && flagNode.flagType == FlagNode.FlagNodeType.End)
                            {
                                dialogueUi.SetActive(false);
                            }
                            break;
                        case DialogueNode.NextType.DelayTime:
                            currentNode = currentNode.GetNodeByField("nextDelaytime");
                            DelayTime(currentNode as DelayTimeNode);
                            break;
                    }
                }

                // 显示下一段对话内容
                if (contentList.Count > 0)
                {
                    content.text = contentList[0];
                }
            }
        }

        // BrannchNode逻辑处理
        private void AddBranchClick(BranchNode node)
        {
            dialogueLock = true;
            for (int i = 0; i < node.branchs.Count; i++)
            {
                var branchPort = node.GetOutputPort("branchs " + i);
                var text = node.branchs[i];
                var btn = Instantiate(branchPb, dialogueUi.transform.Find("Select").transform, false).GetComponent<Button>();
                branchBtns.Add(btn);
                btn.GetComponentInChildren<Text>().text = text;

                if (branchPort.IsConnected)
                {
                    int index = i; // fix for closure
                    btn.onClick.AddListener(delegate
                    {
                        foreach (var connection in branchPort.GetConnections())
                        {
                            dialogueLock = false;   
                            if (connection.node.GetType() == typeof(EventNode))
                            {
                                TriggerEvent(connection.node as EventNode);
                            }
                            else if (connection.node.GetType() == typeof(VoiceNode))
                            {
                                PlayVoice(connection.node as VoiceNode);
                            }
                            else if (connection.node.GetType() == typeof(SetActiveNode))
                            {
                                SetActiveNode setActiveNode = connection.node as SetActiveNode;
                                dialogueUi.SetActive(setActiveNode.active);
                            }
                            else if (connection.node.GetType() == typeof(DebugNode))
                            {
                                DebugLog(connection.node as DebugNode);
                            }
                            else if (connection.node.GetType() == typeof(DelayTimeNode))
                            {
                                currentNode = connection.node;
                                contentList.RemoveAt(0);
                                DelayTime(connection.node as DelayTimeNode);
                            }
                            else if (connection.node.GetType() == typeof(DialogueNode))
                            {
                                currentNode = connection.node;
                                contentList.RemoveAt(0);
                                UpdateDialogueUi(currentNode);
                            }
                            else if (connection.node.GetType() == typeof(BranchNode))
                            {
                                currentNode = connection.node;
                                contentList.RemoveAt(0);
                                UpdateDialogueUi(currentNode);
                                AddBranchClick(currentNode as BranchNode);
                            }
                            else if (connection.node.GetType() == typeof(FlagNode))
                            {
                                // 不写判断惹，反正接在选项后面的一定是end
                                dialogueUi.SetActive(false);
                            }
                            else if (connection.node.GetType() == typeof(LoadSceneNode))
                            {
                                LoadScene(connection.node as LoadSceneNode);
                            }
                        }
                        
                        // 清除所有按钮
                        foreach (var btn in branchBtns)
                        {
                            Destroy(btn.gameObject);
                        }

                        branchBtns.Clear();
                    });

                }
            }
        }
        
        // EventNode逻辑处理
        private void TriggerEvent(EventNode node)
        {
            Debug.Log("剧本触发事件：Gaza." + node.eventName);
            EventCenter.instance.EventTrigger("Gaza."+node.eventName);
            
        }
        
        // VoiceNode逻辑处理
        private void PlayVoice(VoiceNode node)
        {
            if (node.voiceManagerType == VoiceNode.VoiceManagerType.Play)
            {
                if (node.mode == VoiceNode.VoiceModeType.BK)
                {
                    VoiceManager.instance.PlayBKMusic(node.filename);   
                }
                else if (node.mode == VoiceNode.VoiceModeType.Sound)
                {
                    VoiceManager.instance.PlaySound(node.filename , false);   
                }
                else if (node.mode == VoiceNode.VoiceModeType.SoundLoop)
                {
                    VoiceManager.instance.PlaySound(node.filename , true);   
                }
            }
            else if (node.voiceManagerType == VoiceNode.VoiceManagerType.Value)
            {
                if (node.valueType == VoiceNode.ValueModeType.BK)
                {
                    VoiceManager.instance.ChangeBKValue(node.value);
                }
                else if (node.valueType == VoiceNode.ValueModeType.Sound)
                {
                    VoiceManager.instance.ChangeSoundValue(node.value);
                }
            }
            else if (node.voiceManagerType == VoiceNode.VoiceManagerType.Pause)
            {
                if (node.PauseType == VoiceNode.PauseModeType.BK)
                {
                    VoiceManager.instance.PauseBKMusic();
                }
            }
            else if (node.voiceManagerType == VoiceNode.VoiceManagerType.Stop)
            {
                if (node.StopType == VoiceNode.StopModeType.BK)
                {
                    VoiceManager.instance.StopBKMusic();
                }
                else if (node.StopType == VoiceNode.StopModeType.Sound)
                {
                    VoiceManager.instance.StopSound(node.source);
                }
            }

        }
        
        // DebugNode逻辑处理
        private void DebugLog(DebugNode node)
        {
            if (node.mode == DebugNode.DebugModeType.Log)
            {
                Debug.Log(node.text);
            }
            else if (node.mode == DebugNode.DebugModeType.LogWarning)
            {
                Debug.LogWarning(node.text);
            }
            else if (node.mode == DebugNode.DebugModeType.LogWarning)
            {
                Debug.LogWarning(node.text);
            }

        }

        // DelaytimeNode逻辑处理
        private void DelayTime(DelayTimeNode delaytimeNode)
        {
            //Thread.Sleep(delaytimeNode.time);
            Invoke("DelayFunc",delaytimeNode.time);
        }
        private void DelayFunc()
        {
            foreach (var connection in currentNode.GetOutputPort("trigger").GetConnections())
            {
                if (connection.node.GetType() == typeof(EventNode))
                {
                    TriggerEvent(connection.node as EventNode);
                }
                else if (connection.node.GetType() == typeof(SetActiveNode))
                {
                    SetActiveNode setActiveNode = connection.node as SetActiveNode;
                    dialogueUi.SetActive(setActiveNode.active);
                }
                else if (connection.node.GetType() == typeof(VoiceNode))
                {
                    PlayVoice(connection.node as VoiceNode);
                }
                else if (connection.node.GetType() == typeof(DebugNode))
                {
                    DebugLog(connection.node as DebugNode);
                }
                else if (connection.node.GetType() == typeof(LoadSceneNode))
                {
                    LoadScene(connection.node as LoadSceneNode);
                }
                
            }
            switch (currentNode.GetValuesByField("nextType"))
            {
                case DelayTimeNode.NextType.Dialogue:
                    currentNode = currentNode.GetNodeByField("nextDialogue");
                    var node = currentNode as DialogueNode;
                    if (node != null)
                    {
                        contentList.AddRange(node.contents);
                        speaker.text = node.speaker;
                        head.sprite = node.head;
                    }

                    break;
                case DelayTimeNode.NextType.Branch:
                    currentNode = currentNode.GetNodeByField("nextBranch");
                    UpdateDialogueUi(currentNode);
                    AddBranchClick(currentNode as BranchNode);
                    break;
                case DelayTimeNode.NextType.Flag:
                    currentNode = currentNode.GetNodeByField("nextFlag");
                    FlagNode flagNode = currentNode as FlagNode;
                    if (flagNode != null && flagNode.flagType == FlagNode.FlagNodeType.End)
                    {
                        dialogueUi.SetActive(false);
                    }
                    break;
            }
        }

        // LoadSceneNode逻辑处理
        private void LoadScene(LoadSceneNode node)
        {
            if (!node.asyn)
            {
                SceneManager.instance.LoadScene(node.scene);
            }
            else if (node.asyn)
            {
                SceneManager.instance.LoadSceneAsyn(node.scene);
            }
            
        }


    }

}
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Dewity;
using Sirenix.OdinInspector;
using XNode;
using SceneManager = Dewity.SceneManager;

namespace Gaza
{

    /// <summary>
    /// GazaStoryManager
    /// </summary>
    [AddComponentMenu("GazaStoryEditor/StoryManager")]
    public class GazaStoryManager : MonoBehaviour
    {
        
        [LabelText("对话框UI")]public GameObject dialogueUi;
        [LabelText("故事剧本")]public GazaStoryGraph storyGraph;
        [LabelText("选项按钮预制体")]public GameObject branchPb;

        private bool _dialogueLock;
        private Text _content;
        private Text _name;
        private Image _head;

        private Node _currentNode;
        private List<string> _contentList = new List<string>();
        private List<Button> _branchBtns = new List<Button>();
        

        
        /// <summary>
        /// 按照步骤演绎剧本
        /// </summary>
        public void Play()
        {
            if (!_dialogueLock)
            {
                PlayStory();
            }
        }

        /// <summary>
        /// 故事锁
        /// 上锁状态时无法使用Play函数
        /// </summary>
        public void SetStoryLock(bool Lock)
        {
            _dialogueLock = Lock;
        }

        /// <summary>
        /// 开始演绎
        /// </summary>
        public void StartPlay()
        {
            if (_currentNode == null)
            {
                dialogueUi.SetActive(true);
                _currentNode = storyGraph.nodes[0].GetOutputPort("next").Connection.node;
                UpdateDialogueUi(_currentNode);
            }
            else
            {
                Debug.Log("Gaza：演绎已开始");
            }
        }
        
        
        
        // 演绎剧本
        private void PlayStory()
        {
            if (_currentNode == null)
            {
                Debug.Log("Gaza：剧本未开始演绎");
            }
            else if (_currentNode.GetType() == typeof(DialogueNode))
            {
                //第一个节点是DialogueNode
                ShowContent();
            }
            else if (_currentNode.GetType() == typeof(BranchNode))
            {
                //第一个节点是BranchNode
                AddBranchClick(_currentNode as BranchNode);
            }
            else if (_currentNode.GetType() == typeof(DelayTimeNode))
            {
                //第一个节点是DelaytimeNode
                DelayTime(_currentNode as DelayTimeNode);
            }
        }

        // 生命周期
        private void Awake()
        {
            _content = dialogueUi.transform.Find("Content").GetComponent<Text>();
            _name = dialogueUi.transform.Find("Name").GetComponent<Text>();
            _head = dialogueUi.transform.Find("Head").GetComponent<Image>();
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
                    _contentList.AddRange(node.contents);
                    _content.text = _contentList[0];
                    _name.text = node.speaker;
                    _head.sprite = node.head;
                }
            }
            else if (current.GetType() == typeof(BranchNode))
            {
                var branchNode = current as BranchNode;
                if (branchNode != null)
                {
                    _contentList.Add(branchNode.question);
                    _name.text = branchNode.speaker;
                    _head.sprite = branchNode.head;
                }
            }
        }

        // Dialogue逻辑处理
        private void ShowContent()
        {
            if (_contentList.Count > 0)
            {
                _contentList.RemoveAt(0);
                if (_contentList.Count == 0)
                {
                    foreach (var connection in _currentNode.GetOutputPort("trigger").GetConnections())
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
                        else if (connection.node.GetType() == typeof(OpenURLNode))
                        {
                            OpenURL(connection.node as OpenURLNode);
                        }
                    }

                    switch (_currentNode.GetValuesByField("nextType"))
                    {
                        case DialogueNode.NextType.Dialogue:
                            _currentNode = _currentNode.GetNodeByField("nextDialogue");
                            var node = _currentNode as DialogueNode;
                            if (node != null)
                            {
                                _contentList.AddRange(node.contents);
                                _name.text = node.speaker;
                                _head.sprite = node.head;
                            }
                            break;
                        case DialogueNode.NextType.Branch:
                            _currentNode = _currentNode.GetNodeByField("nextBranch");
                            UpdateDialogueUi(_currentNode);
                            AddBranchClick(_currentNode as BranchNode);
                            break;
                        case DialogueNode.NextType.Flag:
                            _currentNode = _currentNode.GetNodeByField("nextFlag");
                            FlagNode flagNode = _currentNode as FlagNode;
                            if (flagNode != null && flagNode.flagType == FlagNode.FlagNodeType.End)
                            {
                                dialogueUi.SetActive(false);
                            }
                            break;
                        case DialogueNode.NextType.DelayTime:
                            _currentNode = _currentNode.GetNodeByField("nextDelaytime");
                            DelayTime(_currentNode as DelayTimeNode);
                            break;
                    }
                }

                // 显示下一段对话内容
                if (_contentList.Count > 0)
                {
                    _content.text = _contentList[0];
                }
            }
        }

        // BrannchNode逻辑处理
        private void AddBranchClick(BranchNode node)
        {
            _dialogueLock = true;
            for (int i = 0; i < node.branchs.Count; i++)
            {
                var branchPort = node.GetOutputPort("branchs " + i);
                var text = node.branchs[i];
                var btn = Instantiate(branchPb, dialogueUi.transform.Find("Select").transform, false).GetComponent<Button>();
                _branchBtns.Add(btn);
                btn.GetComponentInChildren<Text>().text = text;

                if (branchPort.IsConnected)
                {
                    int index = i; // fix for closure
                    btn.onClick.AddListener(delegate
                    {
                        foreach (var connection in branchPort.GetConnections())
                        {
                            _dialogueLock = false;   
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
                                _currentNode = connection.node;
                                _contentList.RemoveAt(0);
                                DelayTime(connection.node as DelayTimeNode);
                            }
                            else if (connection.node.GetType() == typeof(DialogueNode))
                            {
                                _currentNode = connection.node;
                                _contentList.RemoveAt(0);
                                UpdateDialogueUi(_currentNode);
                            }
                            else if (connection.node.GetType() == typeof(BranchNode))
                            {
                                _currentNode = connection.node;
                                _contentList.RemoveAt(0);
                                UpdateDialogueUi(_currentNode);
                                AddBranchClick(_currentNode as BranchNode);
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
                            else if (connection.node.GetType() == typeof(OpenURLNode))
                            {
                                OpenURL(connection.node as OpenURLNode);
                            }
                        }
                        
                        // 清除所有按钮
                        foreach (var btn in _branchBtns)
                        {
                            Destroy(btn.gameObject);
                        }

                        _branchBtns.Clear();
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
            foreach (var connection in _currentNode.GetOutputPort("trigger").GetConnections())
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
                else if (connection.node.GetType() == typeof(OpenURLNode))
                {
                    OpenURL(connection.node as OpenURLNode);
                }
            }
            switch (_currentNode.GetValuesByField("nextType"))
            {
                case DelayTimeNode.NextType.Dialogue:
                    _currentNode = _currentNode.GetNodeByField("nextDialogue");
                    var node = _currentNode as DialogueNode;
                    if (node != null)
                    {
                        _contentList.AddRange(node.contents);
                        _name.text = node.speaker;
                        _head.sprite = node.head;
                    }

                    break;
                case DelayTimeNode.NextType.Branch:
                    _currentNode = _currentNode.GetNodeByField("nextBranch");
                    UpdateDialogueUi(_currentNode);
                    AddBranchClick(_currentNode as BranchNode);
                    break;
                case DelayTimeNode.NextType.Flag:
                    _currentNode = _currentNode.GetNodeByField("nextFlag");
                    FlagNode flagNode = _currentNode as FlagNode;
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
        
        // OpenURLNode逻辑处理
        private void OpenURL(OpenURLNode node)
        {
            Application.OpenURL(node.url);
        }
        
    }

}
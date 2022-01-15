using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace WolfGamer.Utils{
    public class DebugController : MonoBehaviour {

        public static DebugController i{get;private set;}

        public float guiBackGroundXPos,guiBackGroundYpos = 42.6f,guiTextFieldPosY = 4.8f,guiTextFieldHeightPadding,guiTextFieldHeight = 36.6f,guiHelpBoxYSize = 200;

        [SerializeField] private bool showDebugConsole;
        [SerializeField] private bool showHelp;
        private string input;
        public static DebugCommand<int> SKIP_LEVEL;
        public static DebugCommand HELP;
        
        private List<object> commandList;
        

        #region Item Refs.
        [SerializeField] private GameObject androidDebugButton;
        
        [SerializeField] private GameObject gui;
        [SerializeField] private TMP_InputField textField;
        [SerializeField] private ScrollRect cheatScrollView;
        [SerializeField] private Transform scrollViewContent;
        [SerializeField] private Transform lableText;
        private List<TextMeshProUGUI> cheatCodeList;
        private bool showDebugButton;

        #endregion
        
        private void Awake(){
            if(i == null){
                i = this;
            }else{
                Destroy(i.gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }
        
        private void CreateCheatList(){
            
            cheatCodeList = new List<TextMeshProUGUI>();
            for (int i = 0; i < commandList.Count; i++){
                GameObject cheatTextObjts = Instantiate(lableText.gameObject,scrollViewContent.position,Quaternion.identity) as GameObject;
                cheatTextObjts.transform.SetParent(scrollViewContent);
                TextMeshProUGUI lable = cheatTextObjts.GetComponent<TextMeshProUGUI>();
                cheatCodeList.Add(lable);
            }
            
        }
        private void ShowHelpWindow(bool _showHelpWindow){
            cheatScrollView.gameObject.SetActive(_showHelpWindow);
        }


        private void Start(){
            
            SKIP_LEVEL = new DebugCommand<int>("","Skip to a specified level","skip to",(levelNumber) =>{
                // LevelLoader.instance.SkipLevelTo(levelNumber);
            });
            HELP = new DebugCommand("help","Show all the command list","help", () => {
                showHelp = true;
            });
            commandList = new List<object>{
                SKIP_LEVEL,
                HELP,
            };
            CreateCheatList();
        }
        


        private void Update(){
            
            #if UNITY_EDITOR
            if(Input.GetKeyDown(KeyCode.Tab)){
                showDebugConsole = !showDebugConsole;
            }
            if(showDebugConsole){
                if(Input.GetKeyDown(KeyCode.Return)){
                    HandleInputs();
                }
            }
            #endif 
            if(Input.GetKeyDown(KeyCode.Escape)){
                showDebugButton = !showDebugButton;
            }
            androidDebugButton.SetActive(showDebugButton);
            ShowUIForCheat();
        }
        public void ShowHideDebugPanal(){
            #if UNITY_ANDROID
                showDebugConsole = !showDebugConsole;
            #endif
        }
        public void EnterCheat(){
            if(showDebugConsole){
                HandleInputs();
            }
        }
        private void HandleInputs(){
            string[] properties = input.Split(' ');
            for (int i = 0; i < commandList.Count; i++){
                DebugCommandBase commandbase = commandList[i] as DebugCommandBase;
                if(input.Contains(commandbase.CommandId)){
                    
                    if(commandList[i] as DebugCommand != null){
                        (commandList[i] as DebugCommand).InvokeCommandAction();
                    }
                    else if(commandList[i] as DebugCommand<int> != null){
                        (commandList[i] as DebugCommand<int>).InvokeCommandAction(int.Parse(input));
                    }
                }
            }
            input = "";
        }
        private void ShowUIForCheat(){
            if(showDebugConsole){
                if(showHelp){
                    cheatScrollView.gameObject.SetActive(true);
                    for (int i = 0; i < commandList.Count; i++){
                        DebugCommandBase command = commandList[i] as DebugCommandBase;
                        cheatCodeList[i].text = string.Concat(command.CommandId," - ", command.CommandDiscription);
                        ShowHelpWindow(true);

                    }
                }else{
                    
                    ShowHelpWindow(false);
                }
                gui.SetActive(true);

                input = textField.text;
                

            }
            else{
                gui.SetActive(false);
            }
        }
    }

}
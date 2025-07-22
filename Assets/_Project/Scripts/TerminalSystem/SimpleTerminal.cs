using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TerminalSystem
{
    public class SimpleTerminal : MonoBehaviour
    {
        // 终端状态
        private bool isRunning = false;
        private bool isCursorVisible = true;
        
        // 输入缓冲区
        private List<string> outputLines = new List<string>();
        private string currentInput = "";
        private int cursorPosition = 0;

        // 光标控制
        private float cursorBlinkTimer = 0f;
        public float cursorBlinkSpeed = 1.0f;

        // 显示设置
        public TMP_Text textMesh;

        public InputState currentInputState
        {
            get => _currentInputState;

            set
            {
                OnInputStateChange(value);
                _currentInputState = value;
            }
        }
        private InputState _currentInputState = InputState.Lock;
        
        public List<BaseCommand> commands = new List<BaseCommand>();
        public readonly Dictionary<string, BaseCommand> RegisteredCommands = new();
        
        public ScrollRect scrollRect;
        private RectTransform scrollRectTransform;

        public enum InputState
        {
            Lock, // 不接受任何输入
            Command, // 正常的shell命令执行（输入文字回车）
            Choice, // 选择（Y或N）
            AnyKey, // 任意键继续
        }

        private void OnInputStateChange(InputState state)
        {
	        if (state == _currentInputState)
	        {
		        return;
	        }
	        
            switch (state)
            {
                case InputState.Command:
                    AppendLine("root@localhost: ");
                    break;
            }
        }

        private void Start()
        {
            scrollRectTransform = scrollRect.GetComponent<RectTransform>();
            
            InitializeTerminal();
        }

        // 初始化终端
        public void InitializeTerminal(bool fresh = true)
        {
            if (fresh)
            {
                RegisteredCommands.Clear();

                foreach (var cmd in commands)
                {
                    RegisteredCommands.Add(cmd.commandName, Instantiate(cmd));
                }
                
                foreach (var command in RegisteredCommands)
                {
                    command.Value.Initialize(this);
                }
            }

            outputLines.Clear();
            currentInput = "";
            cursorPosition = 0;
            isRunning = true;
            AppendLine("DOS Retro Console for UGUI and TextMeshPro");
            AppendLine("Copyright @ Raymoyne Studios, 1995-2025.");
            AppendLine("");
            UpdateDisplay();
            currentInputState = InputState.Command;
        }

        // 每帧更新显示
        private void Update()
        {
            if (!isRunning) return;

            HandleCursorBlinking();
            UpdateDisplay();
            if(currentInputState != InputState.Lock)
                HandleInput();
        }

        #region API 接口

        /// <summary>
        /// 添加一行文本到终端
        /// </summary>
        public void AppendLine(string text = "")
        {
            outputLines.Add(text);
            UpdateDisplay();
            StartCoroutine(ForceScrollDown());
        }

        /// <summary>
        /// 追加字符到当前输入行
        /// </summary>
        public void Append(char c)
        {
            currentInput += c;
            cursorPosition = currentInput.Length;
            UpdateDisplay();
            StartCoroutine(ForceScrollDown());
        }

        /// <summary>
        /// 清空终端内容
        /// </summary>
        public void Clear()
        {
            outputLines.Clear();
            currentInput = "";
            cursorPosition = 0;
            UpdateDisplay();
            StartCoroutine(ForceScrollDown());
        }

        #endregion

        #region 输入处理

        // 处理键盘输入
        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Backspace) && currentInput.Length > 0)
            {
                currentInput = currentInput.Remove(Mathf.Max(0, cursorPosition - 1), 1);
                cursorPosition = Mathf.Max(0, cursorPosition - 1);
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.Return) && currentInputState == InputState.Command)
            {
                ExecuteCommand();
                return;
            }
            
            if(!string.IsNullOrEmpty(Input.inputString) 
               // 特殊字符（换行、退格、tab等）
               && !Input.inputString.Equals("\n") 
               && !Input.inputString.Equals("\b") 
               && !Input.inputString.Equals("\t") 
               && !Input.inputString.Equals("\r"))
            {
                currentInput = currentInput.Insert(cursorPosition, Input.inputString);
                cursorPosition++;
                return;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                cursorPosition = Mathf.Max(0, --cursorPosition);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                cursorPosition = Mathf.Min(currentInput.Length, ++cursorPosition);
            }
        }

        // 执行命令
        private void ExecuteCommand()
        {
	        if (!string.IsNullOrWhiteSpace(currentInput))
	        {
		        currentInputState = InputState.Lock;
		        string[] inputParts = currentInput.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		        if (inputParts.Length == 0) return;

		        string commandName = inputParts[0];
		        string[] args = inputParts.Skip(1).ToArray();

		        // 执行命令并传递参数
		        OnCommandExecuted(commandName, args);
		        
		        currentInput = "";
		        cursorPosition = 0;
		        
		        currentInputState = InputState.Command;
	        }
        }

        #endregion

        #region 显示控制

        // 更新显示内容
        private void UpdateDisplay()
        {
            List<string> displayLines = new List<string>(outputLines);
            
            string fullText = "";
            
            // 添加历史输出
            for (int i = 0; i < displayLines.Count; i++)
            {
	            if (i == 0)
	            {
		            fullText += displayLines[i];
		            continue;
	            }
	            
                fullText += "\n" + displayLines[i];
            }
            
            // 添加输入行
            if (!string.IsNullOrEmpty(currentInput))
            {
                fullText += currentInput;
                fullText += GetCursorVisual();
            }

            textMesh.text = fullText;
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRectTransform);
        }

        // 获取光标视觉效果
        private string GetCursorVisual()
        {
            return isCursorVisible ? "_" : "";
        }
        
        IEnumerator ForceScrollDown () {
	        // Wait for end of frame AND force update all canvases before setting to bottom.
	        yield return new WaitForEndOfFrame ();
	        Canvas.ForceUpdateCanvases ();
	        scrollRect.verticalNormalizedPosition = 0f;
	        Canvas.ForceUpdateCanvases ();
        }

        #endregion

        #region 光标控制

        // 处理光标闪烁逻辑
        private void HandleCursorBlinking()
        {
            cursorBlinkTimer += Time.deltaTime;
            
            if (cursorBlinkTimer >= cursorBlinkSpeed)
            {
                isCursorVisible = !isCursorVisible;
                cursorBlinkTimer = 0f;
            }
        }

        #endregion

        // 自定义命令执行回调
        protected virtual void OnCommandExecuted(string command, params string[] args)
        {
	        // 在此处添加自定义命令处理逻辑
	        Debug.Log($"执行命令: {command}, 参数: {string.Join(" ", args)}");
            
	        if (!RegisteredCommands.ContainsKey(command))
	        {
		        AppendLine($"{command} is not a recognized command or executable.");
		        return;
	        }

	        RegisteredCommands[command].Execute(args);
        }
    }
}

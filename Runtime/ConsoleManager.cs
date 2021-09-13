using System;
using UnityEngine;
using TMPro;
using System.Linq;

namespace Ilep.Console
{
	public class ConsoleManager : MonoBehaviour
	{
		public EventHandler<bool> ConsoleToggled;
		public EventHandler<string> InputEntered;

		public static ConsoleManager Instance;
		public bool IsOpen { get => m_canvas.activeInHierarchy; }
		public ConsoleCommand[] Commands { get => m_commands; }

		[SerializeField] private GameObject m_canvas;
		[SerializeField] private TMP_InputField m_input;
		[SerializeField] private TMP_Text m_log;
		[SerializeField] private ConsoleCommand[] m_commands;
		[SerializeField] private bool m_locked = true;

		public void LockConsole()
		{
			m_locked = true;
		}

		public void UnlockConsole()
		{
			m_locked = false;
		}

		public void ToggleConsole()
		{
			if (m_locked)
				return;
			if (m_canvas.activeInHierarchy)
				CloseConsole();
			else
				OpenConsole();
		}

		public void OpenConsole()
		{
			if (m_locked)
				return;
			if (IsOpen)
				return;
			m_canvas.SetActive(true);
			m_input.text = "";
			m_input.ActivateInputField();
			if (ConsoleToggled != null)
				ConsoleToggled.Invoke(this, true);
		}

		public void CloseConsole()
		{
			if (m_locked)
				return;
			if (!IsOpen)
				return;
			m_canvas.SetActive(false);
			if (ConsoleToggled != null)
				ConsoleToggled.Invoke(this, false);
		}

		public void PrintLine(string message)
		{
			m_log.text += message;
			m_log.text += "\n";
		}

		public void PrintString(string message)
		{
			m_log.text += message;
		}

		public void SetText(string text)
		{
			m_log.text = text;
		}

		private void OnInputEnter(string input)
		{
			input = input.ToLower();
			InputEntered?.Invoke(this, input);
			PrintLine("> " + input);
			string[] splitInput = SplitInput(input);
			if (splitInput == null)
				return;
			string[] args = splitInput.Skip(1).ToArray();
			foreach (ConsoleCommand cmd in m_commands)
			{
				if (cmd.CmdName.Equals(splitInput[0]))
				{
					cmd.Execute(args);
					break;
				}
			}
			m_input.text = "";
			m_input.ActivateInputField();
		}

		private string[] SplitInput(string input)
		{
			string[] inputArr = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (inputArr == null || inputArr.Length == 0)
				return null;
			return inputArr;
		}
	}
}
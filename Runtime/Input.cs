using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Ilep
{
	public static class Input
	{
		public static Action InputChanged;
		public enum ButtonName
		{
			Console,
			Up,
			Down,
			Left,
			Right,
			HandL,
			HandR,
			Jump
		}

		public static Dictionary<ButtonName, (KeyCode, KeyCode)> keyCodeDict;
		public static float MouseSensitivity = 1f;
		private static readonly (KeyCode, KeyCode)[] defaultKeyCodes = new (KeyCode, KeyCode)[]
		{
		(KeyCode.Backslash, KeyCode.None), // console
		(KeyCode.W,         KeyCode.None), // up/forward
		(KeyCode.S,         KeyCode.None), // down/back
		(KeyCode.A,         KeyCode.None), // left
		(KeyCode.D,         KeyCode.None), // right
		(KeyCode.Mouse0,    KeyCode.None), // left hand
		(KeyCode.Mouse1,    KeyCode.None), // right hand
		(KeyCode.Space,     KeyCode.None)  // jump
		};

		public static void SetDefaultKeys()
		{
			keyCodeDict = new Dictionary<ButtonName, (KeyCode, KeyCode)>();
			int buttonNameMax = Enum.GetValues(typeof(ButtonName)).Cast<int>().Max() + 1;
			for (int i = 0; i < buttonNameMax; ++i)
				keyCodeDict.Add((ButtonName)i, defaultKeyCodes[i]);
			InputChanged?.Invoke();
		}

		public static void SetKey(ButtonName button, KeyCode keyCode, bool altKey)
		{
			bool keyAlreadySet = false;
			ButtonName setButton = ButtonName.Console;
			bool setKeyIsAlt = false;
			foreach (var v in keyCodeDict)
			{
				if (v.Value.Item1 == keyCode)
				{
					setButton = v.Key;
					keyAlreadySet = true;
				}
				else if (v.Value.Item2 == keyCode)
				{
					setButton = v.Key;
					keyAlreadySet = true;
					setKeyIsAlt = true;
				}
			}
			if (keyAlreadySet)
			{
				if (setKeyIsAlt)
					keyCodeDict[setButton] = (keyCodeDict[setButton].Item1, KeyCode.None);
				else
					keyCodeDict[setButton] = (KeyCode.None, keyCodeDict[setButton].Item2);
			}
			if (altKey)
				keyCodeDict[button] = (keyCodeDict[button].Item1, keyCode);
			else
				keyCodeDict[button] = (keyCode, keyCodeDict[button].Item2);
			InputChanged?.Invoke();
		}

		/// <summary>
		/// True while button is held down
		/// </summary>
		public static bool GetButtonHeld(ButtonName buttonName)
		{
			return UnityEngine.Input.GetKey(keyCodeDict[buttonName].Item1) || UnityEngine.Input.GetKey(keyCodeDict[buttonName].Item2);
		}

		/// <summary>
		/// True on the first frame a button is pressed
		/// </summary>
		public static bool GetButtonDown(ButtonName buttonName)
		{
			return UnityEngine.Input.GetKeyDown(keyCodeDict[buttonName].Item1) || UnityEngine.Input.GetKeyDown(keyCodeDict[buttonName].Item2);
		}

		/// <summary>
		/// True on the first frame a button is released
		/// </summary>
		public static bool GetButtonUp(ButtonName buttonName)
		{
			return UnityEngine.Input.GetKeyUp(keyCodeDict[buttonName].Item1) || UnityEngine.Input.GetKeyUp(keyCodeDict[buttonName].Item2);
		}

		public static float GetHorizontal()
		{
			float axis = 0f;
			if (GetButtonHeld(ButtonName.Right))
				axis++;
			if (GetButtonHeld(ButtonName.Left))
				axis--;
			return axis;
		}

		public static float GetVertical()
		{
			float axis = 0f;
			if (GetButtonHeld(ButtonName.Up))
				axis++;
			if (GetButtonHeld(ButtonName.Down))
				axis--;
			return axis;
		}

		public static string[] ControlsToArray()
		{
			string[] arr = new string[keyCodeDict.Count * 3];
			int index = 0;
			foreach (var v in keyCodeDict)
			{
				arr[index++] = v.Key.ToString();
				arr[index++] = v.Value.Item1.ToString();
				arr[index++] = v.Value.Item2.ToString();
			}
			return arr;
		}

		public static void Initialize(string[] arr)
		{
			keyCodeDict = new Dictionary<ButtonName, (KeyCode, KeyCode)>();
			for (int i = 0; i < arr.Length; i += 3)
			{
				ButtonName inputEnum = (ButtonName)Enum.Parse(typeof(ButtonName), arr[i]);
				KeyCode defaultKey = (KeyCode)Enum.Parse(typeof(KeyCode), arr[i + 1]);
				KeyCode altKey = (KeyCode)Enum.Parse(typeof(KeyCode), arr[i + 2]);
				keyCodeDict.Add(inputEnum, (defaultKey, altKey));
			}
			InputChanged?.Invoke();
		}
	} 
}
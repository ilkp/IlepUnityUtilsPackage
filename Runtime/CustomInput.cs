using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class CustomInput
{
	public static Action InputChanged;

	public enum ControlMethod
	{
		Key,
		MouseScroll
	}

	public enum ControlName
	{
		Console,
		Up,
		Down,
		Left,
		Right,
		Jump,
		Fire1,
		Fire2,
		SwitchWeapon1,
		SwitchWeapon2
	}

	public class Control
	{
		public ControlMethod controlMethod;
		public KeyCode keyCode;
		public int mouseScrollDir;
		public bool EquivalentTo(Control other)
		{
			if (controlMethod != other.controlMethod)
				return false;
			if (controlMethod == ControlMethod.Key && keyCode != other.keyCode)
				return false;
			if (controlMethod == ControlMethod.MouseScroll && mouseScrollDir != other.mouseScrollDir)
				return false;
			return true;
		}

		public override string ToString()
		{
			return controlMethod.ToString() + " " + keyCode.ToString() + " " + mouseScrollDir.ToString();
		}

		public static Control FromString(string s)
		{
			Control control = new Control();
			string[] split = s.Split(' ');
			control.controlMethod = (ControlMethod)Enum.Parse(typeof(ControlMethod), split[0]);
			control.keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), split[1]);
			control.mouseScrollDir = int.Parse(split[2]);
			return control;
		}
	}

	public static float mouseSensitivity = 1f;
	public static Dictionary<ControlName, Control[]> controlDict;
	private static readonly (Control, Control)[] defaultControls = new (Control, Control)[]
	{
		 // console
		(new Control    { controlMethod = ControlMethod.Key, keyCode = KeyCode.Backslash },
		new Control     { controlMethod = ControlMethod.Key, keyCode = KeyCode.None }),
		// up/forward
		(new Control    { controlMethod = ControlMethod.Key, keyCode = KeyCode.W },
		new Control     { controlMethod = ControlMethod.Key, keyCode = KeyCode.None }),
		// down/back
		(new Control    { controlMethod = ControlMethod.Key, keyCode = KeyCode.S },
		new Control     { controlMethod = ControlMethod.Key, keyCode = KeyCode.None }),
		// left
		(new Control    { controlMethod = ControlMethod.Key, keyCode = KeyCode.A },
		new Control     { controlMethod = ControlMethod.Key, keyCode = KeyCode.None }),
		// right
		(new Control    { controlMethod = ControlMethod.Key, keyCode = KeyCode.D },
		new Control     { controlMethod = ControlMethod.Key, keyCode = KeyCode.None }),
		// jump
		(new Control    { controlMethod = ControlMethod.Key, keyCode = KeyCode.Space },
		new Control     { controlMethod = ControlMethod.Key, keyCode = KeyCode.None }),
		// fire1
		(new Control    { controlMethod = ControlMethod.Key, keyCode = KeyCode.Mouse0 },
		new Control     { controlMethod = ControlMethod.Key, keyCode = KeyCode.None }),
		// fire2
		(new Control    { controlMethod = ControlMethod.Key, keyCode = KeyCode.Mouse1 },
		new Control     { controlMethod = ControlMethod.Key, keyCode = KeyCode.None }),
		// switch weapon 1
		(new Control    { controlMethod = ControlMethod.MouseScroll, mouseScrollDir = 1 },
		new Control     { controlMethod = ControlMethod.Key, keyCode = KeyCode.None }),
		// switch weapon 2
		(new Control    { controlMethod = ControlMethod.MouseScroll, mouseScrollDir = -1 },
		new Control     { controlMethod = ControlMethod.Key, keyCode = KeyCode.None }),
	};

	/// <summary>
	/// True while button is held down
	/// </summary>
	public static bool GetButtonHeld(ControlName controlName)
	{
		for (int i = 0; i < 2; ++i)
		{
			switch (controlDict[controlName][i].controlMethod)
			{
				case ControlMethod.Key:
					if (Input.GetKey(controlDict[controlName][i].keyCode))
						return true;
					break;
				case ControlMethod.MouseScroll:
					// not defined
					break;
			}
		}
		return false;
	}

	/// <summary>
	/// True on the first frame a button is pressed
	/// </summary>
	public static bool GetButtonDown(ControlName controlName)
	{
		for (int i = 0; i < 2; ++i)
		{
			switch (controlDict[controlName][i].controlMethod)
			{
				case ControlMethod.Key:
					if (Input.GetKeyDown(controlDict[controlName][i].keyCode))
						return true;
					break;
				case ControlMethod.MouseScroll:
					if (Input.mouseScrollDelta.y == controlDict[controlName][i].mouseScrollDir)
						return true;
					break;
			}
		}
		return false;
	}

	/// <summary>
	/// True on the first frame a button is released
	/// </summary>
	public static bool GetButtonUp(ControlName controlName)
	{
		for (int i = 0; i < 2; ++i)
		{
			switch (controlDict[controlName][i].controlMethod)
			{
				case ControlMethod.Key:
					if (Input.GetKeyDown(controlDict[controlName][i].keyCode))
						return true;
					break;
				case ControlMethod.MouseScroll:
					// not defined
					break;
			}
		}
		return false;
	}

	public static float GetHorizontal()
	{
		float axis = 0f;
		if (GetButtonHeld(ControlName.Right))
			axis++;
		if (GetButtonHeld(ControlName.Left))
			axis--;
		return axis;
	}

	public static float GetVertical()
	{
		float axis = 0f;
		if (GetButtonHeld(ControlName.Up))
			axis++;
		if (GetButtonHeld(ControlName.Down))
			axis--;
		return axis;
	}

	public static void SetDefaultKeys()
	{
		controlDict = new Dictionary<ControlName, Control[]>();
		int buttonNameMax = Enum.GetValues(typeof(ControlName)).Cast<int>().Max() + 1;
		for (int i = 0; i < buttonNameMax; ++i)
			controlDict.Add((ControlName)i, new Control[] { defaultControls[i].Item1, defaultControls[i].Item2 });
		InputChanged?.Invoke();
	}

	public static void SetControl(ControlName button, Control control, bool isAlt)
	{
		// Find if the control is already in use somewhere else
		foreach (KeyValuePair<ControlName, Control[]> entry in controlDict)
		{
			for (int i = 0; i < 2; ++i)
			{
				if (entry.Value[i].EquivalentTo(control))
				{
					entry.Value[i].controlMethod = ControlMethod.Key;
					entry.Value[i].keyCode = KeyCode.None;
				}
			}
		}
		if (isAlt)
		{
			controlDict[button][1].controlMethod = control.controlMethod;
			controlDict[button][1].keyCode = control.keyCode;
			controlDict[button][1].mouseScrollDir = control.mouseScrollDir;
		}
		else
		{
			controlDict[button][0].controlMethod = control.controlMethod;
			controlDict[button][0].keyCode = control.keyCode;
			controlDict[button][0].mouseScrollDir = control.mouseScrollDir;
		}
		InputChanged?.Invoke();
	}

	public static string[] ControlsToArray()
	{
		string[] arr = new string[controlDict.Count * 3];
		int index = 0;
		foreach (var v in controlDict)
		{
			arr[index++] = v.Key.ToString();
			arr[index++] = v.Value[0].ToString();
			arr[index++] = v.Value[1].ToString();
		}
		return arr;
	}

	public static void InitControls(string[] arr)
	{
		controlDict = new Dictionary<ControlName, Control[]>();
		for (int i = 0; i < arr.Length; i += 3)
		{
			ControlName inputEnum = (ControlName)Enum.Parse(typeof(ControlName), arr[i]);
			Control primary = Control.FromString(arr[i + 1]);
			Control alt = Control.FromString(arr[i + 2]);
			controlDict.Add(inputEnum, new Control[] { primary, alt });
		}
		InputChanged?.Invoke();
	}
}
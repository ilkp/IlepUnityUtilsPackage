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

	public static int[] horizontal;
	public static int[] vertical;
	public static float mouseSensitivity = 1f;
	public static Dictionary<int, Control[]> controlDict;

	/// <summary>
	/// True while button is held down
	/// </summary>
	public static bool GetButtonHeld(int control)
	{
		for (int i = 0; i < 2; ++i)
		{
			switch (controlDict[control][i].controlMethod)
			{
				case ControlMethod.Key:
					if (Input.GetKey(controlDict[control][i].keyCode))
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
	public static bool GetButtonDown(int control)
	{
		for (int i = 0; i < 2; ++i)
		{
			switch (controlDict[control][i].controlMethod)
			{
				case ControlMethod.Key:
					if (Input.GetKeyDown(controlDict[control][i].keyCode))
						return true;
					break;
				case ControlMethod.MouseScroll:
					if (Input.mouseScrollDelta.y == controlDict[control][i].mouseScrollDir)
						return true;
					break;
			}
		}
		return false;
	}

	/// <summary>
	/// True on the first frame a button is released
	/// </summary>
	public static bool GetButtonUp(int control)
	{
		for (int i = 0; i < 2; ++i)
		{
			switch (controlDict[control][i].controlMethod)
			{
				case ControlMethod.Key:
					if (Input.GetKeyDown(controlDict[control][i].keyCode))
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
		if (GetButtonHeld(horizontal[0]))
			axis--;
		if (GetButtonHeld(horizontal[1]))
			axis++;
		return axis;
	}

	public static float GetVertical()
	{
		float axis = 0f;
		if (GetButtonHeld(vertical[0]))
			axis--;
		if (GetButtonHeld(vertical[1]))
			axis++;
		return axis;
	}

	public static void SetControls(Dictionary<int, (Control, Control)> controls)
	{
		controlDict = new Dictionary<int, Control[]>();
		foreach (KeyValuePair<int, (Control, Control)> entry in controls)
			controlDict.Add(entry.Key, new Control[] { entry.Value.Item1, entry.Value.Item2 });
		InputChanged?.Invoke();
	}

	public static void SetControl(int controlNumber, Control control, bool isAlt)
	{
		// Find if the control is already in use somewhere else
		foreach (KeyValuePair<int, Control[]> entry in controlDict)
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
			controlDict[controlNumber][1].controlMethod = control.controlMethod;
			controlDict[controlNumber][1].keyCode = control.keyCode;
			controlDict[controlNumber][1].mouseScrollDir = control.mouseScrollDir;
		}
		else
		{
			controlDict[controlNumber][0].controlMethod = control.controlMethod;
			controlDict[controlNumber][0].keyCode = control.keyCode;
			controlDict[controlNumber][0].mouseScrollDir = control.mouseScrollDir;
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
		controlDict = new Dictionary<int, Control[]>();
		for (int i = 0; i < arr.Length; i += 3)
		{
			int index = int.Parse(arr[i]);
			Control primary = Control.FromString(arr[i + 1]);
			Control alt = Control.FromString(arr[i + 2]);
			controlDict.Add(index, new Control[] { primary, alt });
		}
		InputChanged?.Invoke();
	}
}
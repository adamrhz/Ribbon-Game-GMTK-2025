using System;

using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Ribbon {
	public static class GameInput{
		public static float GetAxis(string axisName) => InputManager.Instance.GetAxis(axisName);
		public static Vector2 GetAxis2D(string axisName) => InputManager.Instance.GetAxis2D(axisName);

		public static bool GetButton(string buttonName) => InputManager.Instance.GetButton(buttonName);
		public static bool GetButtonDown(string buttonName) => InputManager.Instance.GetButtonDown(buttonName);
		public static bool GetButtonUp(string buttonName) => InputManager.Instance.GetButtonUp(buttonName);
	
		//Local Functions

		public static float GetAxis(this InputManager Input, string axisName) => Input.GetAxis(axisName);
		public static Vector2 GetAxis2D(this InputManager Input,string axisName) => Input.GetAxis2D(axisName);

		public static bool GetButton(this InputManager Input,string buttonName) => Input.GetButton(buttonName);
		public static bool GetButtonDown(this InputManager Input,string buttonName) => Input.GetButtonDown(buttonName);
		public static bool GetButtonUp(this InputManager Input,string buttonName) => Input.GetButtonUp(buttonName);

		//-----------------------------------------------------------------------------------------//
		public static void BlockInput() => InputManager.Instance.BlockInput = true;
		public static void UnblockInput() => InputManager.Instance.BlockInput = false;

		public static PlayerInput InputObject;
	}

	public enum InputDeviceType {
		Keyboard, Mouse, Dualsense, Xbox
	}

	public class InputManager : MonoBehaviour
	{
		public PlayerInput MainInput;

		public List<UButton> Buttons;
		public List<UAxis1D> Axis1D;
		public List<UAxis2D> Axis2D;
		
		[CanBeNull] public static InputManager Instance;
	
		public bool BlockInput;
		
		public string CurrentMap;

	
		public void ChangeMap(string MapNameOrID){
			MainInput.SwitchCurrentActionMap(MapNameOrID);
			CurrentMap = MapNameOrID;
		}
		private void Awake()
		{
			MainInput.onActionTriggered += OnActionTriggered;
			CurrentMap = MainInput.defaultActionMap;
			Buttons = new List<UButton>(); Axis1D = new List<UAxis1D>(); Axis2D = new List<UAxis2D>();
			foreach(InputActionMap a in MainInput.actions.actionMaps){
				foreach(InputAction s in a.actions){
					switch(s.type){
					case InputActionType.Button:
						UButton button = new UButton();
						button.button = s;
						button.Name = s.name;
						button.MapName = a.name;
						Buttons.Add(button);
						break;
	    			
					case InputActionType.Value:
						switch(s.expectedControlType){
						case "Axis":
							UAxis1D axis1 = new UAxis1D();
							axis1.Name = s.name;
							axis1.MapName = a.name;
							Axis1D.Add(axis1);
							break;
						case "Vector2":
							UAxis2D axis2 = new UAxis2D();
							axis2.Name = s.name;
							axis2.MapName = a.name;
							Axis2D.Add(axis2);
							break;
						}
						break;
					}
				}
			}
		}
    
		internal UButton UBGet(string name, string map) => Buttons.Find(s=>s.Name == name && s.MapName == map);
		internal string UBGetMap(string name) => Buttons.Find(s=>s.Name == name).MapName;
		internal UAxis1D UA1Get(string name, string map) => Axis1D.Find(s=>s.Name == name && s.MapName == map);
		internal string UA1GetMap(string name) => Axis1D.Find(s=>s.Name == name).MapName;
		internal UAxis2D UA2Get(string name, string map) => Axis2D.Find(s=>s.Name == name && s.MapName == map);
		internal string UA2GetMap(string name) => Axis2D.Find(s=>s.Name == name).MapName;
		
		private void Update(){
			//CommonMaps[0] = Settings.Instance.currentSettings.inputSettings.PreferredScheme == 0 ? "Player" : "Player Alt";		
			if (!Instance) 
				Instance = this;
		}
	
		private void OnActionTriggered(InputAction.CallbackContext callbackContext)
		{
			if (CurrentMap != callbackContext.action.actionMap.name) return;
			
			switch (callbackContext.action.type)
			{
				case InputActionType.Button:
					try
					{ 
						UBGet(callbackContext.action.name, callbackContext.action.actionMap.name).hold = callbackContext.ReadValueAsButton();
					}
					catch
					{
						//do absolutely nothing because I don't want to log an error out even though this works so IDK why its throwing errors to begin with
					}
					break;
				case InputActionType.Value:
					switch(callbackContext.action.expectedControlType){
					case "Axis": 
						UA1Get(callbackContext.action.name, callbackContext.action.actionMap.name).Value = callbackContext.ReadValue<float>();
						break;
					case "Vector2": 
						UA2Get(callbackContext.action.name, callbackContext.action.actionMap.name).Value = callbackContext.ReadValue<Vector2>();
						break;
					}
					break;
			}
		}

		public bool GetButtonDown(string name)
		{
			try
			{
				if (!BlockInput)
					return UBGet(name, CurrentMap).pressed;
				else return false;
			}
			catch
			{
				return false;
			}
		}

		public bool GetButtonUp(string name)
		{
			try
			{
				if (!BlockInput)
					return UBGet(name, CurrentMap).released;
				else return false;
			}
			catch
			{
				return false;
			}
		}

		public bool GetButton(string name)
		{
			try
			{
				if (!BlockInput)
					return UBGet(name, CurrentMap).hold;
				else return false;
			}
			catch
			{
				return false;
			}
		}

		public float GetAxis(string name)
		{
			try
			{
				if (!BlockInput)
					return UA1Get(name, CurrentMap).Value;
				else return 0;
			}
			catch
			{
				return 0;
			}
		}

		public Vector2 GetAxis2D(string name)
		{
			try
			{
				if (!BlockInput)
					return UA2Get(name, CurrentMap).Value;
				else return Vector2.zero;
			}
			catch
			{
				return Vector2.zero;
			}
		}
	}

	public class UAxis1D{
		public string Name;
		public string MapName;
		public float Value;
	}
	[System.Serializable]
	public class UAxis2D{
		public string Name;
		public string MapName;
		public Vector2 Value;
	}
	[System.Serializable]
	public class UButton
	{
		public string Name; public string MapName; //Define a public name that we can index later with helper functions
		public InputAction button;
		public Action press, release;

		public float PressedTime, FPressedTime, ReleasedTime, FReleasedTime;
		[HideInInspector] public bool hold
		{
			get => _hold;
			set{
				if (_hold == value) return;
				_hold = value;
				if (_hold){
					PressedTime = Time.unscaledTime;
					FPressedTime = Time.fixedUnscaledTime;
				}
				else{
					ReleasedTime = Time.unscaledTime;
					FReleasedTime = Time.fixedUnscaledTime;
				}
			}
		}
		public bool upressed;
	
		public bool pressed => !Time.inFixedTimeStep ? PressedTime == Time.unscaledTime : FPressedTime == Time.fixedUnscaledTime;
		public bool released =>!Time.inFixedTimeStep ? ReleasedTime == Time.unscaledTime : FReleasedTime == Time.fixedUnscaledTime;

		bool _hold;
	}
}

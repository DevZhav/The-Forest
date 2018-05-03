﻿using System;
using System.Collections.Generic;
using System.Linq;
using Rewired;
using UnityEngine;

namespace TheForest.Utils
{
	
	public class InputMappingButtonManager : MonoBehaviour
	{
		
		private void Awake()
		{
			InputMappingButtonManager._instance = this;
		}

		
		private void OnEnable()
		{
			if (this._mappingManager == null)
			{
				this._mappingManager = UnityEngine.Object.FindObjectOfType<InputMapping>();
			}
			this._mappingManager.LoadAllMaps();
			Input.player.controllers.maps.SetMapsEnabled(false, ControllerType.Joystick, "Default");
			Input.player.controllers.maps.SetMapsEnabled(false, ControllerType.Keyboard, "Default");
			if (this._mappingManager.IsNull())
			{
				this._mappingManager = UnityEngine.Object.FindObjectOfType<InputMapping>();
			}
			this._buttons = this._buttonSearchRoot.GetComponentsInChildren<InputMappingButton>().ToList<InputMappingButton>();
		}

		
		private void OnDisable()
		{
			if (this._buttons.SafeCount<InputMappingButton>() > 0)
			{
				foreach (InputMappingButton inputMappingButton in InputMappingButtonManager._instance._buttons)
				{
					inputMappingButton._button.enabled = true;
				}
			}
			if (base.enabled)
			{
				this._mappingManager.LoadAllMaps();
			}
			Input.player.controllers.maps.SetMapsEnabled(true, ControllerType.Joystick, "Default");
			Input.player.controllers.maps.SetMapsEnabled(true, ControllerType.Keyboard, "Default");
		}

		
		public void Save()
		{
			Input.player.controllers.maps.SetMapsEnabled(true, ControllerType.Joystick, "Default");
			Input.player.controllers.maps.SetMapsEnabled(true, ControllerType.Keyboard, "Default");
			this._mappingManager.SaveAllMaps();
			Input.player.controllers.maps.SetMapsEnabled(false, ControllerType.Joystick, "Default");
			Input.player.controllers.maps.SetMapsEnabled(false, ControllerType.Keyboard, "Default");
		}

		
		public void RestoreDefaults()
		{
			this._mappingManager.ClearAllMaps();
			Input input = UnityEngine.Object.FindObjectOfType<Input>();
			if (input)
			{
				UnityEngine.Object.DestroyImmediate(input.gameObject);
			}
			UnityEngine.Object.FindObjectOfType<RewiredSpawner>().SendMessage("Awake");
			this._mappingManager = UnityEngine.Object.FindObjectOfType<InputMapping>();
			this._mappingManager.LoadAllMaps();
			this.Save();
			this.RefreshButtons();
		}

		
		private void RefreshButtons()
		{
			if (this._buttons.SafeCount<InputMappingButton>() == 0)
			{
				return;
			}
			foreach (InputMappingButton inputMappingButton in InputMappingButtonManager._instance._buttons)
			{
				inputMappingButton.Refresh();
			}
		}

		
		public static void PollingStarted(InputMappingButton inputMappingButton)
		{
			if (InputMappingButtonManager._instance == null || InputMappingButtonManager._instance._buttons.SafeCount<InputMappingButton>() == 0)
			{
				return;
			}
			foreach (InputMappingButton inputMappingButton2 in InputMappingButtonManager._instance._buttons)
			{
				inputMappingButton2._button.enabled = false;
			}
		}

		
		public static void PollingStopped(InputMappingButton inputMappingButton)
		{
			if (InputMappingButtonManager._instance == null || InputMappingButtonManager._instance._buttons.SafeCount<InputMappingButton>() == 0)
			{
				return;
			}
			foreach (InputMappingButton inputMappingButton2 in InputMappingButtonManager._instance._buttons)
			{
				inputMappingButton2._button.enabled = true;
			}
		}

		
		public InputMapping _mappingManager;

		
		public GameObject _buttonSearchRoot;

		
		public List<InputMappingButton> _buttons;

		
		private static InputMappingButtonManager _instance;
	}
}

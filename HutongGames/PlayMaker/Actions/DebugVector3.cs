﻿using System;

namespace HutongGames.PlayMaker.Actions
{
	
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Logs the value of a Vector3 Variable in the PlayMaker Log Window.")]
	public class DebugVector3 : FsmStateAction
	{
		
		public override void Reset()
		{
			this.logLevel = LogLevel.Info;
			this.vector3Variable = null;
		}

		
		public override void OnEnter()
		{
			string text = "None";
			if (!this.vector3Variable.IsNone)
			{
				text = this.vector3Variable.Name + ": " + this.vector3Variable.Value;
			}
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, text);
			base.Finish();
		}

		
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		
		[UIHint(UIHint.Variable)]
		[Tooltip("Prints the value of a Vector3 variable in the PlayMaker log window.")]
		public FsmVector3 vector3Variable;
	}
}

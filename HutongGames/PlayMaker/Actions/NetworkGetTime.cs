﻿using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	
	[ActionCategory(ActionCategory.Network)]
	[Tooltip("Get the current network time (seconds).")]
	public class NetworkGetTime : FsmStateAction
	{
		
		public override void Reset()
		{
			this.time = null;
		}

		
		public override void OnEnter()
		{
			this.time.Value = (float)Network.time;
			base.Finish();
		}

		
		[Tooltip("The network time.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat time;
	}
}

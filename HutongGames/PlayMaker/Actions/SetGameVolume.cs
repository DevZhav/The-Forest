﻿using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	
	[Tooltip("Sets the global sound volume.")]
	[ActionCategory(ActionCategory.Audio)]
	public class SetGameVolume : FsmStateAction
	{
		
		public override void Reset()
		{
			this.volume = 1f;
			this.everyFrame = false;
		}

		
		public override void OnEnter()
		{
			AudioListener.volume = this.volume.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		
		public override void OnUpdate()
		{
			AudioListener.volume = this.volume.Value;
		}

		
		[HasFloatSlider(0f, 1f)]
		[RequiredField]
		public FsmFloat volume;

		
		public bool everyFrame;
	}
}

﻿using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Get the value of a Game Object Variable from another FSM.")]
	public class GetFsmGameObject : FsmStateAction
	{
		
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = string.Empty;
			this.storeValue = null;
		}

		
		public override void OnEnter()
		{
			this.DoGetFsmGameObject();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		
		public override void OnUpdate()
		{
			this.DoGetFsmGameObject();
		}

		
		private void DoGetFsmGameObject()
		{
			if (this.storeValue == null)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != this.goLastFrame)
			{
				this.goLastFrame = ownerDefaultTarget;
				this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
			}
			if (this.fsm == null)
			{
				return;
			}
			FsmGameObject fsmGameObject = this.fsm.FsmVariables.GetFsmGameObject(this.variableName.Value);
			if (fsmGameObject == null)
			{
				return;
			}
			this.storeValue.Value = fsmGameObject.Value;
		}

		
		[RequiredField]
		public FsmOwnerDefault gameObject;

		
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		
		[RequiredField]
		[UIHint(UIHint.FsmGameObject)]
		public FsmString variableName;

		
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeValue;

		
		public bool everyFrame;

		
		private GameObject goLastFrame;

		
		private PlayMakerFSM fsm;
	}
}

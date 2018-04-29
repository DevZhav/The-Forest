﻿using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	
	[Tooltip("Gets the number of vertices in a GameObject's mesh. Useful in conjunction with GetVertexPosition.")]
	[ActionCategory("Mesh")]
	public class GetVertexCount : FsmStateAction
	{
		
		public override void Reset()
		{
			this.gameObject = null;
			this.storeCount = null;
			this.everyFrame = false;
		}

		
		public override void OnEnter()
		{
			this.DoGetVertexCount();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		
		public override void OnUpdate()
		{
			this.DoGetVertexCount();
		}

		
		private void DoGetVertexCount()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				MeshFilter component = ownerDefaultTarget.GetComponent<MeshFilter>();
				if (component == null)
				{
					this.LogError("Missing MeshFilter!");
					return;
				}
				this.storeCount.Value = component.mesh.vertexCount;
			}
		}

		
		[Tooltip("The GameObject to check.")]
		[CheckForComponent(typeof(MeshFilter))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the vertex count in a variable.")]
		public FsmInt storeCount;

		
		public bool everyFrame;
	}
}

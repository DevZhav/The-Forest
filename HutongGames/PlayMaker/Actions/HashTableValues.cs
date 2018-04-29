﻿using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	
	[Tooltip("Store all the values of a PlayMaker HashTable Proxy component (PlayMakerHashTableProxy) into a PlayMaker arrayList Proxy component (PlayMakerArrayListProxy).")]
	[ActionCategory("ArrayMaker/HashTable")]
	public class HashTableValues : HashTableActions
	{
		
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.arrayListGameObject = null;
			this.arrayListReference = null;
		}

		
		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.doHashTableValues();
			}
			base.Finish();
		}

		
		public void doHashTableValues()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.arrayListGameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			PlayMakerArrayListProxy arrayListProxyPointer = base.GetArrayListProxyPointer(ownerDefaultTarget, this.arrayListReference.Value, false);
			if (arrayListProxyPointer != null)
			{
				arrayListProxyPointer.arrayList.AddRange(this.proxy.hashTable.Values);
			}
		}

		
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		[ActionSection("Set up")]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component that will store the values")]
		[ActionSection("Result")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault arrayListGameObject;

		
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component that will store the values ( necessary if several component coexists on the same GameObject")]
		public FsmString arrayListReference;
	}
}

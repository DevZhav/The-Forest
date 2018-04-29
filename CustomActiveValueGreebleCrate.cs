﻿using System;
using System.Collections;
using UnityEngine;


public class CustomActiveValueGreebleCrate : CustomActiveValueGreeble
{
	
	private void Awake()
	{
		this.LidDefaultRotation = this._lid.transform.localRotation;
	}

	
	private void OnDisable()
	{
		this.UpdateGreebleData();
		this._lid.transform.localRotation = this.LidDefaultRotation;
		this._interior.SetActive(false);
		this._trigger.SetActive(true);
	}

	
	private void OnEnable()
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
		base.StartCoroutine(this.OnEnableRoutine());
	}

	
	private IEnumerator OnEnableRoutine()
	{
		yield return null;
		if (base.Data != null)
		{
			ISuitcaseState iss = null;
			if (BoltNetwork.isRunning)
			{
				BoltEntity be = base.GetComponent<BoltEntity>();
				if (be.isAttached && be.isOwner)
				{
					iss = be.GetState<ISuitcaseState>();
				}
			}
			byte b = base.Data._instancesState[base.Index];
			if (b != 0)
			{
				if (b == 1)
				{
					this._lid.Play();
					this._interior.SetActive(true);
					this._trigger.SetActive(false);
					if (iss != null)
					{
						iss.Open = true;
					}
					goto IL_2D7;
				}
				if (b != 252)
				{
					this._lid.Play();
					this._interior.SetActive(true);
					this._trigger.SetActive(false);
					int state = base.Data._instancesState[base.Index] >> 1;
					int mpstate = 0;
					for (int i = 0; i < this._pickups.Length; i++)
					{
						bool isActive = (state & 1 << i) == 0;
						if (this._pickups[i] && isActive != this._pickups[i].activeSelf)
						{
							this._pickups[i].SetActive(isActive);
							if (iss != null)
							{
								mpstate |= 1 << this._pickups[i].transform.GetSiblingIndex();
							}
						}
					}
					if (iss != null)
					{
						iss.Open = true;
						iss.FlaresPickedUp = mpstate;
					}
					goto IL_2D7;
				}
			}
			this._lid.transform.localRotation = this.LidDefaultRotation;
			this._interior.SetActive(false);
			this._trigger.SetActive(true);
			IL_2D7:;
		}
		else
		{
			this._lid.transform.localRotation = this.LidDefaultRotation;
			this._interior.SetActive(false);
			this._trigger.SetActive(true);
		}
		yield break;
	}

	
	public override void UpdateGreebleData()
	{
		if (base.Data != null)
		{
			if (!this._interior.activeSelf)
			{
				if (this._trigger.activeSelf)
				{
					base.Data._instancesState[base.Index] = 0;
				}
				else
				{
					base.Data._instancesState[base.Index] = 2;
				}
			}
			else
			{
				byte b = 0;
				for (int i = 0; i < this._pickups.Length; i++)
				{
					GameObject gameObject = this._pickups[i];
					if (!gameObject)
					{
						b |= (byte)(1 << i);
					}
					else if (!gameObject.activeSelf)
					{
						b |= (byte)(1 << i);
						gameObject.SetActive(true);
					}
				}
				base.Data._instancesState[base.Index] = (byte)(((int)b << 1) + 1);
				base.Data = null;
			}
		}
	}

	
	public GameObject _interior;

	
	public GameObject _trigger;

	
	public GameObject[] _pickups;

	
	public Animation _lid;

	
	private Quaternion LidDefaultRotation;
}

﻿using System;
using TheForest.Utils;
using UnityEngine;


public class limitBlurDistance : MonoBehaviour
{
	
	private void Start()
	{
		this.thisTr = base.transform;
		float value = UnityEngine.Random.value;
		if (!base.IsInvoking("checkPlayerDist"))
		{
			base.InvokeRepeating("checkPlayerDist", value, 0.2f);
		}
		if (this.useParentAsSource)
		{
			this.renderers = base.transform.parent.GetComponentsInChildren<Renderer>(true);
		}
		else
		{
			this.renderers = base.transform.GetComponentsInChildren<Renderer>(true);
		}
		this.rendererDist = 25600f;
		this.enableAnimation();
		this.enableRenderers();
	}

	
	private void checkPlayerDist()
	{
		if (LocalPlayer.Transform != null)
		{
			this.playerDist = (this.thisTr.position - LocalPlayer.Transform.position).sqrMagnitude;
		}
		if (this.hideRenderers && this.playerDist > this.rendererDist)
		{
			this.disableRenderers();
		}
		else if (this.hideRenderers && this.playerDist < this.rendererDist)
		{
			this.enableRenderers();
		}
		if (this.hideJoints && this.playerDist > this.rendererDist)
		{
			this.reduceAnimation();
		}
		else if (this.hideJoints && this.playerDist < this.rendererDist)
		{
			this.enableAnimation();
		}
	}

	
	private void enableRenderers()
	{
		if (this.doRenderers)
		{
			for (int i = 0; i < this.renderers.Length; i++)
			{
				if (this.renderers[i])
				{
					this.renderers[i].enabled = true;
				}
			}
		}
		this.doRenderers = false;
	}

	
	private void disableRenderers()
	{
		if (!this.doRenderers)
		{
			for (int i = 0; i < this.renderers.Length; i++)
			{
				if (this.renderers[i])
				{
					this.renderers[i].enabled = false;
				}
			}
		}
		this.doRenderers = true;
	}

	
	private void reduceAnimation()
	{
		if (this.doJoints)
		{
			for (int i = 0; i < this.joints.Length; i++)
			{
				this.joints[i].SetActive(false);
			}
		}
		this.doJoints = false;
	}

	
	private void enableAnimation()
	{
		if (!this.doJoints)
		{
			for (int i = 0; i < this.joints.Length; i++)
			{
				this.joints[i].SetActive(true);
			}
		}
		this.doJoints = true;
	}

	
	public GameObject[] joints;

	
	public Renderer[] renderers;

	
	private bool doAmplify;

	
	private Transform thisTr;

	
	private float playerDist;

	
	public bool useParentAsSource;

	
	public bool hideJoints;

	
	public bool hideRenderers;

	
	private bool doJoints;

	
	private bool doRenderers;

	
	private float rendererDist;
}

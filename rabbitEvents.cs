﻿using System;
using PathologicalGames;
using TheForest.Utils;
using UnityEngine;


public class rabbitEvents : MonoBehaviour
{
	
	private void Awake()
	{
		this.tr = base.transform;
		this.ai = base.transform.parent.GetComponent<animalAI>();
		this.cav = base.transform.GetComponent<coopAnimatorVis>();
		this.ca = base.transform.parent.GetComponent<CoopAnimal>();
		this.animator = base.transform.GetComponent<Animator>();
	}

	
	private void Start()
	{
		this.snowStartHeight = Terrain.activeTerrain.materialTemplate.GetFloat("_SnowStartHeight");
	}

	
	private void Update()
	{
		if (this.net)
		{
			if (this.animator.GetBool("trapped"))
			{
				this.pickupTrigger.SetActive(true);
			}
			else
			{
				this.pickupTrigger.SetActive(false);
			}
		}
	}

	
	private void spawnDust()
	{
		if (BoltNetwork.isClient && !this.ca.isSnow)
		{
			return;
		}
		if (this.tr.position.y < this.snowStartHeight || !this.ca.isSnow || !this.IsInNorthColdArea())
		{
			return;
		}
		float num;
		if (this.net)
		{
			num = this.cav.playerDist;
		}
		else
		{
			num = this.ai.fsmPlayerDist.Value;
		}
		if (num < 35f)
		{
			PoolManager.Pools["Particles"].Spawn(this.snowParticle.transform, this.tr.position, this.tr.rotation);
		}
	}

	
	public bool IsInNorthColdArea()
	{
		return !(Scene.WeatherSystem == null) && ((!Scene.WeatherSystem.UsingSnow) ? (Mathf.Floor(base.transform.position.y) > 160f && Mathf.Floor(base.transform.position.z) < -300f) : (Mathf.Ceil(base.transform.position.y) > 160f && Mathf.Ceil(base.transform.position.z) < -300f));
	}

	
	private Transform tr;

	
	private animalAI ai;

	
	private coopAnimatorVis cav;

	
	private CoopAnimal ca;

	
	private Animator animator;

	
	public GameObject pickupTrigger;

	
	public GameObject snowParticle;

	
	public AudioClip[] hops;

	
	public bool net;

	
	private float snowStartHeight;
}

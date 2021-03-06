﻿using System;
using TheForest.Utils;
using UnityEngine;


[ExecuteInEditMode]
public class LOD_Manager : MonoBehaviour
{
	
	
	public static LOD_Manager Instance
	{
		get
		{
			if (!LOD_Manager.instance)
			{
				Debug.LogWarning("No LOD Manager Found, please add one. (From the Menu: GameObject/The Forest/LOD Manager)");
				LOD_Manager.AddLODManager();
			}
			return LOD_Manager.instance;
		}
	}

	
	private static void AddLODManager()
	{
		if (GameObject.Find("LOD Manager") == null)
		{
			GameObject gameObject = new GameObject("LOD Manager");
			gameObject.AddComponent<LOD_Manager>();
		}
		else
		{
			Debug.Log("LOD Manager already added. :)");
		}
	}

	
	private void Awake()
	{
		if (LOD_Manager.instance == null)
		{
			LOD_Manager.instance = this;
		}
		else if (LOD_Manager.instance != this)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
	}

	
	
	public float RangeMultiplier
	{
		get
		{
			return this.RangeMultiplierPerQuality[(int)TheForestQualitySettings.UserSettings.DrawDistance];
		}
	}

	
	
	public float RangeMultiplierSmall
	{
		get
		{
			return this.RangeMultiplierPerQualitySmall[(int)TheForestQualitySettings.UserSettings.DrawDistance];
		}
	}

	
	private void Update()
	{
		if (Application.isPlaying)
		{
			if (float.IsNaN(this.fps))
			{
				this.fps = 0f;
			}
			if (float.IsNaN(this.currentQuality))
			{
				this.currentQuality = 0f;
			}
			this.fps = Mathf.Lerp(this.fps, 1f / Time.smoothDeltaTime, 0.05f);
			float num = Mathf.Clamp01(this.fps / this.TargetFPS);
			float num2 = Mathf.Abs(num - this.currentQuality);
			if (num2 > 0.075f)
			{
				this.currentQuality = Mathf.Lerp(this.currentQuality, num, 0.01f);
			}
			this.currentQuality = Mathf.Max(0.33f, this.currentQuality);
		}
		float b = LocalPlayer.IsInOutsideWorld ? 1f : 0.25f;
		this.currentCaveQuality = Mathf.Lerp(this.currentCaveQuality, b, 0.02f);
		if (!this.FpsQualityScaling)
		{
			this.currentQuality = 1f;
			this.currentCaveQuality = 1f;
		}
		float num3 = this.currentQuality * this.RangeMultiplier;
		this.Bush.Update(num3 * this.currentCaveQuality);
		this.Plant.Update(num3 * this.currentCaveQuality);
		this.Rocks.Update(num3 * this.currentCaveQuality);
		this.Trees.Update(num3 * this.currentCaveQuality);
		this.Cave.Update(1f);
		this.CaveEntrance.Update(num3);
		float num4 = this.currentQuality * this.RangeMultiplierSmall;
		this.PickUps.Update(num4);
		this.SmallRocks.Update(num4 * this.currentCaveQuality);
		this.SmallBush.Update(num4 * this.currentCaveQuality);
		this.MediumCave.Update(num4 * this.currentCaveQuality);
		this.SmallCave.Update(num4 * this.currentCaveQuality);
	}

	
	private static LOD_Manager instance;

	
	public bool FpsQualityScaling;

	
	[Range(30f, 120f)]
	public float TargetFPS = 30f;

	
	[Range(0.01f, 5.5f)]
	public float Padding = 1f;

	
	[NameFromEnumIndex(typeof(TheForestQualitySettings.DrawDistances))]
	public float[] RangeMultiplierPerQuality = new float[6];

	
	[NameFromEnumIndex(typeof(TheForestQualitySettings.DrawDistances))]
	public float[] RangeMultiplierPerQualitySmall = new float[6];

	
	public LOD_Settings Bush = new LOD_Settings(new float[]
	{
		20f,
		40f
	});

	
	public LOD_Settings Cave = new LOD_Settings(new float[]
	{
		30f,
		100f,
		150f
	});

	
	public LOD_Settings CaveEntrance = new LOD_Settings(new float[]
	{
		30f,
		100f,
		150f
	});

	
	public LOD_Settings Plant = new LOD_Settings(new float[]
	{
		20f,
		80f
	});

	
	public LOD_Settings Rocks = new LOD_Settings(new float[]
	{
		30f,
		100f,
		150f
	});

	
	public LOD_Settings Trees = new LOD_Settings(new float[]
	{
		10f,
		150f,
		3000f
	});

	
	public LOD_Settings MediumCave = new LOD_Settings(new float[]
	{
		30f,
		100f,
		150f
	});

	
	public LOD_Settings SmallCave = new LOD_Settings(new float[]
	{
		30f,
		100f,
		150f
	});

	
	public LOD_Settings SmallBush = new LOD_Settings(new float[]
	{
		20f,
		40f
	});

	
	public LOD_Settings SmallRocks = new LOD_Settings(new float[]
	{
		30f,
		100f,
		150f
	});

	
	public LOD_Settings PickUps = new LOD_Settings(new float[]
	{
		20f,
		80f
	});

	
	private float fps = 60f;

	
	private float currentQuality = 1f;

	
	private float currentCaveQuality = 1f;

	
	public static float TreeOcclusionBonusRatio = 1f;

	
	[Serializable]
	public class QualityRangeData
	{
		
		public float RangeMultiplier = 1f;

		
		public float TreeOcclusionBonusMultiplier;
	}
}

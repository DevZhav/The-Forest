﻿using System;
using Bolt;
using TheForest.Buildings.Creation;
using UdpKit;
using UnityEngine;


internal class CoopFloorToken : IProtocolToken
{
	
	void IProtocolToken.Write(UdpPacket packet)
	{
		packet.WriteBoltEntity(this.Parent);
		if (packet.WriteBool(this.Holes != null))
		{
			int num = Mathf.Min(this.Holes.Length, 20);
			packet.WriteInt(num);
			for (int i = 0; i < num; i++)
			{
				packet.WriteVector3(this.Holes[i]._position);
				packet.WriteFloat(this.Holes[i]._yRotation);
				packet.WriteVector2(this.Holes[i]._size);
			}
		}
	}

	
	void IProtocolToken.Read(UdpPacket packet)
	{
		this.Parent = packet.ReadBoltEntity();
		if (packet.ReadBool())
		{
			this.Holes = new Hole[packet.ReadInt()];
			for (int i = 0; i < this.Holes.Length; i++)
			{
				this.Holes[i] = new Hole();
				this.Holes[i]._position = packet.ReadVector3();
				this.Holes[i]._yRotation = packet.ReadFloat();
				this.Holes[i]._size = packet.ReadVector2();
			}
		}
	}

	
	public BoltEntity Parent;

	
	public Hole[] Holes;
}

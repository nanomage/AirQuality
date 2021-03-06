using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace AirQuality
{ 	/*	
		a class that Describes resource reagents and products of AQReaction objects. 
		AQResourceReagent is a representation of a KSP resource
	*/
	public class AQResourceReagent : ScriptableObject, IConfigNode 
	{
		public bool IsProduct()
		{
			return (Production > AQConventions.floatzero);
		}
		public bool IsConsumable()
		{
			return (Production < AQConventions.floatzero);
		}
		public void Save(ConfigNode node)
		{
			return;                        										 //not supposed to save reagents in-game
		}
		public void Load(ConfigNode node)
		{
			float f;
			bool b;
			if (node.HasValue(AQConventions.Values.Name))
			{
				Name = node.GetValue(AQConventions.Values.Name);
			}
			if (node.HasValue(AQConventions.Values.IsLimiting) && bool.TryParse(node.GetValue(AQConventions.Values.IsLimiting), out b))
			{
				IsLimiting = b;
			}
			if (node.HasValue(AQConventions.Values.Production) && float.TryParse(node.GetValue(AQConventions.Values.Production), out f))
			{
				Production = f;
			}
			return;
		}
		public bool IsLimiting;                //Limiting consumeables will stop or wind down the reaction if lacking todo possibly eliminate, replacing with IsConsumeable, as seeming there's no point consuming the resource unless it is limiting
		public string Name;                    //Displayeable resource name, doubling as its unique identifier
		public float Production;               //negative if consumed, in KSP units/second (probably SNC Litre/sec under RO conventions?)
	}
}
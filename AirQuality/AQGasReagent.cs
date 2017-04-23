using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace AirQuality
{ 	/*	
		a classes that describes possible AQGas reagents and products of AQReaction objects. 
		AQGasGeagent is a representation of an AQGas object.
	*/
	public class AQGasReagent : ScriptableObject, IConfigNode
	{
		public bool IsProduct()
		{
			return (Production > 0);
		}
		public bool IsConsumable()
		{
			return (Production < 0);
		}
		public void Save(ConfigNode node)
		{
			return;																//not supposed to save reagents in-game
		}
		public void Load(ConfigNode node)
		{
			float f;
			bool b;
			if (node.HasValue("Name"))
			{
				Name = node.GetValue("Name");
			}
			if (node.HasValue("IsLimiting") && bool.TryParse(node.GetValue("IsLimiting"), out b))
			{
				IsLimiting = b;
			}
			if (node.HasValue("Production") && float.TryParse(node.GetValue("Production"), out f))
			{
				Production = f;
			}
			return;
		}
		public bool IsLimiting;                //limiting gases will stop/wind down the reaction if consumed and absent/lacking in the air
		public string Name;						//Displayable name, same as AQGas.Longname
		public float Production;               //proportional if Reaction.Type is "Leak", negative if consumed, in mol/sec todo split Leak in a separate PartModule!
	}
}
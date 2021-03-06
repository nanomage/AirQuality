using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace AirQuality
{   
	/* a class describing a habitable volume. This is attached to KSP parts and allows them to house some AQAir
	 * inherent properties of a habitable volume should be restricted to its physical size and shape,
	 * while it should expose methods to determine various aspect's of cabin habitability, depending on shape and 
	 * air composition. As planned: */
	public class ModuleAQHabitableVolume : PartModule
	{
		[KSPField(isPersistant = true, guiActive = true)]
		public double LivingVolume;
		[KSPField(isPersistant = true, guiActive = true)]
		private double LastUpdate;
		public AQAir Air;
		public AQsettings InstanceAQSettings;

		/* this is debug-level data  */
		[KSPField(isPersistant = true, guiActive = true)]
		public float Debug_CarbonDioxidePressure;
		[KSPField(isPersistant = true, guiActive = true)]
		public float Debug_OxygenPressure;
	
		public override void OnAwake()
		{
			print("[AQ:HV] OnAwake");
			Air = new AQAir();
			InstanceAQSettings = new AQsettings();
			base.OnAwake();
		}
		public override void OnStart(StartState state)
		{
			print("[AQ:HV] Onstart: " + state);
			base.OnStart(state);
		}
		public override void OnLoad(ConfigNode node)
		{
			/* First, load the save-independent settings from the global settings node */
			print("[AQ:HV] Onload: " + node);
			print("[AQ:HV] Going to load from node " + AQNodeNames.Config);
			foreach (ConfigNode inode in GameDatabase.Instance.GetConfigNodes(AQNodeNames.Config))
			{
				print("[AQ:HV] Loading aqsettings from " + inode);
				InstanceAQSettings.Load(inode);
				print("[AQ:HV] Settings loaded");
				print("[AQ:HV]\tSimulationStep " + InstanceAQSettings.SimulationStep);
				print("[AQ:HV]\tMaxScaleFactor " + InstanceAQSettings.MaxScaleFactor);
			}
			/* now, load gases from the confignode supplied with the argument */
			if (node.HasNode(AQConventions.Values.AQAir) && (node.GetNode(AQConventions.Values.AQAir).CountNodes > 0))
			{
				print("[AQ:HV] Loading air from " + node.GetNode(AQConventions.Values.AQAir));
				Air.Load(node.GetNode(AQConventions.Values.AQAir));
				print("[AQ:HV] Loaded " + Air.Count + " gases.");
			}
			else /*load from the starting air node*/
			{
				print ("[AQ:HV] Going to load from StartingAir subnode of the node " + AQNodeNames.Config);
				foreach (ConfigNode inode in GameDatabase.Instance.GetConfigNodes(AQNodeNames.Config))
				{
					if (inode.HasNode(AQConventions.Values.StartingAir))
					{
						print("[AQ:HV] Loading starting air from " + inode.GetNode(AQConventions.Values.StartingAir));
						Air.Load(inode.GetNode(AQConventions.Values.StartingAir));
						print("[AQ:HV] Loaded " + Air.Count + " gases.");
					}
				}
			}
			base.OnLoad(node);
		}
		public override void OnSave(ConfigNode node)
		{
			ConfigNode Airnode;
			print("[AQ:HV] OnSave called for " + node);
			if (Air.Count == AQConventions.intzero)
			{
				print("[AQ:HV] Won't save empty air. Gases contained " + Air.Count);
				return;
			}
			if (node.HasNode(AQConventions.Values.AQAir))
			{
				print("[AQ:HV] AQAir node found " + node.GetNode(AQConventions.Values.AQAir));
				Airnode = node.GetNode(AQConventions.Values.AQAir);
			}
			else
			{
				Airnode = node.AddNode(AQConventions.Values.AQAir);
				print("[AQ:HV] Created AQAir node " + node.GetNode(AQConventions.Values.AQAir));
			}
			print("[AQ:HV] Saving " + Air.Count +" gases to " + Airnode);
			Air.Save(Airnode);
			print("[AQ:HV] Saved " + Airnode.CountNodes + "gases");
			base.OnSave(node);
		}
		public override void OnUpdate()
		{
			if (Time.timeSinceLevelLoad < AQConventions.one || !FlightGlobals.ready)
			{
				return;
			}
			if (LastUpdate == AQConventions.floatzero)			//exact float point comparison is done intentionally to catch the case of uninitialized variable
			{
				// Just started running
				LastUpdate = Planetarium.GetUniversalTime();
				return;
			}
			if ((Planetarium.GetUniversalTime() - LastUpdate) > InstanceAQSettings.SimulationStep)
			{
				double ScaleFactor = AQConventions.one;
				ScaleFactor = Math.Min(((Planetarium.GetUniversalTime() - LastUpdate) / InstanceAQSettings.SimulationStep), InstanceAQSettings.MaxScaleFactor);
				LastUpdate+= ScaleFactor * InstanceAQSettings.SimulationStep;
				Debug_CarbonDioxidePressure = (float)Air["CarbonDioxide"].Pressure;
				Debug_OxygenPressure = (float)Air["Oxygen"].Pressure;
			}
		}
	}
}


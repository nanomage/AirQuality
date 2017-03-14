using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace AirQuality
{   /*an interface that mandates a method to update cabin atmosphere, adjusted by scalefactor and cabin living volume   */
	interface IAQGasExchange
	{
		void UpdateAir(Dictionary<string, AQGas> Air, double LivingVolume, double ScaleFactor);
	}
	/* a class containing static variables describing abstract 'standard conditions'
	 *	and some handy physical constants. Standard temperature is assumed
	 *	to be kept constant in the cabin all the time, but standard pressure 
	 *	and molar volume should only be used for initial terrestrial calculations */
	class AQPhysicalConstants
	{
		public static double GasConstant = 8.3144598f;                  //in J/K/mol
		public struct StandardAmbientConditions
		{
			public static double MolarVolume = .024789598f;             //in cubic metres/mol
			public static double Temperature = 298.15f;                 //in K
			public static double Pressure = 100000.0f;                  //in Pa
		}
		public static Dictionary<string, double> MolarMass =            //in kg/mol
			new Dictionary<string, double>								
		{
			{"CarbonDioxide",0.04401f},									//these are actually quite bad, because they
			{"Oxygen", 0.0319988}										//are terrestrial values and will change on
		};																//other planets. Luckily ISRU is out of scope.
	}
	/* a class containing mod's global configuration, simulation settings, and starting atmospheric conditions
	 * they are supposed to be loaded from a configuration node in a AQSettings.cfg file, but not supposed to be 
	 * ever saved. */
	public class AQsettings : IConfigNode
	{
		private const string ConfigNodeName = "AQSettings";
		public double SimulationStep;
		public double MaxScaleFactor;
		public Dictionary<string, AQGas> StartingAir;
		private AQGas Gas;
		public Dictionary<string, double> GasProduction;
		public void Load(ConfigNode node)
		{
			float f;
			if (node.HasValue("SimulationStep") && float.TryParse(node.GetValue("SimulationStep"), out f))
			{
				SimulationStep = f;
			}
			if (node.HasValue("MaxScaleFactor") && float.TryParse(node.GetValue("MaxScaleFactor"), out f))
			{
				MaxScaleFactor = f;
			}
			if (node.HasNode("StartingAir"))
			{
				foreach (ConfigNode ChildNode in node.GetNode("StartingAir").GetNodes())
				{
					Gas.Load(ChildNode);
					StartingAir.Add(ChildNode.name, Gas);
				}
			}
			    if (node.HasNode("GasProduction"))
			{
				ConfigNode ChildNode = node.GetNode("GasProduction");
				foreach (string GasEntry in ChildNode.GetValues())
				{
					if (float.TryParse(ChildNode.GetValue(GasEntry), out f))
					{
						GasProduction.Add(GasEntry, f);
					}
				}
			}
		}
		public void Save(ConfigNode node)
		{
			return;						//not supposed to save settings in-game, only load from the config
		}
	}
	/* a class describing a gas as a component of cabin atmosphere */
	public class AQGas : IConfigNode
	{
		public string LongName;
		public string ShortName;
		public double Pressure;
		public double MinRequiredPressure;
		public double MaxToleratedPressure;
		public double CondensationPressure;
		public double MolarMass;
		public double Quantity;
		public double NarcoticPotential;
		public bool isBreatheable()
		{
			if (MinRequiredPressure > 0) return true;
			return false;
		}
		public bool isPoison()
		{
			if (MaxToleratedPressure > 0) return true;
			return false;
		}
		public void Load(ConfigNode node)
		{
			float f;
			if (node.HasValue("LongName"))
			{
				LongName = node.GetValue("LongName");
			}
			if (node.HasValue("ShortName"))
			{
				LongName = node.GetValue("ShortName");
			}
			if (node.HasValue("Pressure") && float.TryParse(node.GetValue("Pressure"), out f))
			{
				Pressure = f;
			}
			if (node.HasValue("MinRequiredPressure") && float.TryParse(node.GetValue("MinRequiredPressure"), out f))
			{
				MinRequiredPressure = f;
			}
			if (node.HasValue("MaxToleratedPressure") && float.TryParse(node.GetValue("MaxToleratedPressure"), out f))
			{
				MaxToleratedPressure = f;
			}
			if (node.HasValue("CondensationPressure") && float.TryParse(node.GetValue("CondensationPressure"), out f))
			{
				CondensationPressure = f;
			}
			if (node.HasValue("MolarMass") && float.TryParse(node.GetValue("MolarMass"), out f))
			{
				MolarMass = f;
			}
			if (node.HasValue("Quantity") && float.TryParse(node.GetValue("Quantity"), out f))
			{
				Quantity = f;
			}
		    if (node.HasValue("NarcoticPotential") && float.TryParse(node.GetValue("NarcoticPotential"), out f))
			{
				NarcoticPotential = f;
			}
		}
	
		public void Save(ConfigNode node)
		{
			node.AddValue("LongName", LongName);	
			node.AddValue("ShortName", ShortName);
			node.AddValue("Pressure", Pressure);
			node.AddValue("MinRequiredPressure", MinRequiredPressure);
			node.AddValue("MaxToleratedPressure", MaxToleratedPressure);
			node.AddValue("MolarMass", MolarMass);
			node.AddValue("Quantity", Quantity);
			node.AddValue("NarcoticPotential", NarcoticPotential);
		}
	}
	/* a class describing total atmospheric composition of a habitable volume */
	public class AQAir : Dictionary<string, AQGas>, IConfigNode
	{
		public float Temperature;
		private string AQGlobalSettingsNode = "AQGlobalSettings";
		public bool IsBreatheable()
		{
			return true;
		}
		public bool IsPressurised()
		{
			return true;
		}
		public float TotalNarcoticPotential()
		{
			return 0;
		}
		public void Load(ConfigNode AQAirNode)
		{
			return;
		}
		public void Save(ConfigNode AQAirNode)
		{
			return;
		}
	}
	/* a class describing the part's crew, particularly it's capacity to influence cabin air */
	public class AQCrew : IAQGasExchange
	{
		public float CrewNumber;
		public Dictionary<string, double> GasProduction = new Dictionary<string, double>();
		public void Initialise(List<ProtoCrewMember> protoCrew, AQsettings Settings)
		{
			CrewNumber = protoCrew.Count;
			foreach (string GasName in Settings.GasProduction.Keys)
			{
				GasProduction.Add(GasName, Settings.GasProduction[GasName]);
			}
			//	GasProduction.Add("Oxygen", -0.00025679080155881272);
			//	GasProduction.Add("CarbonDioxide", 0.00025679080155881272);
			return;
		}
		public void UpdateAir(Dictionary<string, AQGas> Air, double LivingVolume, double ScaleFactor)
		{
			foreach (string GasProductionEntry in GasProduction.Keys)
			{
				Air[GasProductionEntry].Pressure += ScaleFactor * CrewNumber * GasProduction[GasProductionEntry] * 
					AQPhysicalConstants.GasConstant * AQPhysicalConstants.StandardAmbientConditions.Temperature / LivingVolume;
			}
			return;
		}
	}
	/* the main class describing a habitable volume. This is attached to KSP parts and allows them to house some
	 * AQAir
	 * as well as to have gas exchange modules like
	 * AQCrew 
	 * AQScrubber
	 * AQAirFreezer
	 * AQGreenhouse
	 * AQAirVent
	 * AQLeak
	 * inherent properties of a habitable volume should be restricted to its physical size and shape,
	 * while it should expose methods to determine various aspect's of cabin habitability, depending on shape and 
	 * air composition. As planned:
	 * bool AirIsBreatheable for sufficient oxygen and little enough poison gases to breathe without a mask
	 * bool AirIsPressurised for pressure exceeding the partial pressure of water vapour under 36.6 degrees centigrade
	 * bool AirIsExplosive   for dangerously explosive combinations of combustible gases with oxidisers
	 * float TotalNarcoticPotential for total lipid solubility of components under cabin pressure.
	 * but heaps of others can be thought of*/
	public class HabitableVolume : PartModule
	{
		[KSPField(isPersistant = true, guiActive = true)]
		public float Debug_CarbonDioxidePressure;

		private double LastUpdate;
		private double SimulationStep;
		public AQsettings Settings;

		[KSPField(isPersistant = true)]
		public Dictionary<string, AQGas> Air;


		public class HabitableVolumeConfig : IConfigNode
		{
			public double Volume;
			public void Load(ConfigNode node)
			{
				float f;
				if (node.HasValue("LivingVolume") && float.TryParse(node.GetValue("LivingVolume"), out f))
				{
					Volume = f;
				}
			}
			public void Save(ConfigNode node)
			{
				node.AddValue("LivingVolume", Volume);
			}
		}
		[KSPField(isPersistant = true)]
		public double LivingVolume;

		public AQCrew Crew;
		/*a method to tell if the current atmosphere is breatheable, as defined by properties of constituent gases.
		 *This doesn't take the actual crew gas consumption need into account */
		private bool AirIsBreatheable()
		{
			foreach (string GasName in Air.Keys)
				if ((Air[GasName].MaxToleratedPressure > 0) && (Air[GasName].Pressure > Air[GasName].MaxToleratedPressure))
					return false;                                        //Poison gas pressure exceeded
			foreach (string GasName in Air.Keys)
				if ((Air[GasName].MinRequiredPressure > 0) && (Air[GasName].Pressure > Air[GasName].MinRequiredPressure))
					return true;                                         //Enough breatheable gas
			return false;                                                //Not enough breatheable gas
		}
		//
		public override void OnAwake()
		{

			print("OnAwake");
			base.OnAwake();
		}
		public override void OnStart(StartState state)
		{
			print("Onstart: " + state);
		}
		public override void OnLoad(ConfigNode AQModuleNode)
		{
			/* First, load the save-independent settings from the global settings node */
			print("Onload: " + node);
			AQsettings InstanceAQSettings = new AQsettings;
			string AQSettingsNodeName = "AQGlobalSettings";
			ConfigNode CabinAirNode;
			foreach (ConfigNode AQSettingsNode in GameDatabase.Instance.GetConfigNodes(AQSettingsNodeName))
			{
				InstanceAQSettings.Load(AQSettingsNode);
			}
			/* now, load gases from the confignode supplied with the argument */
			if (AQModuleNode.HasNode("CabinAir"))
			{
				CabinAirNode = AQModuleNode.GetNode("CabinAir");
				foreach (ConfigNode ChildNode in CabinAirNode.GetNodes())
				{ 
				}
			}




			/* now. load gas properties from the global settings node*/
			foreach (ConfigNode AQSettingsNode in GameDatabase.Instance.GetConfigNodes(AQSettingsNodeName))
			{
				
			}
			base.OnLoad(node);
		}
		public override void OnUpdate()
		{
			if (Time.timeSinceLevelLoad < 1.0f || !FlightGlobals.ready)
			{
				return;
			}
			if (LastUpdate == 0.0f)
			{
				// Just started running
				LastUpdate = Planetarium.GetUniversalTime();
				return;
			}
			if ((Planetarium.GetUniversalTime() - LastUpdate) > SimulationStep)
			{
				double ScaleFactor = 1.0f;
				ScaleFactor = Math.Min(((Planetarium.GetUniversalTime() - LastUpdate) / SimulationStep), Settings.MaxScaleFactor);
				LastUpdate+= ScaleFactor * SimulationStep;
				Crew.UpdateAir(Air, LivingVolume, ScaleFactor);
				Debug_CarbonDioxidePressure = (float)Air["CarbonDioxide"].Pressure;
			}
		}
	}
}


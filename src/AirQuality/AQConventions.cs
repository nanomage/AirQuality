using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace AirQuality
{   /* a class containing node names that store mod settings, gas and reaction definitions */
	class AQNodeNames
	{
		public static string Config = "AQSettings";
		public static string GasLibrary = "AQGasDefinitions";
	}
	/* a class containing string and mathematical constants that the mod uses */
	class AQConventions
	{
		public static double floatzero = 0.0f;
		public static int intzero = 0;
		public static double one = 1.0f;
		public struct Values
		{
			public static string Description = "Description";
			public static string AQReactions = "AQReactions";
			public static string StartingAir = "StartingAir";
			public static string AQAir = "AQAir";
			public static string LongName = "LongName";
			public static string ShortName = "ShortName";
			public static string Pressure = "Pressure";
			public static string MinRequiredPressure = "MinRequiredPressure";
			public static string MaxToleratedPressure = "MaxToleratedPressure";
			public static string CondensationPressure = "CondensationPressure";
			public static string MolarMass = "MolarMass";
			public static string NarcoticPotential = "NarcoticPotential";
			public static string Name = "Name";
			public static string IsLimiting = "IsLimiting";
			public static string Production = "Production";
			public static string Status = "Status";
			public static string AlwaysActive = "AlwaysActive";
			public static string Type = "Type";
			public static string AQResourceReagent = "AQResourceReagent";
			public static string AQGasReagent = "AQGasReagent";
			public static string SimulationStep = "SimulationStep";
			public static string MaxScaleFactor = "MaxScaleFactor";
			public static string EmptyString = "";
		}
		public struct Statuses
		{
			public static string Nominal = "Nominal";
			public static string Lacking = "Lacking ";
			public static string Limited = "Limited by ";
		}
		public struct ReactionTypes
		{
			public static string Breathe = "Breathe";
			public static string Leak = "Leak";
			public static string Scrub = "Scrub";
			public static string Backfill = "Backfill";
		}
	}
}


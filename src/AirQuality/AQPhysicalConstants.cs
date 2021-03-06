using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace AirQuality
{   /* a class containing static variables describing abstract 'standard conditions'
	 *	and some handy physical constants. Standard temperature is assumed
	 *	to be kept constant in the cabin all the time, but standard pressure 
	 *	and molar volume should only be used for initial terrestrial calculations */
	class AQPhysicalConstants
	{
		public static double ArmstrongLimit = 6250.0f;                  //in Pa
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
		};                                                              //other planets. Luckily ISRU is out of scope.
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace AirQuality
{   /*an interface that mandates a method to update cabin atmosphere, adjusted by scalefactor and cabin living volume   */
	interface IAQGasExchange
	{
		void UpdateAir(AQAir Air, double LivingVolume, double ScaleFactor, double CrewFactor);
	}
}


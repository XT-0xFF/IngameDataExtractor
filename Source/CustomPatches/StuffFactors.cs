using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace CustomPatches
{
	public class StuffFactors
	{
		public string Name;
		public float Mass;
		public float Armor_Sharp;
		public float Armor_Blunt;
		public float Armor_Heat;
		public float Insulation_Cold;
		public float Insulation_Heat;

		public StuffFactors()
		{ }
		public StuffFactors(ThingDef def)
		{
			Name = def.defName;
			if (def.statBases != null)
			{
				foreach (var stat in def.statBases)
				{
					if (stat.stat.defName == "Mass")
						Mass = stat.value;

					if (stat.stat.defName == "StuffPower_Armor_Sharp")
						Armor_Sharp = stat.value;
					if (stat.stat.defName == "StuffPower_Armor_Blunt")
						Armor_Blunt = stat.value;
					if (stat.stat.defName == "StuffPower_Armor_Heat")
						Armor_Heat = stat.value;

					if (stat.stat.defName == "StuffPower_Insulation_Cold")
						Insulation_Cold = stat.value;
					if (stat.stat.defName == "StuffPower_Insulation_Heat")
						Insulation_Heat = stat.value;
				}
			}
			else
				throw new Exception("StuffFactor statBase null for " + Name);
			//Log.Message(ToString());
		}
		public override string ToString() =>
			$"{Name} - Sharp {Armor_Sharp} Blunt {Armor_Blunt} Heat {Armor_Heat} - Cold {Insulation_Cold} Heat {Insulation_Heat}";
	}
}

using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CustomPatches.ItemListGeneration
{
	public class LeatherAndClothGenerator
	{
		public class LeatherAndClothOutput
		{
			public string Label { get; set; } = "";
			public string ModName { get; set; } = "";

			public string Type { get; set; }

			public float MarketValue { get; set; } = 0.0f;
			public float MaxHitPoints { get; set; } = 1.0f;
			public float ArmorSharp { get; set; } = 1.0f;
			public float ArmorBlunt { get; set; } = 1.0f;
			public float ArmorHeat { get; set; } = 1.0f;
			public float InsulationCold { get; set; } = 0.0f;
			public float InsulationHeat { get; set; } = 0.0f;

			public float Commonality { get; set; }
			public Color Color { get; set; }
			public float Beauty { get; set; } = 1.0f;

			public static string HeaderRow()
			{
				String output = "Material;";
				output += "Mod;";
				output += "Type;";
				output += "MarketValue;";
				output += "MaxHitPoints;";
				output += "ArmorSharp;";
				output += "ArmorBlunt;";
				output += "ArmorHeat;";
				output += "InsulationCold;";
				output += "InsulationHeat;";
				output += "Commonality;";
				output += "Color.Red;";
				output += "Color.Blue;";
				output += "Color.Green;";
				output += "Beauty;";
				return output;
			}

			public override string ToString()
			{
				// Item
				String output = $"{Label};";

				// Mod Name
				output += $"{ModName};";

				// Stats
				output += $"{Type};";
				output += $"{MarketValue};";
				output += $"{MaxHitPoints};";
				output += $"{ArmorSharp};";
				output += $"{ArmorBlunt};";
				output += $"{ArmorHeat};";
				output += $"{InsulationCold};";
				output += $"{InsulationHeat};";
				output += $"{Commonality};";
				output += $"{Color.r};{Color.g};{Color.b};";
				output += $"{Beauty};";
				return output;
			}
		}

		public static void GenerateList()
		{
			// get apparel list of things
			var thingDefs = DefDatabase<ThingDef>.AllDefsListForReading.Where(t => t?.stuffProps?.categories?.FirstOrDefault((x) => x.defName == "Leathery" || x.defName == "Fabric") != null);
			if (thingDefs?.Count() > 0)
			{
				List<LeatherAndClothOutput> matList = new List<LeatherAndClothOutput>();
				foreach (var thing in thingDefs)
				{
					LeatherAndClothOutput mat = new LeatherAndClothOutput
					{
						Label = thing.label.CapitalizeFirst(),
						ModName = thing.modContentPack?.Name
					};

					if (thing.statBases != null)
					{
						foreach (var stat in thing.statBases)
						{
							var name = stat.stat.defName;
							var value = stat.value;
							switch (name)
							{
								case "MarketValue":
									mat.MarketValue = value;
									break;
								case "StuffPower_Armor_Sharp":
									mat.ArmorSharp = value;
									break;
								case "StuffPower_Armor_Blunt":
									mat.ArmorBlunt = value;
									break;
								case "StuffPower_Armor_Heat":
									mat.ArmorHeat = value;
									break;
								case "StuffPower_Insulation_Cold":
									mat.InsulationCold = value;
									break;
								case "StuffPower_Insulation_Heat":
									mat.InsulationHeat = value;
									break;
							}
						}
					}

					if (thing.stuffProps != null)
					{
						if (thing.stuffProps.categories != null)
						{
							List<string> type = new List<string>();
							foreach (var cat in thing.stuffProps.categories)
							{
								var name = cat.defName;
								if (name == "Leathery")
									type.Add("Leathery");
								else if (name == "Fabric")
									type.Add("Fabric");
							}
							if (type.Count > 0)
								mat.Type = string.Join(", ", type);
						}

						if (thing.stuffProps.color != null)
							mat.Color = thing.stuffProps.color;

						mat.Commonality = thing.stuffProps.commonality;

						if (thing.stuffProps.statFactors != null)
						{
							foreach (var stat in thing.stuffProps.statFactors)
							{
								var name = stat.stat.defName;
								var value = stat.value;
								switch (name)
								{
									case "Beauty":
										mat.Beauty = value;
										break;
									case "MaxHitPoints":
										mat.MaxHitPoints = value;
										break;
								}
							}
						}
					}

					matList.Add(mat);
				}

				string output = LeatherAndClothOutput.HeaderRow() + "\n";
				foreach (var item in matList)
					output += item.ToString() + "\n";
				//Log.Message(output);
				try
				{
					string outputPath = "Mods\\IngameDataExtractor\\Raw_LeathersAndCloth.csv";
					File.WriteAllText(outputPath, output);
					Log.Message(outputPath);
				}
				catch (Exception exc)
				{
					Log.Error(nameof(LeatherAndClothGenerator) + ": failed writing to file: " + exc.Message);
				}
			}
			else
				Log.Error(nameof(LeatherAndClothGenerator) + ": no leathers & cloth found; this should not happen!");
		}
	}
}

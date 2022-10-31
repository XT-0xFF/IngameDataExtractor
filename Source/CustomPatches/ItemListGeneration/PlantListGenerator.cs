using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace CustomPatches.ItemListGeneration
{
	public class PlantListGenerator
	{
		public class PlantOutput
		{
			public string Label { get; set; } = "";
			public string ModName { get; set; } = "";

			public float MinSkill { get; set; }
			public float FertilityRequired { get; set; }
			public float FertilitySensitivity { get; set; }
			public float GlowRequired { get; set; }
			public float GlowOptimal { get; set; }
			public bool BlockAdjacentSow { get; set; }
			public float GrowDays { get; set; }
			public float PlantNutrition { get; set; }
			public float PlantNutritionPerDay => PlantNutrition / GrowDays * (BlockAdjacentSow ? 0.25f : 1.0f);

			public bool HasProduct { get; set; } = false;
			public string ProductLabel { get; set; }
			public FoodTypeFlags ProductFoodType { get; set; } = FoodTypeFlags.None;
			public float ProductYield { get; set; }
			public float ProductNutrition { get; set; }
			public float ProductNutritionPerDay => ProductNutrition * ProductYield / GrowDays * (BlockAdjacentSow ? 0.25f : 1.0f);
			public bool ProductIsEdible => ProductFoodType != FoodTypeFlags.None && FoodTypeFlags.OmnivoreHuman.HasFlag(ProductFoodType);
			public string ProductChemical { get; set; } = "";

			public static string HeaderRow()
			{
				String output = "Plant;";
				output += "Mod;";
				output += "Skill;";
				output += "Fertility Required;";
				output += "Fertility Sensitivity;";
				output += "Light Required;";
				output += "Light Optimal;";
				output += "Blocks Adjacent;";
				output += "Days;";
				output += "Plant Nutrition;";
				output += "Plant Nutr/Day;";
				output += "Product;";
				output += "Product FoodType;";
				output += "Product Yield;";
				output += "Product Nutrition;";
				output += "Product Nutr/Day;";
				output += "Product Human Edible;";
				output += "Product Drug;";
				return output;
			}

			public override string ToString()
			{
				// Item
				String output = $"{Label};";

				// Mod Name
				output += $"{ModName};";

				// Stats
				output += $"{MinSkill};";
				output += $"{FertilityRequired};";
				output += $"{FertilitySensitivity};";
				output += $"{GlowRequired};";
				output += $"{GlowOptimal};";
				output += $"{BlockAdjacentSow};";
				output += $"{GrowDays};";
				output += $"{PlantNutrition};";
				output += $"{PlantNutritionPerDay};";

				if (HasProduct)
				{
					output += $"{ProductLabel};";
					output += $"{ProductFoodType};";
					output += $"{ProductYield};";
					output += $"{ProductNutrition};";
					output += $"{ProductNutritionPerDay};";
					output += $"{ProductIsEdible};";
					output += $"{ProductChemical};";
				}
				else
				{
					output += ";;;;;;;";
				}

				return output;
			}
		}

		public static void GenerateList()
		{
			// get apparel list of things
			var thingDefs = DefDatabase<ThingDef>.AllDefsListForReading.Where(t => t.thingClass == typeof(Plant) || t.thingClass.ToString() == "VanillaBrewingExpanded.Plant_AutoProduce");
			if (thingDefs?.Count() > 0)
			{
				List<PlantOutput> plantList = new List<PlantOutput>();
				foreach (var thing in thingDefs)
				{
					PlantOutput plant = new PlantOutput
					{
						Label = thing.label.CapitalizeFirst(),
						ModName = thing.modContentPack?.Name
					};
					
					if (thing.statBases != null)
					{
						foreach (var stat in thing.statBases)
						{
							var name = stat.stat?.defName;
							var value = stat.value;
							if (name == "Nutrition")
								plant.PlantNutrition = value;
						}
					}
					
					if (thing.plant != null)
					{
						if (!thing.plant.Sowable)
						{
							Log.Message(plant.Label + " not sowable, skipped");
							continue;
						}
						//if (!thing.plant.Harvestable)
						//{
						//	Log.Message(plant.Label + " not harvestable, skipped");
						//	continue;
						//}

						plant.BlockAdjacentSow = thing.plant.blockAdjacentSow;
						plant.GrowDays = thing.plant.growDays;
						plant.FertilityRequired = thing.plant.fertilityMin;
						plant.FertilitySensitivity = thing.plant.fertilitySensitivity;
						plant.ProductYield = thing.plant.harvestYield;
						plant.MinSkill = thing.plant.sowMinSkill;
						plant.GlowRequired = thing.plant.growMinGlow;
						plant.GlowOptimal = thing.plant.growOptimalGlow;
						
						var harvestedThingDef = thing.plant.harvestedThingDef;
						if (harvestedThingDef != null)
						{
							plant.HasProduct = true;
							plant.ProductLabel = harvestedThingDef.label?.CapitalizeFirst();
							if (harvestedThingDef.ingestible != null)
								plant.ProductFoodType = harvestedThingDef.ingestible.foodType;

							if (harvestedThingDef.statBases != null)
							{
								foreach (var stat in harvestedThingDef.statBases)
								{
									var name = stat.stat?.defName;
									var value = stat.value;
									if (name == "Nutrition")
										plant.ProductNutrition = value;
								}
							}
							if (harvestedThingDef.comps != null)
							{
								foreach (var comp in harvestedThingDef.comps)
								{
									if (comp is CompProperties_Drug drug)
									{
										plant.ProductChemical = drug.chemical?.label?.CapitalizeFirst();
									}
								}
							}
						}
					}

					plantList.Add(plant);
				}

				string output = PlantOutput.HeaderRow() + "\n";
				foreach (var item in plantList)
					output += item.ToString() + "\n";
				//Log.Message(output);
				try
				{
					string outputPath = "Mods\\IngameDataExtractor\\Raw_Plants.csv";
					File.WriteAllText(outputPath, output);
					Log.Message(outputPath);
				}
				catch (Exception exc)
				{
					Log.Error(nameof(PlantListGenerator) + ": failed writing to file: " + exc.Message);
				}
			}
			else
				Log.Error(nameof(PlantListGenerator) + ": no plants found; this should not happen!");
		}
	}
}

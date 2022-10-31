using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace CustomPatches.ItemListGeneration
{
	public class AnimalListGenerator
	{
		public class AnimalOutput
		{
			public string Label { get; set; } = "";
			public string ModName { get; set; } = "";

			public float MarketValue { get; set; } = 0.0f;

			public float BaseBodySize { get; set; } = float.NaN;

			public float MeatAmount { get; set; } = 90.0f;
			public float MeatAmountTotal { get; set; } = 0;
			public float MeatNutritionTotal => MeatAmountTotal * 0.05f;
			public float MeatPerGrowthDay => DaysToAdult > 0 ? MeatAmountTotal / DaysToAdult : 0.0f;

			public string LeatherType { get; set; } = "";
			public float LeatherAmount { get; set; } = 30.0f;
			public float LeatherAmountTotal { get; set; } = 0;
			public float LeatherValue { get; set; } = 0;
			public float LeatherValueTotal => LeatherAmountTotal * LeatherValue;

			public string MilkType { get; set; } = "";
			public float MilkIntervalDays { get; set; } = 1.0f;
			public float MilkAmount { get; set; } = 0.0f;
			public bool MilkFemalesOnly { get; set; } = true;
			public float MilkNutrition { get; set; } = 0.0f;
			public float MilkNutritionPerDay => (MilkNutrition * MilkAmount) / MilkIntervalDays;

			public float BaseHungerRate { get; set; } = 0;
			public float HungerRate => BaseHungerRate * 1.6f;
			public List<string> FoodType { get; set; } = new List<string>();

			public float ComfyTempMax { get; set; } = 40.0f;
			public float ComfyTempMin { get; set; } = float.NaN;

			public float GestationPeriodDays { get; set; } = float.NaN;
			public float DaysToAdult { get; set; } = float.NaN;
			public float MateMtbHours { get; set; } = float.NaN;
			public float AverageLitterSizeOrEggs { get; set; } = 0; // TODO
			public float LifeExpectancy { get; set; } = float.NaN;

			public float Wildness { get; set; } = float.NaN;
			public float MinimumHandlingSkill { get; set; } = float.NaN;
			public float ManhunterOnDamageChance { get; set; } = float.NaN;
			public float ManhunterOnTameFailChance { get; set; } = float.NaN;
			public string Trainability { get; set; } = "";
			public float RoamMtbDays { get; set; } = 0.0f;
			public bool BlockedByFence { get; set; } = false;
			public float CarryingCapacity { get; set; } = 75;
			public bool PackAnimal { get; set; } = false;
			public float RidingSpeed { get; set; } = 0.0f;
			public float NuzzleMtbhHours { get; set; } = 0;
			public float Petness { get; set; } = 0.0f;
			public bool Predator { get; set; } = false;
			public float MaxPreyBodySize { get; set; } = 0.0f;

			public float MovementSpeed { get; set; } = float.NaN;
			public float BaseHealthScale { get; set; } = float.NaN;
			public float Health => BaseHealthScale * 30;

			public float DamagePerSecond { get; set; } = 0;
			public float Armor_Sharp { get; set; } = 0;
			public float Armor_Blunt { get; set; } = 0;
			public float Armor_Heat { get; set; } = 0;

			public static string HeaderRow()
			{
				String output = "Animal;";
				output += "Mod;";
				output += "Market Value;";
				output += "Base Body Size;";
				//output += "Meat Amount;";
				output += "Meat Amount Total;";
				output += "Meat Nutrition Total;";
				output += "Meat Per Growth Day;";
				output += "Leather Type;";
				//output += "Leather Amount;";
				output += "Leather Amount Total;";
				output += "Leather Value;";
				output += "Leather Value Total;";
				output += "Milk Type;";
				output += "Milk Interval Days;";
				output += "Milk Amount;";
				output += "Milk Females Only;";
				//output += "MilkNutrition;";
				output += "Milk Nutrition/Day;";
				//output += "Base Hunger Rate;";
				output += "Hunger Rate;";
				output += "Food Type;";
				output += "Comfy Temp Max;";
				output += "Comfy Temp Min;";
				output += "Gestation Period Days;";
				output += "Days To Adult;";
				output += "Mate Mtbh Hours;";
				output += "Average Litter/Eggs;";
				output += "Life Expectancy;";
				output += "Wildness;";
				output += "Minimum Handling Skill;";
				output += "Manhunter On Damage;";
				output += "Manhunter On Tame Fail;";
				output += "Trainability;";
				output += "Roam Mtb Days;";
				output += "Block by Fences;";
				output += "Pack Animal;";
				output += "Carrying Capacity;";
				output += "Riding Speed;";
				output += "Nuzzle Mtbh Hours;";
				output += "Petness;";
				output += "Predator;";
				output += "Max Prey Body Size;";
				output += "Movement Speed;";
				//output += "Base Health Scale;";
				output += "Health;";
				output += "Damage/Second;";
				output += "Armor Sharp;";
				output += "Armor Blunt;";
				output += "Armor Heat;";
				return output;
			}

			public override string ToString()
			{
				// Item
				String output = $"{Label};";

				// Mod Name
				output += $"{ModName};";

				// Stats
				output += $"{MarketValue};";
				output += $"{BaseBodySize};";
				//output += $"{MeatAmount};";
				output += $"{MeatAmountTotal};";
				output += $"{MeatNutritionTotal};";
				output += $"{MeatPerGrowthDay};";
				if (!string.IsNullOrEmpty(LeatherType))
				{
					output += $"{LeatherType};";
					//output += $"{LeatherAmount};";
					output += $"{LeatherAmountTotal};";
					output += $"{LeatherValue};";
					output += $"{LeatherValueTotal};";
				}
				else
				{
					output += ";";
					//output += ";";
					output += ";";
					output += ";";
					output += ";";
				}
				if (!string.IsNullOrEmpty(MilkType))
				{
					output += $"{MilkType};";
					output += $"{MilkIntervalDays};";
					output += $"{MilkAmount};";
					output += $"{MilkFemalesOnly};";
					//output += $"{MilkNutrition};";
					output += $"{MilkNutritionPerDay};";
				}
				else
				{
					output += ";";
					output += ";";
					output += ";";
					output += ";";
					//output += ";";
					output += ";";
				}
				//output += $"{BaseHungerRate};";
				output += $"{HungerRate};";
				output += String.Join(", ", FoodType) + ";";
				output += $"{ComfyTempMax};";
				output += $"{ComfyTempMin};";
				output += $"{GestationPeriodDays};";
				output += $"{DaysToAdult};";
				output += $"{MateMtbHours};";
				output += $"{AverageLitterSizeOrEggs};";
				output += $"{LifeExpectancy};";
				output += $"{Wildness};";
				output += $"{MinimumHandlingSkill};";
				output += $"{ManhunterOnDamageChance};";
				output += $"{ManhunterOnTameFailChance};";
				output += $"{Trainability};";
				output += $"{RoamMtbDays};";
				output += $"{BlockedByFence};";
				output += $"{PackAnimal};";
				output += $"{CarryingCapacity};";
				output += $"{RidingSpeed};";
				output += $"{NuzzleMtbhHours};";
				output += $"{Petness};";
				output += $"{Predator};";
				output += $"{MaxPreyBodySize};";
				output += $"{MovementSpeed};";
				//output += $"{BaseHealthScale};";
				output += $"{Health};";
				output += $"{DamagePerSecond};";
				output += $"{Armor_Sharp};";
				output += $"{Armor_Blunt};";
				output += $"{Armor_Heat};";

				return output;
			}
		}

		public static void GenerateList()
		{
			// get apparel list of things
			var thingDefs = DefDatabase<ThingDef>.AllDefsListForReading.Where(t => t.thingClass == typeof(Pawn));
			if (thingDefs?.Count() > 0)
			{
				List<AnimalOutput> animalist = new List<AnimalOutput>();
				foreach (var thing in thingDefs)
				{
					AnimalOutput animal = new AnimalOutput
					{
						Label = thing.label.CapitalizeFirst(),
						ModName = thing.modContentPack?.Name
					};

					if (thing.comps != null)
					{
						foreach (var comp in thing.comps)
						{
							if (comp is CompProperties_Milkable milkable)
							{
								var milkDef = milkable.milkDef;
								animal.MilkType = milkDef?.label.CapitalizeFirst();
								animal.MilkIntervalDays = milkable.milkIntervalDays;
								animal.MilkAmount = milkable.milkAmount;
								animal.MilkFemalesOnly = milkable.milkFemaleOnly;

								if (milkDef?.IsNutritionGivingIngestible == true)
								{
									foreach (var stat in milkDef.statBases)
										if (stat.stat.defName == "Nutrition")
											animal.MilkNutrition = stat.value;
								}
							}
						}
					}

					if (thing.tools != null)
					{
						// TODO check for missing manipulation parts and factor that in?
						animal.DamagePerSecond = CalculateDPS(thing.tools) * 0.62f;
					}

					if (thing.race != null)
					{
						var race = thing.race;
						animal.LeatherType = race.leatherDef?.label?.CapitalizeFirst();
						animal.LeatherValue = race.leatherDef?.BaseMarketValue ?? 0.0f;
						animal.BaseHungerRate = race.baseHungerRate;
						animal.BaseBodySize = race.baseBodySize;
						animal.BaseHealthScale = race.baseHealthScale;

						animal.Trainability = race.trainability?.label?.CapitalizeFirst();
						animal.RoamMtbDays = race.roamMtbDays ?? 0.0f;
						animal.BlockedByFence = race.FenceBlocked;
						animal.PackAnimal = race.packAnimal;
						animal.CarryingCapacity *= animal.BaseBodySize;
						animal.Wildness = race.wildness;
						animal.MinimumHandlingSkill = Mathf.RoundToInt(Mathf.Clamp(GenMath.LerpDouble(0.15f, 1f, 0f, 10f, animal.Wildness), 0f, 20f));
						animal.NuzzleMtbhHours = race.nuzzleMtbHours;
						animal.Petness = race.petness;
						animal.Predator = race.predator;
						animal.MaxPreyBodySize = race.maxPreyBodySize;

						animal.GestationPeriodDays = race.gestationPeriodDays;
						animal.MateMtbHours = race.mateMtbHours;
						if (race.lifeStageAges != null)
						{
							foreach (var item in race.lifeStageAges)
								if (item.def.label.ToLower().Contains("adult"))
									animal.DaysToAdult = item.minAge * 60.0f;
						}
						// TODO Egg layer or litter !?
						// TODO animal.AverageLitterSizeOrEggs = race.litterSizeCurve ??? ;
						animal.LifeExpectancy = race.lifeExpectancy;

						animal.ManhunterOnDamageChance = race.manhunterOnDamageChance;
						animal.ManhunterOnTameFailChance = race.manhunterOnTameFailChance;

						foreach (FoodTypeFlags item in Enum.GetValues(typeof(FoodTypeFlags)))
							if (item != FoodTypeFlags.None && race.foodType.HasFlag(item))
								animal.FoodType.Add(item.ToString());
					}

					if (thing.statBases != null)
					{
						foreach (var stat in thing.statBases)
						{
							var name = stat.stat.defName;
							var value = stat.value;
							switch (name)
							{
								case "MoveSpeed":
									animal.MovementSpeed = value;
									break;
								case "MarketValue":
									animal.MarketValue = value;
									break;
								case "ComfyTemperatureMax":
									animal.ComfyTempMax = value;
									break;
								case "ComfyTemperatureMin":
									animal.ComfyTempMin = value;
									break;
								case "MeatAmount":
									animal.MeatAmount = value;
									break;
								case "LeatherAmount":
									animal.LeatherAmount = value;
									break;
								case "ArmorRating_Sharp":
									animal.Armor_Sharp = value;
									break;
								case "ArmorRating_Blunt":
									animal.Armor_Blunt = value;
									break;
								case "ArmorRating_Heat":
									animal.Armor_Heat = value;
									break;
								case "CaravanRidingSpeedFactor":
									animal.RidingSpeed = value;
									break;
							}
						}

						animal.MeatAmountTotal = StatDefOf.MeatAmount.postProcessCurve.Evaluate(animal.MeatAmount * animal.BaseBodySize);
						animal.LeatherAmountTotal = StatDefOf.LeatherAmount.postProcessCurve.Evaluate(animal.LeatherAmount * animal.BaseBodySize);
					}

					animalist.Add(animal);
				}

				string output = AnimalOutput.HeaderRow() + "\n";
				foreach (var item in animalist)
					output += item.ToString() + "\n";
				//Log.Message(output);
				try
				{
					string outputPath = "Mods\\IngameDataExtractor\\Raw_Animals.csv";
					File.WriteAllText(outputPath, output);
					Log.Message(outputPath);
				}
				catch (Exception exc)
				{
					Log.Error(nameof(AnimalListGenerator) + ": failed writing to file: " + exc.Message);
				}
			}
			else
				Log.Error(nameof(AnimalListGenerator) + ": no animals found; this should not happen!");
		}

		private static float CalculateDPS(List<Tool> tools)
		{
			float highestInitialWeight = 0f;
			int catMidCount = 0, catBestCount = 0;
			List<DamageCalc> allDPSList = new List<DamageCalc>();

			// find highest initial weight
			foreach (var tool in tools)
			{
				float damage = tool.power;
				float cooldown = tool.cooldownTime;
				float armorPenetration = tool.armorPenetration;
				float chanceFactor = tool.chanceFactor;
				float initialWeight = damage * (1f + (armorPenetration < 0f ? damage * 0.015f : armorPenetration)) / cooldown * chanceFactor;
				highestInitialWeight = Math.Max(initialWeight, highestInitialWeight);
			}

			// save each tools details
			foreach (var tool in tools)
			{
				float damage = tool.power;
				float cooldown = tool.cooldownTime;
				float armorPenetration = tool.armorPenetration;
				float chanceFactor = tool.chanceFactor;
				float initialWeight = damage * (1f + (armorPenetration < 0f ? damage * 0.015f : armorPenetration)) / cooldown * chanceFactor;

				int cat;
				// worst
				if (initialWeight < highestInitialWeight * 0.25f)
					continue;
				// mid
				else if (initialWeight < highestInitialWeight * 0.95f)
				{
					cat = 1;
					catMidCount++;
				}
				// best
				else
				{
					cat = 2;
					catBestCount++;
				}

				allDPSList.Add(new DamageCalc
				{
					Damage = damage,
					Cooldown = cooldown,
					ArmorPenetration = armorPenetration,
					ChanceFactor = chanceFactor,
					InitialWeight = initialWeight,
					Cat = cat,
				});
			}

			// calculate weighting factor
			float factorCatMid = 1f / catMidCount * 0.25f;
			float factorCatBest = 1f / catBestCount * 0.75f;
			float factorCatTotal = 0f;
			foreach (var dps in allDPSList)
			{
				switch (dps.Cat)
				{
					case 1:
						dps.FactorCat = factorCatMid;
						break;
					case 2:
						dps.FactorCat = factorCatBest;
						break;
					default:
						continue;
				}
				factorCatTotal += dps.FactorCat;
			}

			float totalDamage = 0, totalCooldown = 0;
			foreach (var dps in allDPSList)
			{
				totalDamage += dps.FactorCat / factorCatTotal * dps.Damage;
				totalCooldown += dps.FactorCat / factorCatTotal * dps.Cooldown;
			}
			return totalDamage / totalCooldown;
		}

		private class DamageCalc
		{
			public float Damage;
			public float Cooldown;
			public float ArmorPenetration;
			public float ChanceFactor;
			public float InitialWeight;
			public int Cat;
			public float FactorCat;
			public float DPS => Damage / Cooldown;
		}
	}
}

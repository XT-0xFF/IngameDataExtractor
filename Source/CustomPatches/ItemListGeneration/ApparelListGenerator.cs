using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using System.IO;

namespace CustomPatches.ItemListGeneration
{
	public class ApparelListGenerator
	{
		public class ApparelOutput
		{
			public string Label { get; set; } = "";
			public string ModName { get; set; } = "";
			public Gender Gender { get; set; } = Gender.None;
			public ApparelType Type { get; set; } = ApparelType.None;
			public List<string> CreatedAt { get; } = new List<string>();
			public QualityFactors Quality { get; set; } = QualityFactors.Normal;
			public ApparelLayer Layers { get; set; } = ApparelLayer.None;
			public ApparelCover Covers { get; set; } = ApparelCover.None;
			public List<char> StuffCategories { get; } = new List<char>();
			public float Mass { get; set; }
			private float _armor_Sharp;
			public float Armor_Sharp
			{
				get => Quality.CalcProtection(_armor_Sharp);
				set => _armor_Sharp = value;
			}
			private float _armor_Blunt;
			public float Armor_Blunt
			{
				get => Quality.CalcProtection(_armor_Blunt);
				set => _armor_Blunt = value;
			}
			private float _armor_Heat;
			public float Armor_Heat
			{
				get => Quality.CalcProtection(_armor_Heat);
				set => _armor_Heat = value;
			}
			private float _insulation_Cold;
			public float Insulation_Cold
			{
				get => Quality.CalcInsulation(_insulation_Cold);
				set => _insulation_Cold = value;
			}
			private float _insulation_Heat;
			public float Insulation_Heat
			{
				get => Quality.CalcInsulation(_insulation_Heat);
				set => _insulation_Heat = value;
			}
			public List<string> EquippedStatOffsets { get; } = new List<string>();

			public ApparelOutput Copy(QualityFactors quality = null)
			{
				ApparelOutput copy = new ApparelOutput
				{
					Label = Label,
					ModName = ModName,
					Gender = Gender,
					Type = Type,
					Quality = quality ?? QualityFactors.Normal,
					Layers = Layers,
					Covers = Covers,
					Mass = Mass,
					_armor_Sharp = _armor_Sharp,
					_armor_Blunt = _armor_Blunt,
					_armor_Heat = _armor_Heat,
					_insulation_Cold = _insulation_Cold,
					_insulation_Heat = _insulation_Heat,
				};
				copy.CreatedAt.AddRange(CreatedAt);
				copy.StuffCategories.AddRange(StuffCategories);
				copy.EquippedStatOffsets.AddRange(EquippedStatOffsets);
				return copy;
			}

			public static string HeaderRow()
			{
				String output = "Apparel;";
				output += "Mod;";
				output += "Type;";
				output += "Gender;";
				output += "Created at;";
				output += "On top;";
				output += "Outer;";
				output += "Middle;";
				output += "Skin;";
				output += "Headgear;";
				output += "Utility;";
				output += "Head;";
				output += "Eyes;";
				output += "Ears;";
				output += "Nose;";
				output += "Jaw;";
				output += "Torso;";
				output += "Neck;";
				output += "Shoulders;";
				output += "Arms;";
				output += "Hands;";
				output += "Fingers;";
				output += "Legs;";
				output += "Feet;";
				output += "Toes;";
				output += "Waist;";
				output += "Mats;";
				output += "Mass;";
				output += "A Sharp;";
				output += "A Blunt;";
				output += "A Heat;";
				output += "I Cold;";
				output += "I Heat;";
				output += "Offsets when equipped;";
				return output;
			}

			public override string ToString()
			{
				// Item
				String output = $"{Label};";

				// Mod Name
				output += $"{ModName};";

				// Type
				List<string> types = new List<string>();
				foreach (ApparelType item in Enum.GetValues(typeof(ApparelType)))
					if (item != ApparelType.None && Type.HasFlag(item))
						types.Add($"{item}");
				output += string.Join(" & ", types) + ";";

				// Gender
				output += $"{Gender};";

				// Created at
				if (CreatedAt.Count > 0)
					output += string.Join(", ", CreatedAt);
				output += ";";

				// Layers
				foreach (ApparelLayer item in Enum.GetValues(typeof(ApparelLayer)))
					if (item != ApparelLayer.None)
						output += Layers.HasFlag(item) ? "x;" : ";";

				// Covers
				foreach (ApparelCover item in Enum.GetValues(typeof(ApparelCover)))
					if (item != ApparelCover.None)
						output += Covers.HasFlag(item) ? "x;" : ";";

				// StuffCategories
				if (StuffCategories.Count > 0)
					output += string.Join(", ", StuffCategories);
				output += ";";

				// Stats
				output += $"{Mass};";
				output += $"{Armor_Sharp};";
				output += $"{Armor_Blunt};";
				output += $"{Armor_Heat};";
				output += $"{Insulation_Cold};";
				output += $"{Insulation_Heat};";

				// EquippedStatOffsets
				if (EquippedStatOffsets.Count > 0)
					output += string.Join(", ", EquippedStatOffsets);
				output += ";";

				return output;
			}
		}

		public static void GenerateList()
		{
			// standard materials
			var steelDef = DefDatabase<ThingDef>.AllDefsListForReading.First(t => t.defName == "Steel");
			var clothDef = DefDatabase<ThingDef>.AllDefsListForReading.First(t => t.defName == "Cloth");
			var plainleatherDef = DefDatabase<ThingDef>.AllDefsListForReading.First(t => t.defName == "Leather_Plain");
			var sandstoneDef = DefDatabase<ThingDef>.AllDefsListForReading.First(t => t.defName == "BlocksSandstone");
			var woodDef = DefDatabase<ThingDef>.AllDefsListForReading.First(t => t.defName == "WoodLog");

			StuffFactors unknown = new StuffFactors();
			StuffFactors steel = new StuffFactors(steelDef);
			StuffFactors cloth = new StuffFactors(clothDef);
			StuffFactors plainleather = new StuffFactors(plainleatherDef);
			StuffFactors stone = new StuffFactors(sandstoneDef);
			StuffFactors wood = new StuffFactors(woodDef);

			// get apparel list of things
			var thingDefs = DefDatabase<ThingDef>.AllDefsListForReading.Where(t => t.thingClass == typeof(Apparel));
			if (thingDefs?.Count() > 0)
			{
				List<ApparelOutput> apparelList = new List<ApparelOutput>();
				foreach (var thing in thingDefs)
				{
					ApparelOutput apparel = new ApparelOutput
					{
						Label = thing.label.CapitalizeFirst(),
						ModName = thing.modContentPack?.Name
					};

					StuffFactors factors = unknown;
					if (thing.stuffCategories != null)
					{
						foreach (var item in thing.stuffCategories)
						{
							var stuff = item.defName;
							if (stuff == "Metallic" 
								|| factors == steel)
								factors = steel;
							else if (stuff == "Fabric" 
								|| factors == cloth)
								factors = cloth;
							else if (stuff == "Leathery"
								|| factors == plainleather)
								factors = plainleather;
							else if (stuff == "Stony" 
								|| factors == stone)
								factors = stone;
							else if (stuff == "Woody" 
								|| factors == wood)
								factors = wood;

							apparel.StuffCategories.Add(stuff.CapitalizeFirst().First());
						}
					}
					if (factors == unknown && apparel.StuffCategories.Count > 0)
						throw new Exception("Missing StuffFactors: " + String.Join(", ", apparel.StuffCategories));
					apparel.StuffCategories.Sort();

					if (thing.recipeMaker?.recipeUsers != null)
					{
						foreach (var item in thing.recipeMaker.recipeUsers)
						{
							var label = item.label.ToLower();
							if (label.Contains("crafting"))
								apparel.CreatedAt.AddDistinct("crafting");
							else if (label.Contains("tailor"))
								apparel.CreatedAt.AddDistinct("tailor");
							else if (label.Contains("smithy"))
								apparel.CreatedAt.AddDistinct("smithy");
							else if (label.Contains("fabrication"))
								apparel.CreatedAt.AddDistinct("fabrication");
							else if (label.Contains("machining"))
								apparel.CreatedAt.AddDistinct("machining");
						}
					}
					apparel.CreatedAt.Sort();

					if (thing.apparel != null)
					{
						if (thing.apparel.layers != null)
						{
							foreach (var item in thing.apparel.layers)
							{
								var layer = item.label.ToLower();
								switch (layer)
								{
									case "on top":
										apparel.Layers |= ApparelLayer.OnTop;
										break;
									case "outer":
										apparel.Layers |= ApparelLayer.Outer;
										break;
									case "middle":
										apparel.Layers |= ApparelLayer.Middle;
										break;
									case "skin":
										apparel.Layers |= ApparelLayer.Skin;
										break;
									case "headgear":
										apparel.Layers |= ApparelLayer.Headgear;
										break;
									case "utility":
										apparel.Layers |= ApparelLayer.Utility;
										break;
									default:
										Log.Warning("Layer missing enum: " + layer);
										break;
								}
							}
						}

						apparel.Gender = thing.apparel.gender;

						if (thing.apparel.bodyPartGroups != null)
						{
							foreach (var item in thing.apparel.bodyPartGroups)
							{
								var cover = item.label.ToLower();
								if (cover == "full head")
								{
									apparel.Covers |= ApparelCover.Head;
									apparel.Covers |= ApparelCover.Eyes;
									apparel.Covers |= ApparelCover.Ears;
									apparel.Covers |= ApparelCover.Nose;
									apparel.Covers |= ApparelCover.Jaw;
								}
								else if (cover == "upper head")
								{
									apparel.Covers |= ApparelCover.Head;
									apparel.Covers |= ApparelCover.Ears;
								}
								else if (cover == "head")
									apparel.Covers |= ApparelCover.Head;
								else if (cover.Contains("eye"))
									apparel.Covers |= ApparelCover.Eyes;
								else if (cover.Contains("ear"))
									apparel.Covers |= ApparelCover.Ears;
								else if (cover == "nose")
									apparel.Covers |= ApparelCover.Nose;
								else if (cover == "teeth")
									apparel.Covers |= ApparelCover.Jaw;
								else if (cover == "torso")
									apparel.Covers |= ApparelCover.Torso;
								else if (cover == "neck")
									apparel.Covers |= ApparelCover.Neck;
								else if (cover.Contains("shoulder"))
									apparel.Covers |= ApparelCover.Shoulders;
								else if (cover.Contains("arm"))
									apparel.Covers |= ApparelCover.Arms;
								else if (cover.Contains("hand"))
								{
									apparel.Covers |= ApparelCover.Hands;
									apparel.Covers |= ApparelCover.Fingers;
								}
								else if (cover.Contains("finger") 
									|| cover.Contains("thumb") 
									|| cover.Contains("pinky"))
									apparel.Covers |= ApparelCover.Fingers;
								else if (cover.Contains("leg"))
									apparel.Covers |= ApparelCover.Legs;
								else if (cover == "feet"
									|| cover.Contains("foot"))
								{
									apparel.Covers |= ApparelCover.Feet;
									apparel.Covers |= ApparelCover.Toes;
								}
								else if (cover.Contains("toe"))
									apparel.Covers |= ApparelCover.Toes;
								else if (cover == "utility slot")
									apparel.Covers |= ApparelCover.Waist;
								else
									Log.Warning("Cover missing enum: " + cover);
							}
						}

						if (apparel.Covers.HasFlag(ApparelCover.Head)
							|| apparel.Covers.HasFlag(ApparelCover.Eyes)
							|| apparel.Covers.HasFlag(ApparelCover.Ears)
							|| apparel.Covers.HasFlag(ApparelCover.Nose)
							|| apparel.Covers.HasFlag(ApparelCover.Jaw))
							apparel.Type |= ApparelType.Head;
						if (apparel.Covers.HasFlag(ApparelCover.Torso)
							|| apparel.Covers.HasFlag(ApparelCover.Shoulders)
							|| apparel.Covers.HasFlag(ApparelCover.Arms)
							|| apparel.Covers.HasFlag(ApparelCover.Neck))
							apparel.Type |= ApparelType.Body;
						if (apparel.Covers.HasFlag(ApparelCover.Hands)
							|| apparel.Covers.HasFlag(ApparelCover.Fingers))
							apparel.Type |= ApparelType.Hands;
						if (apparel.Covers.HasFlag(ApparelCover.Legs))
							apparel.Type |= ApparelType.Legs;
						if (apparel.Covers.HasFlag(ApparelCover.Feet)
							|| apparel.Covers.HasFlag(ApparelCover.Toes))
							apparel.Type |= ApparelType.Feet;
						if (apparel.Covers.HasFlag(ApparelCover.Waist))
							apparel.Type |= ApparelType.Utility;

						if (thing.statBases != null)
						{
							//String debug = apparel.Label + " - " + factors + "\n";
							foreach (var stat in thing.statBases)
							{
								var name = stat.stat.defName;
								var value = stat.value;
								if (name == "Mass")
									apparel.Mass = value;
								else if (name == "StuffEffectMultiplierArmor")
								{
									apparel.Armor_Sharp = value * factors.Armor_Sharp;
									apparel.Armor_Blunt = value * factors.Armor_Blunt;
									apparel.Armor_Heat = value * factors.Armor_Heat;
								}
								else if (name == "ArmorRating_Sharp")
									apparel.Armor_Sharp = value;
								else if (name == "ArmorRating_Blunt")
									apparel.Armor_Blunt = value;
								else if (name == "ArmorRating_Heat")
									apparel.Armor_Heat = value;
								else if (name == "StuffEffectMultiplierInsulation_Cold")
									apparel.Insulation_Cold = value * factors.Insulation_Cold;
								else if (name == "StuffEffectMultiplierInsulation_Heat")
									apparel.Insulation_Heat = value * factors.Insulation_Heat;
								else if (name == "Insulation_Cold")
									apparel.Insulation_Cold = value;
								else if (name == "Insulation_Heat")
									apparel.Insulation_Heat = value;
								//debug += name + " " + value + "\n";
							}
							//Log.Message(debug + apparel.Armor_Sharp + " " + apparel.Armor_Blunt + " " + apparel.Armor_Heat + " " + apparel.Insulation_Cold + " " + apparel.Insulation_Heat);
						}
					}

					if (thing.equippedStatOffsets != null)
						foreach (var offset in thing.equippedStatOffsets)
							apparel.EquippedStatOffsets.Add($"{offset.stat.defName} {offset.value}");
					apparel.EquippedStatOffsets.Sort();

					apparelList.Add(apparel);
				}

				string output = ApparelOutput.HeaderRow() + "\n";
				foreach (var item in apparelList)
					output += item.ToString() + "\n";
				//Log.Message(output);
				try
				{
					string outputPath = "Mods\\IngameDataExtractor\\Raw_Apparel.csv";
					File.WriteAllText(outputPath, output);
					Log.Message(outputPath);
				}
				catch (Exception exc)
				{
					Log.Error(nameof(ApparelListGenerator) + ": failed writing to file: " + exc.Message);
				}
			}
			else
				Log.Error(nameof(ApparelListGenerator) + ": no apparel found; this should not happen!");
		}

		[Flags]
		public enum ApparelType
		{
			None = 0,
			Head = 1,
			Body = 2,
			Hands = 4,
			Legs = 8,
			Feet = 16,
			Utility = 32,
		}

		[Flags]
		public enum ApparelLayer
		{
			None = 0,
			OnTop = 1,
			Outer = 2,
			Middle = 4,
			Skin = 8,
			Headgear = 16,
			Utility = 32,
		}

		[Flags]
		public enum ApparelCover
		{
			None = 0,
			// Head
			Head = 1,
			Eyes = 2,
			Ears = 4,
			Nose = 8,
			Jaw = 16,
			// Body
			Torso = 32,
			Neck = 64,
			// Arms
			Shoulders = 128,
			Arms = 256,
			// Hands
			Hands = 512,
			Fingers = 1024, // pinky, finger, thumb
			// Legs
			Legs = 2048,
			// Feet
			Feet = 4096,
			Toes = 8192,
			// Utility
			Waist = 16384,
		}
	}
}

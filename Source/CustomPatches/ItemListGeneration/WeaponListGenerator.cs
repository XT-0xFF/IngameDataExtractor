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
	public class WeaponListGenerator
	{
		public class WeaponOutput
		{
			public string Label { get; set; } = "";
			public string ModName { get; set; } = "";
			public List<string> CreatedAt { get; } = new List<string>();
			public List<char> StuffCategories { get; } = new List<char>();
			public QualityFactors Quality { get; set; } = QualityFactors.Normal;
			public float Mass { get; set; }
			private float _damagePerShot;
			public float DamagePerShot
			{
				get => Quality.CalcRangedDamage(_damagePerShot);
				set => _damagePerShot = value;
			}
			public float StoppingPower { get; set; }
			public float PenetrationInPercent { get; set; }
			public float RoundsPerMinute { get; set; } = 1;
			public float BurstCount { get; set; } = 1;
			public float Range { get; set; }
			public float _accuracy_Close;
			public float Accuracy_Close
			{
				get => Quality.CalcRangedAccuracy(_accuracy_Close);
				set => _accuracy_Close = value;
			}
			public float _accuracy_Short;
			public float Accuracy_Short
			{
				get => Quality.CalcRangedAccuracy(_accuracy_Short);
				set => _accuracy_Short = value;
			}
			public float _accuracy_Medium;
			public float Accuracy_Medium
			{
				get => Quality.CalcRangedAccuracy(_accuracy_Medium);
				set => _accuracy_Medium = value;
			}
			public float _accuracy_Long;
			public float Accuracy_Long
			{
				get => Quality.CalcRangedAccuracy(_accuracy_Long);
				set => _accuracy_Long = value;
			}
			public float CooldownInSeconds { get; set; }
			public float WarmupInSeconds { get; set; }
			public List<string> EquippedStatOffsets { get; } = new List<string>();

			public float DamagePerSecond_Max => (DamagePerShot * BurstCount) / (((BurstCount - 1.0f) * 60.0f / RoundsPerMinute) + CooldownInSeconds + WarmupInSeconds);
			public float DamagePerSecond_Close => DamagePerSecond_Max * Accuracy_Close;
			public float DamagePerSecond_Short => DamagePerSecond_Max * Accuracy_Short;
			public float DamagePerSecond_Medium => DamagePerSecond_Max * Accuracy_Medium;
			public float DamagePerSecond_Long => DamagePerSecond_Max * Accuracy_Long;

			public WeaponOutput Copy(QualityFactors quality = null)
			{
				WeaponOutput copy = new WeaponOutput
				{
					Label = Label,
					ModName = ModName,
					Quality = quality ?? QualityFactors.Normal,
					Mass = Mass,
					_damagePerShot = _damagePerShot,
					StoppingPower = StoppingPower,
					PenetrationInPercent = PenetrationInPercent,
					RoundsPerMinute = RoundsPerMinute,
					BurstCount = BurstCount,
					Range = Range,
					_accuracy_Close = _accuracy_Close,
					_accuracy_Short = _accuracy_Short,
					_accuracy_Medium = _accuracy_Medium,
					_accuracy_Long = _accuracy_Long,
					CooldownInSeconds = CooldownInSeconds,
					WarmupInSeconds = WarmupInSeconds,
				};
				copy.CreatedAt.AddRange(CreatedAt);
				copy.StuffCategories.AddRange(StuffCategories);
				copy.EquippedStatOffsets.AddRange(EquippedStatOffsets);
				return copy;
			}

			public static string HeaderRow()
			{
				String output = "Weapon;";
				output += "Mod;";
				output += "Created at;";
				output += "Mats;";
				output += "Quality;";
				output += "Mass;";
				output += "Damage per Shot;";
				output += "Stopping Power;";
				output += "Armor Penetration;";
				output += "RPM;";
				output += "Burst Count;";
				output += "Range;";
				output += "Accuracy Close;";
				output += "Accuracy Short;";
				output += "Accuracy Medium;";
				output += "Accuracy Long;";
				output += "Cooldown;";
				output += "Warmup;";
				output += "DPS Theoretical;";
				output += "DPS Close;";
				output += "DPS Short;";
				output += "DPS Medium;";
				output += "DPS Long;";
				output += "Offsets when equipped;";
				return output;
			}

			public override string ToString()
			{
				// Item
				String output = $"{Label};";

				// Mod Name
				output += $"{ModName};";

				// Created at
				if (CreatedAt.Count > 0)
					output += string.Join(", ", CreatedAt);
				output += ";";

				// StuffCategories
				if (StuffCategories.Count > 0)
					output += string.Join(", ", StuffCategories);
				output += ";";

				// Quality
				output += $"{Quality};";

				// Stats
				output += $"{Mass};";
				output += $"{DamagePerShot};";
				output += $"{StoppingPower};";
				output += $"{PenetrationInPercent};";
				output += $"{RoundsPerMinute};";
				output += $"{BurstCount};";
				output += $"{Range};";
				output += $"{Accuracy_Close};";
				output += $"{Accuracy_Short};";
				output += $"{Accuracy_Medium};";
				output += $"{Accuracy_Long};";
				output += $"{CooldownInSeconds};";
				output += $"{WarmupInSeconds};";
				output += $"{DamagePerSecond_Max};";
				output += $"{DamagePerSecond_Close};";
				output += $"{DamagePerSecond_Short};";
				output += $"{DamagePerSecond_Medium};";
				output += $"{DamagePerSecond_Long};";

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
			var sandstoneDef = DefDatabase<ThingDef>.AllDefsListForReading.First(t => t.defName == "BlocksSandstone");
			var woodDef = DefDatabase<ThingDef>.AllDefsListForReading.First(t => t.defName == "WoodLog");

			StuffFactors unknown = new StuffFactors();
			StuffFactors steel = new StuffFactors(steelDef);
			StuffFactors stone = new StuffFactors(sandstoneDef);
			StuffFactors wood = new StuffFactors(woodDef);

			// get weapon list of things
			List<WeaponOutput> weaponList = new List<WeaponOutput>();
			var thingDefs = DefDatabase<ThingDef>.AllDefsListForReading.Where(t => t.thingCategories?.Where((x) => x.defName.Contains("WeaponsRanged"))?.Count() > 0);
			if (thingDefs?.Count() > 0)
			{
				foreach (var thing in thingDefs)
				{
					WeaponOutput weapon = new WeaponOutput
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
							else if (stuff == "Stony"
								|| factors == stone)
								factors = stone;
							else if (stuff == "Woody"
								|| factors == wood)
								factors = wood;

							weapon.StuffCategories.Add(stuff.CapitalizeFirst().First());
						}
					}
					if (factors == unknown && weapon.StuffCategories.Count > 0)
						throw new Exception("Missing StuffFactors: " + String.Join(", ", weapon.StuffCategories));
					weapon.StuffCategories.Sort();

					if (thing.recipeMaker?.recipeUsers != null)
					{
						foreach (var item in thing.recipeMaker.recipeUsers)
						{
							var label = item.label.ToLower();
							if (label.Contains("crafting"))
								weapon.CreatedAt.AddDistinct("crafting");
							else if (label.Contains("tailor"))
								weapon.CreatedAt.AddDistinct("tailor");
							else if (label.Contains("smithy"))
								weapon.CreatedAt.AddDistinct("smithy");
							else if (label.Contains("fabrication"))
								weapon.CreatedAt.AddDistinct("fabrication");
							else if (label.Contains("machining"))
								weapon.CreatedAt.AddDistinct("machining");
						}
					}
					weapon.CreatedAt.Sort();

					bool canShoot = false;
					if (thing.Verbs != null)
					{
						foreach (var verb in thing.Verbs)
						{
							if (verb.verbClass == typeof(Verb_Shoot) || verb.verbClass.ToString() == "SmokingGun.Verb_ShootWithSmoke")
							{
								canShoot = true;
								weapon.WarmupInSeconds = verb.warmupTime;
								weapon.Range = verb.range;
								weapon.BurstCount = verb.burstShotCount;
								weapon.RoundsPerMinute = verb.ticksBetweenBurstShots > 0 ? 3600.0f / verb.ticksBetweenBurstShots : verb.ticksBetweenBurstShots == 0 ? float.PositiveInfinity : 1;

								var projectile = verb.defaultProjectile?.projectile;
								if (projectile != null)
								{
									weapon.DamagePerShot = projectile.GetDamageAmount(1);
									weapon.StoppingPower = projectile.StoppingPower;
									weapon.PenetrationInPercent = projectile.GetArmorPenetration(1);
								}
								else
									throw new Exception(weapon.Label + " lacks projectile?");
							}
						}
					}
					if (!canShoot)
					{
						Log.Message(weapon.Label + " does not have 'Verb_Shoot', skipping");
						continue;
					}

					if (thing.statBases != null)
					{
						foreach (var stat in thing.statBases)
						{
							var name = stat.stat.defName;
							var value = stat.value;
							if (name == "Mass")
								weapon.Mass = value;
							else if (name == "AccuracyTouch")
								weapon.Accuracy_Close = value;
							else if (name == "AccuracyShort")
								weapon.Accuracy_Short = value;
							else if (name == "AccuracyMedium")
								weapon.Accuracy_Medium = value;
							else if (name == "AccuracyLong")
								weapon.Accuracy_Long = value;
							else if (name == "RangedWeapon_Cooldown")
								weapon.CooldownInSeconds = value;
						}
					}

					if (thing.equippedStatOffsets != null)
						foreach (var offset in thing.equippedStatOffsets)
							weapon.EquippedStatOffsets.Add($"{offset.stat.defName} {offset.value}");
					weapon.EquippedStatOffsets.Sort();
					
					weaponList.Add(weapon.Copy(QualityFactors.Normal));
					weaponList.Add(weapon.Copy(QualityFactors.Good));
					weaponList.Add(weapon.Copy(QualityFactors.Excellent));
					weaponList.Add(weapon.Copy(QualityFactors.Masterwork));
					weaponList.Add(weapon.Copy(QualityFactors.Legendary));
				}

				string output = WeaponOutput.HeaderRow() + "\n";
				foreach (var item in weaponList)
					output += item.ToString() + "\n";
				//Log.Message(output);
				try
				{
					string outputPath = "Mods\\IngameDataExtractor\\Raw_Weapons.csv";
					File.WriteAllText(outputPath, output);
					Log.Message(outputPath);
				}
				catch (Exception exc)
				{
					Log.Error(nameof(WeaponListGenerator) + ": failed writing to file: " + exc.Message);
				}
			}
			else
				Log.Error(nameof(WeaponListGenerator) + ": no weapon found; this should not happen!");
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using System.Reflection;
using RimWorld;
using System.IO;

namespace CustomPatches
{
	public class CustomPatches : Mod
	{
		public static string AssemblyDirectory
		{
			get
			{
				string codeBase = Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uri.Path);
				return Path.GetDirectoryName(path);
			}
		}

		public CustomPatches(ModContentPack content) : base(content)
		{
			Log.Message("INFO: CustomPatches assembly is active!");
		}

		//CustomPatchesSettings settings;
		//settings = GetSettings<CustomPatchesSettings>();

		//public override void DoSettingsWindowContents(Rect inRect)
		//{
		//	Listing_Standard listingStandard = new Listing_Standard();
		//	listingStandard.Begin(inRect);
		//	listingStandard.CheckboxLabeled("exampleBoolExplanation", ref settings.exampleBool, "exampleBoolToolTip");
		//	listingStandard.Label("exampleFloatExplanation");
		//	settings.exampleFloat = listingStandard.Slider(settings.exampleFloat, 100f, 300f);
		//	listingStandard.End();
		//	base.DoSettingsWindowContents(inRect);
		//}

		//public override string SettingsCategory()
		//{
		//	return "Custom Patches".Translate();
		//}

		//public static void SetSettings()
		//{
		//	var test = DefDatabase<ThingDef>.AllDefsListForReading;

		//	foreach (var item in test)
		//	{
		//		if (item.GetType() == typeof(RaidStrategyDef))
		//			Log.Message("CustomPatches: byType Type " + item.GetType() + " Name " + item.defName);
		//		else if (item.defName == "ImmediateAttackSappers")
		//			Log.Message("CustomPatches: byName Type " + item.GetType() + " Name " + item.defName);
		//	}
		//	Log.Message("CustomPatches: Test ");


		//	//FieldInfo DefsByNameFI = typeof(DefDatabase<RaidStrategyDef>).GetField("defsByName", BindingFlags.Static | BindingFlags.NonPublic);
		//	//if ((DefsByNameFI.GetValue(null) as Dictionary<string, D>).TryGetValue(defName, out def))
		//	//	return true;
		//	//Log.Error($"Unable to find def [{defName}]");
		//}
	}

	//public class CustomPatchesSettings : ModSettings
	//{
	//	public bool exampleBool;
	//	public float exampleFloat = 200f;

	//	public override void ExposeData()
	//	{
	//		Scribe_Values.Look(ref exampleBool, "exampleBool");
	//		Scribe_Values.Look(ref exampleFloat, "exampleFloat", 200f);
	//		base.ExposeData();
	//	}
	//}
}

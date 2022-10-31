using System;
using System.Reflection;
using RimWorld;
using Verse;
using HarmonyLib;
using CustomPatches.ItemListGeneration;
using System.Collections.Generic;
using System.Linq;

namespace CustomPatches
{
	[StaticConstructorOnStartup]
	class HarmonyPatches
	{
		static HarmonyPatches()
		{
			var harmony = new Harmony("customPatches.mod");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
	}

	//[HarmonyPatch(typeof(TradeShip), "ColonyThingsWillingToBuy")]
	//static class Patch_PassingShip_TryOpenComms
	//{
	//	// Before an orbital trade
	//	static void Postfix(ref IEnumerable<Thing> __result, Pawn playerNegotiator)
	//	{
	//		List<Thing> things = null;
	//		if (playerNegotiator != null && playerNegotiator.Map != null)
	//		{
	//			foreach (Thing thing in playerNegotiator.Map.listerBuildings.allBuildingsColonist)
	//			{
	//				if (thing is RimFridge_Building storage)
	//				{
	//					//var storage = thing as Building_Storage;
	//					foreach (IntVec3 cell in storage.AllSlotCells())
	//					{
	//						foreach (Thing refrigeratedItem in playerNegotiator.Map.thingGrid.ThingsAt(cell))
	//						{
	//							if (storage.settings.AllowedToAccept(refrigeratedItem))
	//							{
	//								if (things == null)
	//								{
	//									if (__result?.Count() == 0)
	//										things = new List<Thing>();
	//									else
	//										things = new List<Thing>(__result);
	//								}
	//								things.Add(refrigeratedItem);
	//								break;
	//							}
	//						}
	//					}
	//				}
	//			}
	//		}
	//		if (things != null)
	//			__result = things;
	//	}
	//}

	[HarmonyPatch(typeof(Page_SelectScenario), "BeginScenarioConfiguration")]
	static class Patch_Page_SelectScenario_BeginScenarioConfiguration
	{
		[HarmonyPriority(Priority.First)]
		static void Postfix()
		{
			try
			{
				ApparelListGenerator.GenerateList();
				WeaponListGenerator.GenerateList();
				PlantListGenerator.GenerateList();
				AnimalListGenerator.GenerateList();
				LeatherAndClothGenerator.GenerateList();
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
		}
	}

	//[HarmonyPatch(typeof(SavedGameLoaderNow), "LoadGameFromSaveFileNow")]
	//static class Patch_SavedGameLoaderNow_LoadGameFromSaveFileNow
	//{
	//	[HarmonyPriority(Priority.First)]
	//	static void Prefix()
	//	{
	//		CustomPatches.SetSettings();
	//	}
	//}

	//[HarmonyPatch(typeof(Root_Play), "SetupForQuickTestPlay")]
	//static class Patch_Root_Play_SetupForQuickTestPlay
	//{
	//	[HarmonyPriority(Priority.First)]
	//	static void Prefix()
	//	{
	//		CustomPatches.SetSettings();
	//	}
	//}
}

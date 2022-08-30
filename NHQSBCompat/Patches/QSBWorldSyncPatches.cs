using HarmonyLib;
using QSB.WorldSync;
using System.Collections.Generic;
using System.Linq;

namespace NHQSBCompat.Patches;

[HarmonyPatch]
public static class QSBWorldSyncPatches
{
	private static readonly string[] ManagersToRemove =
	{
		"OrbManager",
		"OccasionalManager",
		"RaftManager",
		"DreamRaftManager",
		"ItemManager",
		"ModelShipManager"
	};

	/*
	[HarmonyPrefix]
	[HarmonyPatch(typeof(QSBWorldSync), nameof(QSBWorldSync.BuildWorldObjects))]
	private static void QSBWorldSync_BuildWorldObjects()
	{
		if (Main.Instance.NewHorizonsAPI.GetCurrentStarSystem() != "SolarSystem")
		{
			List<WorldObjectManager> managers = QSBWorldSync.Managers.ToList();
			managers.RemoveAll(m => ManagersToRemove.Contains(m.ToString()));
			QSBWorldSync.Managers = managers.ToArray();
		}
	}
*/
}

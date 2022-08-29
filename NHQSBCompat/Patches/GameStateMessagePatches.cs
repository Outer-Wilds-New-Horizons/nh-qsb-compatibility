using HarmonyLib;
using Mirror;
using NHQSBCompat.Managers;
using QSB.Patches;
using QSB.SaveSync.Messages;

namespace NHQSBCompat.Patches;

[HarmonyPatch]
internal class GameStateMessagePatches
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(GameStateMessage), nameof(GameStateMessage.Serialize))]
	public static void GameStateMessage_Serialize(GameStateMessage __instance, NetworkWriter writer) =>
		writer.Write(Main.Instance.NewHorizonsAPI.GetCurrentStarSystem());

	[HarmonyPostfix]
	[HarmonyPatch(typeof(GameStateMessage), nameof(GameStateMessage.Deserialize))]
	public static void GameStateMessage_Deserialize(GameStateMessage __instance, NetworkReader reader) =>
		WarpManager.Instance.RemoteChangeStarSystem(reader.Read<string>());
}

using HarmonyLib;
using Mirror;
using NHQSBCompat.Managers;
using QSB;
using QSB.Patches;
using QSB.Player;
using QSB.SaveSync.Messages;

namespace NHQSBCompat.Patches;

[HarmonyPatch]
internal class GameStateMessagePatches
{
	private static string _initialSystem;

	[HarmonyPostfix]
	[HarmonyPatch(typeof(GameStateMessage), nameof(GameStateMessage.Serialize))]
	public static void GameStateMessage_Serialize(GameStateMessage __instance, NetworkWriter writer) =>
		writer.Write(Main.Instance.NewHorizonsAPI.GetCurrentStarSystem());

	[HarmonyPostfix]
	[HarmonyPatch(typeof(GameStateMessage), nameof(GameStateMessage.Deserialize))]
	public static void GameStateMessage_Deserialize(GameStateMessage __instance, NetworkReader reader) =>
		_initialSystem = reader.Read<string>();

	[HarmonyPostfix]
	[HarmonyPatch(typeof(GameStateMessage), nameof(GameStateMessage.OnReceiveRemote))]
	public static void GameStateMessage_OnReceiveRemote()
	{
		if (QSBCore.IsHost)
		{
			Main.Log($"Why is the host being given the initial state info?");
		}
		else
		{
			Main.Log($"Player#{QSBPlayerManager.LocalPlayerId} is being sent to {_initialSystem}");
			WarpManager.Instance.RemoteChangeStarSystem(_initialSystem);
		}
	}

}

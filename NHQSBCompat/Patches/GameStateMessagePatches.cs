using HarmonyLib;
using Mirror;
using NHQSBCompat.Managers;
using QSB;
using QSB.Player;
using QSB.SaveSync.Messages;

namespace NHQSBCompat.Patches;

[HarmonyPatch]
internal class GameStateMessagePatches
{
	private static string _initialSystem;
	private static int[] _hostAddonHash;

	[HarmonyPostfix]
	[HarmonyPatch(typeof(GameStateMessage), nameof(GameStateMessage.Serialize))]
	public static void GameStateMessage_Serialize(GameStateMessage __instance, NetworkWriter writer)
	{
		var currentSystem = Main.Instance.NewHorizonsAPI.GetCurrentStarSystem();

		writer.Write(currentSystem);
		writer.WriteArray(Main.HashAddonsForSystem(currentSystem));
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(GameStateMessage), nameof(GameStateMessage.Deserialize))]
	public static void GameStateMessage_Deserialize(GameStateMessage __instance, NetworkReader reader)
	{
		_initialSystem = reader.Read<string>();
		_hostAddonHash = reader.ReadArray<int>();
	}

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

			WarpManager.RemoteChangeStarSystem(_initialSystem, false, false, _hostAddonHash);
		}
	}
}
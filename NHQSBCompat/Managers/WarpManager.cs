using HarmonyLib;
using Mirror;
using QSB;
using QSB.Menus;
using QSB.Messaging;
using QSB.Player;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace NHQSBCompat.Managers;

public static class WarpManager
{
    internal static bool RemoteWarp = false;

    private static void Kick(string reason)
    {
		Main.Log(reason);
		MenuManager.Instance.OnKicked(reason);
		NetworkClient.Disconnect();
	}

    public static void RemoteChangeStarSystem(string system, bool ship, bool vessel, int[] hostAddonHash)
    {
        // Flag to not send a message
        RemoteWarp = true;

		Main.Log($"Remote request received to go to {system}");

		if (!NewHorizons.Main.SystemDict.ContainsKey(system))
		{
            // If you can't go to that system then you have to be disconnected
            Kick($"You don't have the mod installed for {system}");
        }
        else
        {
            var localHash = Main.HashAddonsForSystem(system);
            if (localHash != hostAddonHash)
            {
                var missingAddonHashes = hostAddonHash.Except(localHash);
                var extraAddonHashes = localHash.Except(hostAddonHash);

                if (missingAddonHashes.Count() > 0)
                {
                    Kick($"You are missing {missingAddonHashes.Count()} addon(s) that effect {system}");
                    return;
                }

                if (extraAddonHashes.Count() > 0)
                {
                    var extraMods = extraAddonHashes.Select(x => Main.HashToMod(x));

                    // TODO: Disable these mods for the client and do not kick them

					Kick($"You have {extraAddonHashes.Count()} extra addon(s) that effect {system}. Check the logs.");
                    Main.Log($"You have mods affecting {system} that the host does not: {string.Join(", ", extraMods)}");
					return;
				}
            }

            NewHorizons.Main.Instance.ChangeCurrentStarSystem(system, ship, vessel);
		}
    }

	public class NHWarpMessage : QSBMessage
	{
        private string _starSystem;
        private bool _shipWarp;
        private bool _vesselWarp;

		public NHWarpMessage(string starSystem, bool shipWarp, bool vesselWarp) : base() 
        {
			_starSystem = starSystem;
			_shipWarp = shipWarp;
			_vesselWarp = vesselWarp;
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);

            writer.Write(_starSystem);
            writer.Write(_shipWarp);
            writer.Write(_vesselWarp);
        }

		public override void Deserialize(NetworkReader reader)
		{
			base.Deserialize(reader);

			_starSystem = reader.Read<string>();
			_shipWarp = reader.Read<bool>();
			_vesselWarp = reader.Read<bool>();
		}

		public override void OnReceiveRemote()
        {
            Main.Log($"Player#{From} is telling Player#{To} to warp to {_starSystem}");
            if (QSBCore.IsHost && !NewHorizons.Main.SystemDict.ContainsKey(_starSystem))
            {
                // If the host doesn't have that system then we can't
                Main.Log($"The host doesn't have {_starSystem} installed: aborting");
			}
            else
            {
                if (QSBCore.IsHost)
				{
					new NHWarpMessage(_starSystem, _shipWarp, _vesselWarp).Send();
				}

				RemoteChangeStarSystem(_starSystem, _shipWarp, _vesselWarp, Main.HashAddonsForSystem(_starSystem));
			}
		}
	}

    [HarmonyPatch]
    public class NHWarpPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NewHorizons.Main), nameof(NewHorizons.Main.ChangeCurrentStarSystem))]
        public static bool NewHorizons_ChangeCurrentStarSystem(string newStarSystem, bool warp, bool vessel)
        {
            if (RemoteWarp)
            {
                // We're being told to warp so just do it
                RemoteWarp = false;
                return true;
            }

            Main.Log($"Local request received to go to {newStarSystem}");
            if (QSBCore.IsHost)
            {
                // The host will tell all other users to warp
                Main.Log($"Host: Telling others to go to {newStarSystem}");
                new NHWarpMessage(newStarSystem, warp, vessel).Send();
                // The host can now warp 
                return true;
            }
            else
            {
                // We're a client that has to tell the host to start warping people
                Main.Log($"Client: Telling host to send us to {newStarSystem}");
                new NHWarpMessage(newStarSystem, warp, vessel) { To = 0 }.Send();

                // We have to wait for the host to get back to us
                return false;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(NewHorizons.Main), nameof(NewHorizons.Main.ChangeCurrentStarSystem))]
        public static void NewHorizons_ChangeCurrentStarSystem(NewHorizons.Main __instance)
        {
            if (__instance.IsWarpingFromShip)
            {
                // If QSB doesn't say we're piloting the ship then dont keep them on as the one warping
                __instance.GetType().GetProperty(nameof(NewHorizons.Main.IsWarpingFromShip)).SetValue(__instance, QSBPlayerManager.LocalPlayer.FlyingShip);
            }
        }
    }
}
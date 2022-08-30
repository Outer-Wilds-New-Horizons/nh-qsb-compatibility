using HarmonyLib;
using Mirror;
using QSB;
using QSB.Menus;
using QSB.Messaging;
using QSB.Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace NHQSBCompat.Managers;

public static class WarpManager
{
    internal static bool RemoteWarp = false;

    public static void RemoteChangeStarSystem(string system, bool ship, bool vessel)
    {
        // Flag to not send a message
        RemoteWarp = true;

		if (!NewHorizons.Main.SystemDict.ContainsKey(system))
		{
            // If you can't go to that system then you have to be disconnected
            var msg = $"You don't have the mod installed for {system}";
            Main.Log(msg);
			MenuManager.Instance.OnKicked(msg);
            NetworkClient.Disconnect();
        }
        else
        {
            Main.Log($"Remote request received to go to {system}");
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
                if (QSBCore.IsHost) new NHWarpMessage(_starSystem, _shipWarp, _vesselWarp).Send();
				RemoteChangeStarSystem(_starSystem, _shipWarp, _vesselWarp);
			}
		}
	}

    [HarmonyPatch]
    public class NHWarpPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NewHorizons.Main), nameof(NewHorizons.Main.ChangeCurrentStarSystem))]
        public static bool NewHorizons_ChangeCurrentStartSystem(string newStarSystem, bool warp, bool vessel)
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
				new NHWarpMessage(newStarSystem, false, vessel).Send();
                // The host can now warp 
                return true;
			}
            else
            {
				// We're a client that has to tell the host to start warping people
				Main.Log($"Client: Telling host to send us to {newStarSystem}");
				new NHWarpMessage(newStarSystem, false, vessel) { To = 0 }.Send();

                // We have to wait for the host to get back to us
                return false;
			}
        }
    }
}



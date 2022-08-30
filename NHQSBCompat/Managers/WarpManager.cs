using HarmonyLib;
using Mirror;
using QSB;
using QSB.Menus;
using QSB.Messaging;
using QSB.Player;
using UnityEngine;

namespace NHQSBCompat.Managers;

public class WarpManager : MonoBehaviour
{
    public static WarpManager Instance;

    internal static bool RemoteWarp = false;

    public void RemoteChangeStarSystem(string system, bool ship, bool vessel)
    {
        // Flag to not send a message
        RemoteWarp = true;

		if (!NewHorizons.Main.SystemDict.ContainsKey(system))
		{
            // If you can't go to that system then you have to be disconnected
            MenuManager.Instance.OnKicked($"You don't have the mod installed for {system}");
            NetworkClient.Disconnect();
        }
        else
        {
            NewHorizons.Main.Instance.ChangeCurrentStarSystem(system, ship, vessel);
		}
    }

	public class NHWarpMessage : QSBMessage<(string starSystem, bool shipWarp, bool vesselWarp)>
	{
		public NHWarpMessage(string starSystem, bool shipWarp, bool vesselWarp) : base((starSystem, shipWarp, vesselWarp)) { }

        public override void OnReceiveRemote()
        {
            Main.Log($"Player#{From} is telling Player#{To} to warp to {Data.starSystem}");
            if (QSBCore.IsHost)
            {
				if (!NewHorizons.Main.SystemDict.ContainsKey(Data.starSystem))
                {
                    // If the host doesn't have that system then we can't
                    Main.Log($"The host doesn't have {Data.starSystem} installed: aborting");
                }
                else
                {
					// The host will tell all other users to warp
					new NHWarpMessage(Data.starSystem, Data.shipWarp, Data.vesselWarp).Send();
				}
			}
            else
            {
                if (From == 0)
                {
					// Clients being told to warp by the host
					Instance.RemoteChangeStarSystem(Data.starSystem, Data.shipWarp, Data.vesselWarp);
				}
                else
                {
                    Main.Log("A client tried to tell another client to warp (has to go through host -> this shouldn't happen)");
                }
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

			if (QSBCore.IsHost)
            {
				// The host will tell all other users to warp
				new NHWarpMessage(newStarSystem, warp, vessel).Send();
                // The host can now warp 
                return true;
			}
            else
            {
				// We're a client that has to tell the host to start warping people
				new NHWarpMessage(newStarSystem, warp, vessel) { To = 0 }.Send();

                // We have to wait for the host to get back to us
                return false;
			}
        }
    }
}



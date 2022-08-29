using Mirror;
using QSB.Menus;
using QSB.Messaging;
using UnityEngine;

namespace NHQSBCompat.Managers;

public class WarpManager : MonoBehaviour
{
    public static WarpManager Instance;

    public bool RemoteWarp = false;

    public void Awake()
    {
        Instance = this;

        Main.Instance.NewHorizonsAPI.GetChangeStarSystemEvent().AddListener(OnChangeStarSystem);
    }

    public void OnDestroy()
    {
        Main.Instance.NewHorizonsAPI.GetChangeStarSystemEvent()?.RemoveListener(OnChangeStarSystem);
    }

    private void OnChangeStarSystem(string system)
    {
        if (RemoteWarp)
        {
            RemoteWarp = false;
        }
        else
        {
			new NHWarpMessage(system).Send();
        }
	}

    public void RemoteChangeStarSystem(string system)
    {
        // Flag to not send a message
        RemoteWarp = true;

        if (!Main.Instance.NewHorizonsAPI.ChangeCurrentStarSystem(system))
        {
            // If you can't go to that system then you have to be disconnected
            MenuManager.Instance.OnKicked($"You don't have the mod installed for {system}");
            NetworkClient.Disconnect();
        }
    }

	public class NHWarpMessage : QSBMessage<string>
	{
		public NHWarpMessage(string data) : base(data) { }

        public override void OnReceiveRemote()
        {
            Main.Log($"Player#{From} is telling Player#{To} to warp to {Data}");
            Instance.RemoteChangeStarSystem(Data);
        }
	}
}



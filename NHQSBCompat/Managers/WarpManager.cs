using Mirror;
using QSB.Menus;
using QSB.Messaging;
using UnityEngine;

namespace NHQSBCompat.Managers;

public class WarpManager : MonoBehaviour
{
    public static WarpManager Instance;

    public bool IsWarping = false;

    public void Awake()
    {
        Instance = this;

        Main.Instance.NewHorizonsAPI.GetChangeStarSystemEvent().AddListener(OnChangeStarSystem);
        LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;
    }

    public void OnDestroy()
    {
        Main.Instance.NewHorizonsAPI.GetChangeStarSystemEvent()?.RemoveListener(OnChangeStarSystem);
        LoadManager.OnCompleteSceneLoad -= OnCompleteSceneLoad;
    }

    private void OnCompleteSceneLoad(OWScene _, OWScene loadScene)
    {
        if (loadScene != OWScene.SolarSystem) return;

        // If we were changing systems then we just arrived
        IsWarping = false;
    }

    private void OnChangeStarSystem(string system)
    {
        // Only send the message if we weren't already changing
        if (!IsWarping)
        {
            new NHWarpMessage(system).Send();
        }
        IsWarping = true;
    }

    public void RemoteChangeStarSystem(string system)
    {
        // Want to make sure that this one doesn't start sending messages too
        IsWarping = true;
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

		public override void OnReceiveRemote() => Instance.RemoteChangeStarSystem(Data);
	}
}



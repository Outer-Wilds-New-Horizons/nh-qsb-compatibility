using OWML.Common;
using OWML.ModHelper;
using QSB.Messaging;

namespace NHQSBCompat
{
    public class Main : ModBehaviour
    {
        public static Main Instance;

        public INewHorizons NewHorizonsAPI;
        public bool IsChangingSystem = false;

        public void Start()
        {
            Instance = this;

            ModHelper.Console.WriteLine($"My mod {nameof(Main)} is loaded!", MessageType.Success);

            NewHorizonsAPI = ModHelper.Interaction.GetModApi<INewHorizons>("xen.NewHorizons");
            NewHorizonsAPI.GetChangeStarSystemEvent().AddListener(OnChangeStarSystem);

            QSBPatcher.Apply();

            LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;
        }

        public void OnDestroy()
        {
            LoadManager.OnCompleteSceneLoad -= OnCompleteSceneLoad;
            NewHorizonsAPI?.GetChangeStarSystemEvent()?.RemoveListener(OnChangeStarSystem);
        }

        public void OnCompleteSceneLoad(OWScene _, OWScene loadScene)
        {
            if (loadScene != OWScene.SolarSystem) return;

            // If we were changing systems then we just arrived
            IsChangingSystem = false;
        }

        public void OnChangeStarSystem(string system)
        {
            // Only send the message if we weren't already changing
            if(!IsChangingSystem)
            {
                new NHWarpMessage(system).Send();
            }
            IsChangingSystem = true;
        }

        public void RemoteChangeStarSystem(string system)
        {
            // Want to make sure that this one doesn't start sending messages too
            IsChangingSystem = true;
            NewHorizonsAPI.ChangeCurrentStarSystem(system);
        }
    }
}

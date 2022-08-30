using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using QSB.Messaging;
using System.Reflection;
using QSB;
using QSB.Menus;
using Mirror;
using NHQSBCompat.Managers;

namespace NHQSBCompat
{
    public class Main : ModBehaviour
    {
        public static Main Instance;

        public INewHorizons NewHorizonsAPI;

        public void Start()
        {
			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

			Instance = this;

            NewHorizonsAPI = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");

            gameObject.AddComponent<WarpManager>();
            gameObject.AddComponent<DestructionManager>();
        }

        public static void Log(string msg) => Instance.ModHelper.Console.WriteLine(msg);
	}
}

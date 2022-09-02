using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using QSB.Messaging;
using System.Reflection;
using QSB;
using QSB.Menus;
using Mirror;
using NHQSBCompat.Managers;
using System.Linq;
using System.Collections.Generic;

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


        }

        public static void Log(string msg) => Instance.ModHelper.Console.WriteLine(msg);

		public static string HashToMod(int hash)
		{
			foreach (var mod in NewHorizons.Main.MountedAddons)
			{
				var name = mod.ModHelper.Manifest.UniqueName;
				if (name.GetStableHashCode() == hash) return name;
			}
			return null;
		}

		public static int[] HashAddonsForSystem(string system)
		{
			if (NewHorizons.Main.BodyDict.TryGetValue(system, out var bodies))
			{
				var addonHashes = bodies
					.Where(x => x.Mod.ModHelper.Manifest.UniqueName != "xen.NewHorizons")
					.Select(x => x.Mod.ModHelper.Manifest.UniqueName.GetStableHashCode())
					.Distinct();

				var nhPlanetHashes = bodies
					.Where(x => x.Mod.ModHelper.Manifest.UniqueName == "xen.NewHorizons")
					.Select(x => x.Config.name.GetStableHashCode());

				return addonHashes.Concat(nhPlanetHashes).ToArray();
			}
			else
			{
				return null;
			}
		}
	}
}

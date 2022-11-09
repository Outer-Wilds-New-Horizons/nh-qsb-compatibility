using HarmonyLib;
using OWML.ModHelper;
using System.Reflection;
using Mirror;
using NewHorizons;
using System.Linq;

namespace NHQSBCompat;

public class Main : ModBehaviour
{
	public static Main Instance;

	public INewHorizons NewHorizonsAPI;

	public void Start()
	{
		Instance = this;
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
		NewHorizonsAPI = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
	}

	public static void Log(string msg) => Instance.ModHelper.Console.WriteLine(msg);

	public static string HashToMod(int hash)
	{
		foreach (var mod in NewHorizons.Main.MountedAddons)
		{
			var name = mod.ModHelper.Manifest.UniqueName;
			if (name.GetStableHashCode() == hash)
			{
				return name;
			}
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

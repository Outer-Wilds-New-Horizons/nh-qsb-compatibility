using UnityEngine;

namespace NHQSBCompat.Managers;

internal class DestructionManager : MonoBehaviour
{
	public void Awake()
	{
		LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;
	}

	public void OnDestroy()
	{
		LoadManager.OnCompleteSceneLoad -= OnCompleteSceneLoad;
	}

	private void OnCompleteSceneLoad(OWScene _, OWScene loadScene)
	{
		if (loadScene == OWScene.SolarSystem)
		{
			if (Main.Instance.NewHorizonsAPI.GetCurrentStarSystem() != "SolarSystem")
			{
				// Spams errors when a new player is joining a different star system
				Destroy(FindObjectOfType<OrbitalProbeLaunchController>());
			}
		}
	}
}

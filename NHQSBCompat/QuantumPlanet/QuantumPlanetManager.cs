using Cysharp.Threading.Tasks;
using NHQSBCompat.QuantumPlanet.WorldObjects;
using QSB.WorldSync;
using System.Threading;

namespace NHQSBCompat.QuantumPlanet;

internal class QuantumPlanetManager : WorldObjectManager
{
	public override WorldObjectScene WorldObjectScene => WorldObjectScene.Both;
	public override bool DlcOnly => false;

	public override async UniTask BuildWorldObjects(OWScene scene, CancellationToken ct)
	{
		QSBWorldSync.Init<QSBQuantumPlanet, NewHorizons.Components.Quantum.QuantumPlanet>();
	}
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookingPrototype.Utilities;
using JetBrains.Annotations;
using UnityEngine;

namespace CookingPrototype.GameCore {
public class SpawnPlacesHandler : MonoBehaviour {
	[SerializeField]
	private List<Transform> _spawnPoints;

	[PublicAPI]
	public bool HasAnyFreeSpawnPoint
		=> _spawnPoints.Any(x => x.childCount == 0);

	public int TotalSpawnPoints => _spawnPoints.Count;
	
	public Transform GetSpawnPoint() {
		return _spawnPoints.FirstOrDefault(x => x.childCount == 0);
	}
}
}
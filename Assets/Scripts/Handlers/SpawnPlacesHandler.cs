using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookingPrototype.Utilities;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;

namespace CookingPrototype.GameCore {
public class SpawnPlacesHandler : MonoBehaviour {
	[SerializeField]
	private bool _initFromInspector;
	
	[ShowIf("_initFromInspector")]
	[SerializeField]
	private List<Transform> _spawnPoints;

	[PublicAPI]
	public bool HasAnyFreeSpawnPoint
		=> _spawnPoints.Any(x => x.childCount == 0);

	public int TotalSpawnPoints => _spawnPoints.Count;

	public void AddSpawnPoint(Transform spawnPoint) {
		_spawnPoints.Add(spawnPoint);
	}

	public void AddSpawnPoints(IEnumerable<Transform> spawnPoints) {
		_spawnPoints.AddRange(spawnPoints);
	}


	public Transform GetSpawnPoint() {
		return _spawnPoints.FirstOrDefault(x => x.childCount == 0);
	}
	
	public List<Transform> GetAllFreeSpawnPoints() {
		return _spawnPoints
			.Where(x => x.childCount == 0)
			.ToList();
	}
}
}
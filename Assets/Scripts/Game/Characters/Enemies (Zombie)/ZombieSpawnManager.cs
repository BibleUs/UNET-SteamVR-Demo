using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ZombieSpawnManager : NetworkBehaviour {

	[SerializeField] private GameObject zombiePrefab;
	[SerializeField] private Transform[] zombieSpawnPoints;
	private int counter;
	[SerializeField] private int numberOfZombies = 20;
	[SerializeField] private int maxNumberOfZombies = 80;
	[SerializeField] private float waveRate = 10;
	[SerializeField] private bool isSpawnActivated = true;

	public override void OnStartServer (){
		StartCoroutine(ZombieSpawner());
	}

	IEnumerator ZombieSpawner(){
		for(;;)
		{
			yield return new WaitForSeconds(waveRate);
			GameObject[] zombies = GameObject.FindGameObjectsWithTag(Constants.DefaultZombieTag);
			if(zombies.Length < maxNumberOfZombies)
			{
				InitSpawn();
			}
		}
	}

	void InitSpawn(){
		if(isSpawnActivated)
		{
			for(int i = 0; i < numberOfZombies; i++)
			{
				int randomIndex = Random.Range(0, zombieSpawnPoints.Length);
				SpawnZombies(zombieSpawnPoints[randomIndex].position + new Vector3(Random.Range(-20,20),0,Random.Range(-20,20)), zombieSpawnPoints[randomIndex].rotation);
			}
		}
	}

	void SpawnZombies(Vector3 spawnPos, Quaternion spawnRot){
		counter++;
		GameObject go = GameObject.Instantiate(zombiePrefab, spawnPos, spawnRot) as GameObject;
		go.GetComponent<Zombie_ID>().zombieID = Constants.DefaultZombieTag + counter;
		NetworkServer.Spawn(go);
	}

}

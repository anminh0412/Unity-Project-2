using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ZombieNetworkSpawn : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform[] spawnPoints;
    [SerializeField] float timeToSpawn;
    float timeFromSpawn;
    int zombies;
    [SerializeField] int maxZombies;

    private void Start()
    {
        timeFromSpawn = timeToSpawn;
    }
    void Update()
    {
        zombies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        timeFromSpawn -= Time.deltaTime;
        if (timeFromSpawn < 0 && zombies <= maxZombies)
        {
            int randomNumber = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomNumber];
            //GameObject zombiePrefab = zombiePrefab;
            GameObject Go = PhotonNetwork.Instantiate(this.zombiePrefab.name, spawnPoint.position, Quaternion.identity, 0);
            timeFromSpawn = timeToSpawn;
        }
    }
}

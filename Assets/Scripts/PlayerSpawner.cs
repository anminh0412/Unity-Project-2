using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    public Transform[] spawnPoints;

    private void Start()
    {
        int randomNumber = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomNumber];
        //GameObject playerToSpawn = playerPrefab;
        GameObject playerToSpawn = playerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        GameObject Go = PhotonNetwork.Instantiate(playerToSpawn.name, spawnPoint.position, Quaternion.identity, 0);
        Go.GetComponent<CharacterAttackController>().enabled = true;
        Go.GetComponent<KinematicCharacterMotor>().enabled = true;
        Go.GetComponent<ExampleCharacterController>().enabled = true;
        Go.GetComponent<CharacterHealthController>().enabled = true;
        Go.GetComponent<AudioListener>().enabled = true;
        //Go.GetComponent<Animator>().enabled = true;
        Go.transform.Find("ExampleCamera").gameObject.SetActive(true);
        Go.transform.Find("Canvas").gameObject.SetActive(true);
        Go.transform.Find("PlayerInput").gameObject.GetComponent<ExamplePlayer>().enabled = true;
    }
}

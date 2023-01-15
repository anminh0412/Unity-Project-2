using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class CharacterHealthController : MonoBehaviourPunCallbacks, IPunObservable
{
    public bool isAlive = true;
    public float maxHealth;
    public float currentHealth;
    public float healing;
    public float timeToRespawn;
    public Transform[] spawnPoints;
    public bool immortal = false;
    public GameObject deadPanel;
    public GameObject crossHair;
    float respawnTime;
    public TextMeshProUGUI respawnText;
    public TextMeshProUGUI HealText;
    public TextMeshProUGUI DeadText;
    public TextMeshProUGUI KillText;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI pingFpsText;
    float deltaTime = 1f;
    float ping;
    string playerName;

    public int deadTimes = 0;
    public int kills = 0;
    //public float immortalTime;

    bool callDeadFunctioned = false;
    GameObject dameAuthor;
    int authorId;

    public Animator anim;

    PhotonView view;

    private void Start()
    {
        respawnTime = timeToRespawn;
        currentHealth = maxHealth;
        GameObject Go = GameObject.Find("GetMapSpawnerPoint");
        spawnPoints = Go.GetComponent<GetSpawnPoint>().spawnPoints;
        view = GetComponent<PhotonView>();
    }
    private void Update()
    {
        DieEvent();
        if (view.IsMine)
        {

            HealText.SetText(currentHealth.ToString());
            DeadText.SetText("Dead: " + deadTimes.ToString());
            KillText.SetText("Kill: " + kills.ToString());

            //Show Ping/Fps
            ping = PhotonNetwork.GetPing();
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            pingFpsText.SetText(ping.ToString() + "ping/" + Mathf.Ceil(fps).ToString() + "fps");
            //Set Respawn Text
            if (isAlive == false)
            {
                respawnTime -= Time.deltaTime;
                int tempInt = (int)respawnTime;
                string tempString = tempInt.ToString();
                respawnText.SetText(tempString);
            }
        }
        //Debug.LogError(dameAuthor);
    }
    /*private void Awake()
    {
        //Get Player Name
        if (view.IsMine)
        {

        }
        playerName = PhotonNetwork.LocalPlayer.NickName;
        playerNameText.SetText(playerName);
    }*/
    public void CharacterTakeDame(float _damage, bool isHeadShoot,GameObject _author)
    {
        if (view.IsMine)
        {
            if (!immortal)
            {
                currentHealth -= _damage;
                if (_author != null && currentHealth <= 0f && callDeadFunctioned == false)
                    _author.GetComponent<PhotonView>().RPC("GetKills", RpcTarget.All, 1);
            }
        }
    }
    public void DieEvent()
    {
        if (view.IsMine)
        {
            if (currentHealth <= 0)
            {
                anim.SetBool("isDead", true);

                //if (callFunctioned && !respawned) 
                //Dead code
                if (!callDeadFunctioned) Dead();
            }
        }
    }
    public void HeadShootEvent()
    {
        if (view.IsMine)
            anim.SetBool("isHeadShoot", true);
        Invoke("Respawn", timeToRespawn);

        //Dead code
        Dead();
    }

    public void Respawn()
    {
        if (view.IsMine) 
        {
            gameObject.GetComponent<CharacterAttackController>().ResetBullets();
            immortal = false;
            //Respawn code
            isAlive = true;
            currentHealth = maxHealth;
            callDeadFunctioned = false;
            deadPanel.SetActive(false);
            crossHair.SetActive(true);
            gameObject.GetComponent<CharacterAttackController>().enabled = true;
            gameObject.GetComponent<KinematicCharacterMotor>().enabled = true;
            gameObject.GetComponent<ExampleCharacterController>().enabled = true;
            anim.SetBool("isDead", false);
            anim.SetBool("isHeadShoot", false);

           
            respawnTime = timeToRespawn;

            //Set Respawn point
            int randomNumber = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomNumber];
            gameObject.GetComponent<ExampleCharacterController>().Motor.SetPositionAndRotation(spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
    }
    void Dead()
    {
        if (view.IsMine) 
        {
            immortal = true;
            isAlive = false;
            callDeadFunctioned = true;
            deadTimes += 1;
            deadPanel.SetActive(true);
            crossHair.SetActive(false);
            gameObject.GetComponent<CharacterAttackController>().enabled = false;
            gameObject.GetComponent<KinematicCharacterMotor>().enabled = false;
            gameObject.GetComponent<ExampleCharacterController>().enabled = false;
            Invoke("Respawn", timeToRespawn);
        }
    }
    public void GetKill(int kill)
    {
        kills = kills + kill;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
            stream.SendNext(deadTimes);
            stream.SendNext(immortal);
            //stream.SendNext(kills);
        }
        else
        {
            currentHealth = (float)stream.ReceiveNext();
            deadTimes = (int)stream.ReceiveNext();
            immortal = (bool)stream.ReceiveNext();
            //kills = (int)stream.ReceiveNext();
        }
    }
}

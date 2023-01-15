using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using KinematicCharacterController;
using KinematicCharacterController.Examples;

public class CharacterSoundManager : MonoBehaviour, IPunObservable
{
    PhotonView view;

    [SerializeField] GameObject runOnGroundSound;
    [SerializeField] GameObject runOnGroundWater;
    [SerializeField] GameObject dieSound;
    [SerializeField] GameObject respawnSound;
    [SerializeField] GameObject slideSound;
    [SerializeField] GameObject jumpSound;
    [SerializeField] GameObject changeGunSound;
    [SerializeField] GameObject changeKnifeSound;
    [SerializeField] GameObject currentGun;
    [SerializeField] GameObject currentKnife;

    bool running;
    bool dead;
    bool respawn;
    bool isSlide;
    bool isJump;
    bool readyToImpact = false;
    bool changeGun = false;
    bool changeKnife = false;
    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    void Update()
    {
        GetCurrentState();
        if (running)
        {
            runOnGroundSound.SetActive(true);
        }else runOnGroundSound.SetActive(false);

        if (isSlide)
        {
            slideSound.SetActive(true);
        }
        else slideSound.SetActive(false);

        if (isJump)
        {
            readyToImpact = true;
        } 

        if (!isJump && readyToImpact) 
        { 
            jumpSound.SetActive(true);
            ResetJumpState();
        }

        if (dead)
        {
            dieSound.SetActive(true);
        }else dieSound.SetActive(false);

        if(changeGun)
        {
            changeGunSound.SetActive(true);
        }
        else changeGunSound.SetActive(false);

        if (changeKnife)
        {
            changeKnifeSound.SetActive(true);
        }
        else changeKnifeSound.SetActive(false);

        changeGun = currentGun.activeSelf;
        changeKnife = currentKnife.activeSelf;
    }
    void ResetJumpState()
    {
        readyToImpact = false;
        Invoke("ResetJumpSound", 0.3f);
    }
    void ResetJumpSound()
    {
        jumpSound.SetActive(false);
    }
    void GetCurrentState()
    {
        if (view.IsMine) 
        {
            if (gameObject.GetComponent<ExampleCharacterController>().startRun != 0f && gameObject.GetComponent<ExampleCharacterController>().isCrouch != true && gameObject.GetComponent<ExampleCharacterController>().sliding != true && gameObject.GetComponent<ExampleCharacterController>().falling != true)
            {
                running = true;
            }
            else running = false;

            if (gameObject.GetComponent<ExampleCharacterController>().sliding)
            {
                isSlide = true;
            } 
            else isSlide = false;

            if (gameObject.GetComponent<ExampleCharacterController>().falling)
            {
                isJump = true;
            }
            else isJump = false;

            if (!gameObject.GetComponent<CharacterHealthController>().isAlive)
            {
                dead = true;
            }
            else dead = false;
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(running);
            stream.SendNext(isSlide);
            stream.SendNext(isJump);
            stream.SendNext(readyToImpact);
            stream.SendNext(dead);
            stream.SendNext(changeGun);
            stream.SendNext(changeKnife);
        }
        else
        {
            running = (bool)stream.ReceiveNext();
            isSlide = (bool)stream.ReceiveNext();
            isJump = (bool)stream.ReceiveNext();
            readyToImpact = (bool)stream.ReceiveNext();
            dead = (bool)stream.ReceiveNext();
            changeGun = (bool)stream.ReceiveNext();
            changeKnife = (bool)stream.ReceiveNext();
        }
    }
}

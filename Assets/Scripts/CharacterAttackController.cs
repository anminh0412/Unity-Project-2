using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using System;
using UnityEngine.Animations.Rigging;
using Photon.Pun;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace KinematicCharacterController.Examples
{


    public class CharacterAttackController : MonoBehaviourPunCallbacks
    {
        //Set Network IsMine Player
        PhotonView view;

        public Animator anim;

        public GameObject weaponBag;
        public GameObject currentGun;
        public GameObject currentKnife;
        public GameObject mainCamera;

        public MultiAimConstraint hardAim;
        public TwoBoneIKConstraint secondHardAim;
        public TextMeshProUGUI text;

        public int currentWeapon = 2;

        public bool jumping = false;
        public bool sliding = false;
        float littelDelayGun = 0.7f;
        bool changeDelayValueGun = false;
        float littelDelayKinfe = 0.7f;
        bool changeDelayValueKnife = false;


        //For Gun
        public bool isShoot;
        public bool isAim;
        public float aimingTime;
        //public float fireRate = 15f;
        public float shootingTime = 8f;
        [SerializeField] Transform recoilCamFollowPos;
        [SerializeField] float kickBackAmount = -1;
        [SerializeField] float kickBackSpeed, returnSpeed;
        float currentRecoilPosition, finalRecoilPosition;
        GunController _gunController;


        //Gun stats
        public int damage;
        public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
        public int magazineSize, bulletsPerTap, bulletsMax;
        public bool allowButtonHold, canSpray;
        public int bulletsLeft, bulletsShot;
        public int bulletsMaxSize;

        //bools 
        public bool shooting, readyToShoot, reloading, notRunning;

        [SerializeField] Rig rig;
        [SerializeField] RigBuilder rigBuilder;

        //For Sword
        public GameObject slade1;
        public GameObject slade2;
        public GameObject sladePos;

        public bool leftSlash;
        public bool leftSlash2;
        public bool rightSlash;

        public bool onSlash = false;
        public float breakTime = 0.5f;
        public float nextSlashTime = 0f;

        private void Start()
        {
            view = GetComponent<PhotonView>();
            rigBuilder.enabled = true;
            view.RPC("SelectWeapon", RpcTarget.All, currentWeapon);
            _gunController = currentGun.GetComponent<GunController>();
        }
        void Update()
        {
            SwitchWeapon();
            ControlWeapon(currentWeapon);

            //Set Sword Animaton 
            if (view.IsMine && onSlash) anim.SetLayerWeight(1, 1f);
            else if(view.IsMine && !onSlash) anim.SetLayerWeight(1, 0f);
        }
        private void FixedUpdate()
        {
            if(currentWeapon == 1 && view.IsMine)
            {
                //Recoil 
                currentRecoilPosition = Mathf.Lerp(currentRecoilPosition, 0, returnSpeed * Time.deltaTime);
                finalRecoilPosition = Mathf.Lerp(finalRecoilPosition, currentRecoilPosition, kickBackSpeed * Time.deltaTime);
                recoilCamFollowPos.localPosition = new Vector3(0f, -finalRecoilPosition / 4, finalRecoilPosition);
            }
        }
        public void SwitchWeapon()
        {
            if (view.IsMine)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    //Gun
                    currentWeapon = 1;
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    //Sword
                    currentWeapon = 2;
                }
                if (Input.GetAxis("Mouse ScrollWheel") != 0f || Input.GetKeyDown(KeyCode.Q))
                {
                    currentWeapon = (currentWeapon == 1) ? (2) : (1);
                }
                view.RPC("SelectWeapon", RpcTarget.All, currentWeapon);
            }
        }
        [PunRPC]
        void SelectWeapon(int newWeapon)
        {
            //Change Weapon
            if (newWeapon == 1)
            {
                currentGun.SetActive(true);
                currentKnife.SetActive(false);
            }
            else if (newWeapon == 2)
            {
                currentGun.SetActive(false);
                currentKnife.SetActive(true);
            }
        }

        void ControlWeapon(int newWeapon)
        {
            if (!view.IsMine) return;
            if (newWeapon == 1)
            {
                RifleAttackAction();
                hardAim.weight = 0.75f;
                secondHardAim.weight = 1f;
                onSlash = false;
            }
            else if (newWeapon == 2)
            {
                SwordAttackAction();
                isAim = false;
                shootingTime = 0f;
                anim.SetBool("isAim", isAim);
                anim.SetBool("isShoot", isShoot);
                anim.SetBool("isReload", false);
                hardAim.weight = 0f;
                secondHardAim.weight = 0f;
            }
        }
        public void GetGunInfo()
        {
            if (!view.IsMine) return;
            //Get Gun Information
            damage = _gunController.damage;
            timeBetweenShooting = _gunController.timeBetweenShooting;
            spread = _gunController.spread;
            range = _gunController.range;
            reloadTime = _gunController.reloadTime;
            timeBetweenShots = _gunController.timeBetweenShots;
            bulletsMax = _gunController.bulletsMax;
            magazineSize = _gunController.magazineSize;
            bulletsPerTap = _gunController.bulletsPerTap;
            bulletsLeft = _gunController.bulletsLeft;
            bulletsShot = _gunController.bulletsShot;
            readyToShoot = _gunController.readyToShoot;
            reloading = _gunController.reloading;
            allowButtonHold = _gunController.allowButtonHold;
            canSpray = _gunController.canSpray; 
            bulletsMaxSize = _gunController.bulletsMaxSize; 
        }
        public void ResetBullets()
        {
            if (!view.IsMine) return;
            _gunController.bulletsMax = bulletsMaxSize;
            _gunController.bulletsLeft = magazineSize;
        }
        public void RifleAttackAction()
        {
            if (!changeDelayValueGun) littelDelayGun = 0.7f;
            if (littelDelayGun > 0f)
            {
                littelDelayGun -= Time.deltaTime;
                changeDelayValueGun = true;
            }
            changeDelayValueKnife = false;

            if (!view.IsMine) return;
            GetGunInfo();

            //Set Text
            if (text != null)
                text.SetText(currentGun.name + ": " + bulletsLeft / bulletsPerTap + " / " + bulletsMax);

            if(canSpray)
            if (Input.GetKeyDown(KeyCode.C)) _gunController.allowButtonHold = (allowButtonHold) ? (false) : (true);

            if (Input.GetAxis("Vertical") < 0f || Input.GetAxis("Horizontal") != 0f)
            {
                if (Input.GetAxis("Vertical") > 0f)
                {
                    notRunning = true;
                    anim.SetBool("runShooting", true);
                }
                else
                {
                    notRunning = false;
                    shootingTime = 0f;
                }
            }
            else { notRunning = true; anim.SetBool("runShooting", false); }

            //Aiming Animation
            if (isAim || isShoot)
            {
                anim.SetBool("isAim", true);
            }
            else 
            { 
                anim.SetBool("isAim", false);
            }
            //Set Rig Weight
            if (isAim || isShoot)
            {
                rig.weight = 1f;
                if(!notRunning || reloading || jumping || sliding) rig.weight = 0f;
            }
            else 
            {
                rig.weight = 0f;
            }

            //Reload Animation
            anim.SetBool("isReload", reloading);

            //New Shoot Event
            if (allowButtonHold) 
            { 
                shooting = Input.GetKey(KeyCode.Mouse0); 
            }
            else shooting = Input.GetKeyDown(KeyCode.Mouse0);

            //Reload
            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading && bulletsMax > 0) 
            {
                _gunController.Reload(); 
            }

            //Reload automatically when trying to shoot without ammo
            if (readyToShoot && shooting && !reloading && bulletsLeft <= 0 && bulletsMax > 0) currentGun.GetComponent<GunController>().Reload();

            //Shoot
            if (readyToShoot && shooting && !reloading && bulletsLeft > 0 && notRunning && !jumping && !sliding && littelDelayGun <= 0f)
            {
                bulletsShot = 0;
                _gunController.NewShoot();
                currentRecoilPosition += kickBackAmount;
                anim.SetBool("isShoot", true);
                isShoot = true;
                shootingTime = 8f;
            }
            else { anim.SetBool("isShoot", false); } 

            //Set Shoot Animation
            if (isShoot)
            {
                shootingTime -= Time.deltaTime;
            }
            if(shootingTime <= 0f)
            {
                isShoot = false;
            }

            if (isAim || isShoot)
            {
                GetComponent<ExampleCharacterController>().TransitionToState(CharacterState.Aiming);
            }else GetComponent<ExampleCharacterController>().TransitionToState(CharacterState.Default);
        }
        public void SwordAttackAction()
        {
            if (!changeDelayValueKnife) littelDelayKinfe = 0.7f;
            if (littelDelayKinfe > 0f)
            {
                littelDelayKinfe -= Time.deltaTime;
                changeDelayValueKnife = true;
            }
            changeDelayValueGun = false;

            if (!view.IsMine) return;
            anim.SetBool("leftSlash", leftSlash);
            anim.SetBool("leftSlash2", leftSlash2);
            anim.SetBool("rightSlash", rightSlash);

            breakTime -= Time.deltaTime;

            //Set Text
            if (text != null)
                text.SetText(currentKnife.name);

            //Checkking charactor is slash
            if (nextSlashTime > 0f)
            {
                onSlash = true;
            }
            else onSlash = false;
            nextSlashTime -= Time.deltaTime;

            //Input slash action
            if (Input.GetMouseButtonDown(0) && !onSlash && littelDelayKinfe <= 0f)
            {
                leftSlash = true;
                onSlash = true;
                nextSlashTime = 0.7f;
                breakTime = 0.1f;
                GameObject currentSlade1 = PhotonNetwork.Instantiate(this.slade1.name, sladePos.transform.position, Quaternion.identity);
                currentSlade1.GetComponent<CharacterKnifeController>().FindAuthor(gameObject);
                currentSlade1.GetComponent<CharacterKnifeController>().GetPos(sladePos);
            }
            if (leftSlash && onSlash)
            {
                if (Input.GetMouseButtonDown(0) && breakTime <= 0f)
                {
                    leftSlash2 = true;
                    breakTime = 0.5f;
                    nextSlashTime = 0.6f;
                    GameObject currentSlade2 = PhotonNetwork.Instantiate(this.slade2.name, sladePos.transform.position, Quaternion.identity);
                    currentSlade2.GetComponent<CharacterKnifeController>().FindAuthor(gameObject);
                    currentSlade2.GetComponent<CharacterKnifeController>().GetPos(sladePos);
                }
            }
            if (Input.GetMouseButtonDown(1) && !onSlash && littelDelayKinfe <= 0f)
            {
                onSlash = true;
                rightSlash = true;
                nextSlashTime = 0.6f;
                GameObject currentSlade2 = PhotonNetwork.Instantiate(this.slade2.name, sladePos.transform.position, Quaternion.identity);
                currentSlade2.GetComponent<CharacterKnifeController>().FindAuthor(gameObject);
                currentSlade2.GetComponent<CharacterKnifeController>().GetPos(sladePos);
            }
            //Break off slash action
            if (!onSlash)
            {
                leftSlash = false;
                leftSlash2 = false;
                rightSlash = false;
            }
        }
    }
}
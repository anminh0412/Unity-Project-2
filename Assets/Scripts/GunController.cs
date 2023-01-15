using UnityEngine;
//using TMPro;
using Photon.Pun;


public class GunController : MonoBehaviour
{
    PhotonView view;
    //bullet 
    public GameObject bullet;
    public GameObject grenade;

    //SoundManager
    public GameObject reloadSound;
    public GameObject changeWeaponSound;
    public GameObject bulletShootSound;
    public GameObject grenadeShootSound;

    //player
    public GameObject player;

    //currentBullet
    public int checkBullet = 1;

    //bullet force
    public float shootForce, upwardForce;

    //Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap, bulletsMax;
    public bool allowButtonHold;
    public int bulletsLeft, bulletsShot;
    public int bulletsMaxSize;
    //bools 
    public bool shooting, readyToShoot, reloading;

    //Reference
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;
    public Transform currentTargetPoint;
    [SerializeField] Transform shootTargetPoint;
    [SerializeField] float localKickBackAmount;
    [SerializeField] float currentKickBackAmount;
    [SerializeField] float kickBackSpeed, returnSpeed;
    float currentRecoilPosition, finalRecoilPosition;

    //Graphics
    public Vector3 SetShootPos;
    //public GameObject impactEffect;
    public ParticleSystem muzzleFlash;
    public GameObject muzzleFlashLight;
    public float camShakeMagnitude, camShakeDuration;
    //public TextMeshProUGUI text;

    //bug
    public bool allowInvoke = true;
    public bool canSpray;
    private void Start()
    {
        view = GetComponent<PhotonView>();
        readyToShoot = true;
    }
    private void Awake()
    {
        bulletsLeft = magazineSize;
        bulletsMax = bulletsMaxSize;
    }
    private void Update()
    {
        if (readyToShoot) currentKickBackAmount = Mathf.Lerp(0, currentKickBackAmount, returnSpeed * Time.deltaTime);
        currentRecoilPosition = Mathf.Lerp(currentRecoilPosition, 0, returnSpeed * Time.deltaTime);
        finalRecoilPosition = Mathf.Lerp(finalRecoilPosition, currentRecoilPosition, kickBackSpeed * Time.deltaTime);

        FindTargerPoint();

    }
    private void FixedUpdate()
    {
        shootTargetPoint.localPosition = new Vector3(0f, finalRecoilPosition, 0f);
    }
    public void FindTargerPoint()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, 1000f , whatIsEnemy, QueryTriggerInteraction.Ignore))
        {
            currentTargetPoint.position = raycastHit.point;
        }
        else
        {
            currentTargetPoint.position = fpsCam.transform.position + fpsCam.transform.forward * 1000f;
        }
    }
/*    public void CallShootFunsion()
    {
        view.RPC("NewShoot", RpcTarget.All);
    }
    [PunRPC]*/
    public void NewShoot()
    {
        readyToShoot = false;

        //Add Recoil
        currentRecoilPosition += currentKickBackAmount;
        currentKickBackAmount += localKickBackAmount;

        //Calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = shootTargetPoint.position - attackPoint.position;

        //Calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction

        //Instantiate bullet/projectile
        if (bullet) 
        {
            GameObject currentBullet = PhotonNetwork.Instantiate(this.bullet.name, attackPoint.position, Quaternion.identity);
            currentBullet.transform.forward = directionWithSpread.normalized;
            //Add forces to bullet
            currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
            currentBullet.GetComponent<CustomBullet>().FindAuthor(player);
            PhotonNetwork.Instantiate(this.bulletShootSound.name, gameObject.transform.position, gameObject.transform.localRotation);
        }
        if (grenade) 
        {
            GameObject currentBullet = PhotonNetwork.Instantiate(this.grenade.name, attackPoint.position, Quaternion.identity);
            currentBullet.transform.forward = directionWithSpread.normalized;
            //Add forces to bullet
            currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
            currentBullet.GetComponent<CustomGrenade>().FindAuthor(player);
            PhotonNetwork.Instantiate(this.grenadeShootSound.name, gameObject.transform.position, gameObject.transform.localRotation);
        }
        //GameObject currentBullet = PhotonNetwork.Instantiate(this.bullet.name, attackPoint.position, Quaternion.identity); //store instantiated bullet in currentBullet
        //Rotate bullet to shoot direction
        //currentBullet.transform.forward = directionWithSpread.normalized;

        //Add forces to bullet
        //currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        //currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse); //NoNeed
        //Set author shoot the bullet
        //if (bullet) currentBullet.GetComponent<CustomBullet>().FindAuthor(player);
        //else return;
        //if (grenade) currentBullet.GetComponent<CustomGrenade>().FindAuthor(player);
        //else return;
        //Instantiate muzzle flash, if you have one
        if (muzzleFlash != null)
            muzzleFlash.Play();
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.GetComponent<Light>().enabled = true;
            Invoke("DisableLight", 0.05f);
        }

        bulletsLeft--;
        bulletsShot++;

        //Invoke resetShot function (if not already invoked), with your timeBetweenShooting
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;

            //Add recoil to player (should only be called once)
            //playerRb.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);
        }

        //if more than one bulletsPerTap make sure to repeat shoot function
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("NewShoot", timeBetweenShots);
    }
    public void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }
    void DisableLight()
    {
        muzzleFlashLight.GetComponent<Light>().enabled = false;
    }
    public void Reload()
    {
        reloading = true;
        reloadSound.SetActive(true);
        Invoke("ReloadFinished", reloadTime);
    }
    public void ReloadFinished()
    {
        if (bulletsMax >= magazineSize)
        {
            bulletsLeft = magazineSize; 
            bulletsMax -= bulletsShot;
            bulletsShot = 0;
        }
        else 
        { 
            bulletsLeft = bulletsMax;
            bulletsMax = 0;
        }
        
        reloading = false;
        reloadSound.SetActive(false);
    }
}

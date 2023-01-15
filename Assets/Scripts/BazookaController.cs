using UnityEngine;
//using TMPro;
using Photon.Pun;

public class BazookaController : MonoBehaviour
{
    //PhotonView view;
    //bullet 
    public GameObject bullet;
    public GameObject grenade;

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

    //bools 
    public bool shooting, readyToShoot, reloading;

    //Reference
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;
    public Transform currentTargetPoint;

    //Graphics
    public Vector3 SetShootPos;
    //public GameObject impactEffect;
    public ParticleSystem muzzleFlash;
    public float camShakeMagnitude, camShakeDuration;
    //public TextMeshProUGUI text;

    //bug
    public bool allowInvoke = true;
    public bool canSpray;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
    private void Update()
    {
        //SetText
        //text.SetText(bulletsLeft + " / " + magazineSize);
        //Set ammo display, if it exists :D
        //if (text != null)
        // text.SetText(gameObject.name + ": " + bulletsLeft / bulletsPerTap + " / " + bulletsMax);
        FindTargerPoint();
    }
    public void FindTargerPoint()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 200f, whatIsEnemy, QueryTriggerInteraction.Ignore))
        {
            currentTargetPoint.position = raycastHit.point;
        }
        else
        {
            currentTargetPoint.position = fpsCam.transform.position + fpsCam.transform.forward * 200f;
        }
    }
    public void NewShoot()
    {
        readyToShoot = false;

        //Calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = currentTargetPoint.position - attackPoint.position;

        //Calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction

        //Instantiate bullet/projectile
        GameObject currentBullet = PhotonNetwork.Instantiate(this.bullet.name, attackPoint.position, Quaternion.identity); //store instantiated bullet in currentBullet
        //Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        //Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        //currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse); //NoNeed
        //Set author shoot the bullet
        if (bullet) currentBullet.GetComponent<CustomBullet>().FindAuthor(player);
        else return;
        if (grenade) currentBullet.GetComponent<CustomGrenade>().FindAuthor(player);
        else return;
        //Instantiate muzzle flash, if you have one
        if (muzzleFlash != null)
            muzzleFlash.Play();

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
    public void Reload()
    {
        reloading = true;
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
    }
}

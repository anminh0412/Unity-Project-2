using UnityEngine;
using Photon.Pun;

public class CustomBullet : MonoBehaviourPunCallbacks
{
    GameObject target;
    GameObject author;
    PhotonView view;

    //Assignables
    public GameObject impactEffect;
    public Rigidbody rb;

    //public GameObject shootSound;
    public GameObject flyingSound;
    public GameObject impactSound;

    //Damage
    public float bulletDame;

    //Lifetime
    public int maxCollisions;
    public float maxLifetime;

    int collisions;
    bool impacted = false;

    //Setup
    PhysicMaterial physics_mat;
    [Range(0f, 1f)]
    public float bounciness;
    public bool useGravity;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        view = GetComponent<PhotonView>();
        Setup();
    }
    private void Awake()
    {
        //m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        //m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }
    private void Update()
    {
        if (view.IsMine)
        {
            if (collisions > maxCollisions) Impact();

            //Count down lifetime
            maxLifetime -= Time.deltaTime;
            if (maxLifetime <= 0) DestroyEvent();
            //Debug.LogError(target);
        }
    }
    public void FindAuthor(GameObject Go)
    {
        author = Go;
    }
    private void DestroyEvent()
    {
        if (view.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (view.IsMine) 
        {
            //Don't count collisions with other bullets
            if (collision.collider.CompareTag("Bullet")) return;

            else if (collision.collider.CompareTag("Enemy"))
            {
                foreach (ContactPoint contact in collision.contacts)
                {
                    //SendDame
                    if (contact.otherCollider.name == "HeadCollider")
                    {
                        collision.gameObject.GetComponent<PhotonView>().RPC("EnemyTakeDame", RpcTarget.All, bulletDame, true);
                    }
                    else
                    {
                        collision.gameObject.GetComponent<PhotonView>().RPC("EnemyTakeDame", RpcTarget.All, bulletDame, false);
                    }
                }
            }
            else if (collision.gameObject.CompareTag("Player"))
            {
                if (collision.gameObject == author)
                {
                    return;
                }
                else
                {
                    target = collision.gameObject;
                }
            }
            collisions++;
        }
    }
    private void Impact()
    {
        if (view.IsMine)
        {
            if (!impacted)
            {
                PhotonNetwork.Instantiate(this.impactEffect.name, gameObject.transform.position, gameObject.transform.localRotation);
                //PhotonNetwork.Instantiate(this.impactSound.name, gameObject.transform.position, gameObject.transform.localRotation);
                impacted = true;
                if(target != null)
                {
                    target.GetComponent<PhotonView>().RPC("BulletSendDame", RpcTarget.All, bulletDame, null);
                    if (target.GetComponent<CharacterHealthController>().currentHealth <= bulletDame && target.GetComponent<CharacterHealthController>().immortal == false)
                    {
                        author.GetComponent<PhotonView>().RPC("GetKills", RpcTarget.All);
                    }
                }
                Invoke("DestroyEvent", 0.05f);
            }
        }
    }
    private void Setup()
    {
        //Create a new Physic material
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        //Assign material to collider
        GetComponent<SphereCollider>().material = physics_mat;

        //Set gravity
        rb.useGravity = useGravity;
    }
}

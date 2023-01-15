using UnityEngine;
using Photon.Pun;

public class CustomGrenade : MonoBehaviourPunCallbacks
{
    GameObject author;
    PhotonView view;
    //Assignables
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEnemies;
    private bool exploded = false;

    //SoundManager
    public GameObject impactSound;
    public GameObject flyingSound;

    //Stats
    [Range(0f, 1f)]
    public float bounciness;
    public bool useGravity;

    //Damage
    public int explosionDamage;
    public float explosionRange;
    public float explosionForce;

    //Lifetime
    public int maxCollisions;
    public float maxLifetime;
    public bool explodeOnTouch = true;

    int collisions;
    PhysicMaterial physics_mat;

    private void Start()
    {
        Setup();
        view = GetComponent<PhotonView>();
    }
    
    private void Update()
    {
        //When to explode:
        if (collisions > maxCollisions) Explode();

        //Count down lifetime
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Explode();
    }
    public void FindAuthor(GameObject Go)
    {
        author = Go;
    }
    private void Explode()
    {
        if (view.IsMine)
        {
            if (exploded == false)
            {
                GameObject _exploded = PhotonNetwork.Instantiate(this.explosion.name, transform.position, Quaternion.identity);
                _exploded.GetComponent<NeworkExploed>().FindAuthor(author);
                PhotonNetwork.Instantiate(this.impactSound.name, gameObject.transform.position, gameObject.transform.localRotation);
                exploded = true;
                Invoke("Delay", 0.01f);
            }
        }
    }
    private void Delay()
    {
       if(view.IsMine) PhotonNetwork.Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject);
        //Don't count collisions with other bullets
        if (collision.collider.CompareTag("Bullet")) return;

        //Count up collisions
        collisions++;
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

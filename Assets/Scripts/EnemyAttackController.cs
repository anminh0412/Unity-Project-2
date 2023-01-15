using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private SphereCollider triggerSphere;
    [SerializeField] private bool MoveRigidBody = true;
    [SerializeField] private float RgbRotationSpeed = 5f;
    [SerializeField] private float TriggerRadius = 5f;
    [SerializeField] private float ChaseSpeed = 5f;
    [SerializeField] private float ChaseDelay = 1f;


    #region private
    private Transform localTrans;
    private bool detected = false;
    //private GameObject TargetPlayer;
    private Transform TargetTrans;
    private Vector3 targetPos;
    private Rigidbody localRgb;
    #endregion

    public Animator anim;
    public float dist;
    public float currentSpeed;
    public float attackTime = 0f;
    bool targetIsAlive;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.gameObject.GetComponent<Animator>();

        localRgb = GetComponent<Rigidbody>();
        localTrans = GetComponent<Transform>();

        if (!triggerSphere) triggerSphere = GetComponent<SphereCollider>();
        if (triggerSphere)
            triggerSphere.radius = TriggerRadius;

        TargetTrans = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (attackTime >= -1f)
        {
            attackTime -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        //Get Velocity
        //currentSpeed = localRgb.velocity.magnitude*100000f;
        anim.SetFloat("Speed", currentSpeed);
        if (TargetTrans != null) 
        { 
            dist = Vector3.Distance(TargetTrans.position, localTrans.position);
            if (detected && dist > 1.5f && attackTime <= 0f)
            {
                //Chase Player..
                Chase(TargetTrans);
                currentSpeed = 1f;
                AttackEventEnd();
            }
            else if(dist <= 1.5f)
            {
                currentSpeed = 0f;
                AttackEventStart();
            }
        } 
    }
    void AttackEventStart()
    {
        RotateRgb(TargetTrans);
        attackTime = 2f;
        anim.SetBool("isAttack", true);
    }
    void AttackEventEnd()
    {
        anim.SetBool("isAttack", false);
    }
    void Chase(Transform _target)
    {
        var speed = ChaseSpeed;

        targetPos = _target.position;
        targetPos.y = localTrans.position.y;

        //Move Rigibody;
        if (MoveRigidBody)
        {
            RotateRgb(_target);
            localRgb.MovePosition(localRgb.position + localTrans.forward * speed * Time.deltaTime);
        }
    }

    private void RotateRgb(Transform _target)
    {
        Vector3 localTarget = localTrans.InverseTransformPoint(_target.position);

        float angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        Vector3 eulerAngleVelocity = new Vector3(0, angle, 0);
        Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime * RgbRotationSpeed);
        localRgb.MoveRotation(localRgb.rotation * deltaRotation);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (detected) return;

        //Follow only if the Detected one is the Player..
        if (other.CompareTag("Player"))
        {
            detected = true;
            StartCoroutine(ActivateChasing(other.transform, ChaseDelay));
        }
        else
        {
            Physics.IgnoreCollision(other.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }

    IEnumerator ActivateChasing(Transform other, float _waitSec = 1f)
    {
        yield return new WaitForSeconds(_waitSec);
        TargetTrans = other;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireSphere(this.transform.position, TriggerRadius);
    }

    public void StopChasing()
    {
        detected = false;
    }
}

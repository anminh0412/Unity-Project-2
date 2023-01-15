using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyHealController : MonoBehaviour
{
    public float maxHealth = 500f;
    public float currentHealth;
    public Animator anim;
    public bool headShoot;
    public GameObject deadSound;
    public GameObject idleSound;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        anim = gameObject.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0f && !headShoot)
        {
            Die();
        }else if(currentHealth <= 0f && headShoot)
        {
            DieByHeadShoot();
        }
    }
    public void HealthController(float damage)
    {
        currentHealth -= damage;
    }
    void Die()
    {
        anim.SetBool("isDead", true);
        GetComponent<EnemyAttackController>().enabled = false;
        GetComponent<SphereCollider>().enabled = false;
        Invoke("Dead", 8f);
        deadSound.SetActive(true);
        idleSound.SetActive(false);
    }
    void DieByHeadShoot()
    {
        anim.SetBool("isHeadShoot", true);
        GetComponent<EnemyAttackController>().enabled = false;
        GetComponent<SphereCollider>().enabled = false;
        Invoke("Dead", 8f);
        deadSound.SetActive(true);
        idleSound.SetActive(false);
    }
    void Dead()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSendDame : MonoBehaviour
{
    public float damage;
    //private void OnCollisionEnter(Collision collision)
    //{/
        /*if(collision.collider.CompareTag("Player")){
            collision.gameObject.GetComponent<CharacterHealthController>().CharacterTakeDame(damage, false);
            Debug.Log("danh ne");
        }*/
        //Debug.Log(collision.gameObject.name);
    //}
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<CharacterHealthController>().CharacterTakeDame(damage, false, null);
        }
    }
}

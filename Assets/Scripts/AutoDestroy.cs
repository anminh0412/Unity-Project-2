using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] float destroyTime;
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}

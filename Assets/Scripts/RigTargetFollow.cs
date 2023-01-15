using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigTargetFollow : MonoBehaviour
{
    [SerializeField] Transform target;

    private void Update()
    {
        gameObject.transform.position = target.position;
    }
}

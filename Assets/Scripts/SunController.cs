using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour
{
    public float DayLength;
    private float _rotationSpeed;

    void Update()
    {
        _rotationSpeed = Time.deltaTime / DayLength;
        //transform.Rotate(_rotationSpeed, 0, 0);
        if(transform.localEulerAngles.x > 195f)
        {
            gameObject.transform.eulerAngles = new Vector3(
                gameObject.transform.eulerAngles.x - 195f,
                gameObject.transform.eulerAngles.y,
                gameObject.transform.eulerAngles.z
                );
        }else transform.Rotate(_rotationSpeed, 0, 0);
    }
}

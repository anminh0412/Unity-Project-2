using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    public Animator anim;
    public CharacterController characterController;
    public GameObject mainCamera;
    bool onAction = false;
    public float Gravity;
    public bool useGravity = true;
    public bool isMoving = false;
    public bool isRunning = false;
    private float vertical;
    private float horizontal;
    Vector3 velocity;
    public float startRun = 0f;
    public float speed = 0f;
    public float rotationY;
    public float rotationYCamera;
    public bool isLock;
    private void Start()
    {
        //Hide mouse
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        

        if (vertical != 0f || horizontal != 0f)
        {
            startRun = (Mathf.Abs(vertical) + Mathf.Abs(horizontal)) / (Mathf.Abs(vertical) + Mathf.Abs(horizontal));
        }
        else startRun = 0f;

        anim.SetFloat("Vertical", vertical);
        anim.SetFloat("Horizontal", horizontal);

        //doubleTap to Slide
        if (Input.GetKeyDown(KeyCode.LeftShift) && !onAction && isRunning)
        {
            anim.SetBool("isSlide", true);
            onAction = true;
            Invoke("endSlide", 0.77f);
        }

        //Running
        anim.SetFloat("Speed", startRun);

        if (startRun >= 1f)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        //Jump on Running
        if (Input.GetKeyDown(KeyCode.Space) && !onAction && isRunning)
        {
            anim.SetBool("isJump", true);
            onAction = true;
            Invoke("endJump", 1.52f);
        }
          
        
        //Add Gravity
        if (useGravity)
        {
            velocity.y += -Gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }

        //Rotation Manager
        //if (vertical == 0 && horizontal == 0) rotationY = 0f;
        if (vertical == 0)
        {
            if (horizontal > 0) rotationY = 90;
            if (horizontal < 0) rotationY = -90;
        }
        else if (horizontal == 0)
        {
            if (vertical > 0) rotationY = 0;
            if (vertical < 0) rotationY = 180;
        }
        else
        {
            rotationY = Mathf.Atan2(horizontal , vertical) * Mathf.Rad2Deg;
            /*if (vertical > 0)
            {
                rotationY = Mathf.Atan(horizontal / vertical);
                Debug.Log(Mathf.Atan(1f));
            }
            else if (vertical < 0)
            {
                if (horizontal > 0) rotationY = (-Mathf.Atan(vertical / horizontal)) + 90;
                if (horizontal < 0) rotationY = (-Mathf.Atan(vertical / horizontal)) - 90;
            }*/
        }

        //Press ESC pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isLock = !isLock;
            if (isLock)
            {
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if(isRunning) rotationYCamera = mainCamera.transform.localEulerAngles.y;
        Vector3 newRotation = new Vector3(0, rotationY + rotationYCamera, 0);
        transform.eulerAngles = newRotation;
    }

    void endJump()
    {
        anim.SetBool("isJump", false);
        onAction = false;
    }
    void endSlide()
    {
        anim.SetBool("isSlide", false);
        onAction = false;
    }
}

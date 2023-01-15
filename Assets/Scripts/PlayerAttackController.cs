using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    public Animator anim;
    protected bool onSkill = false;
    public bool isLock;

    private void Start()
    {
        //Hide mouse
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
    }
    void Update()
    {
        //Press Attack
        if (Input.GetMouseButtonDown(0) && onSkill == false)
        {
            anim.SetBool("isCross", true);
            onSkill = true;
            Invoke("CrossOff", 1.1f);
        }
        if (Input.GetMouseButtonDown(1) && onSkill == false)
        {
            anim.SetBool("isHook", true);
            onSkill = true;
            Invoke("HookOff", 1.45f);
        }
        if (Input.GetKeyDown(KeyCode.E) && onSkill == false)
        {
            anim.SetBool("isBodyKick", true);
            onSkill = true;
            Invoke("BodyKickOff", 0.8f);
        }
        if (Input.GetKeyDown(KeyCode.R) && onSkill == false)
        {
            anim.SetBool("isHighKick", true);
            onSkill = true;
            Invoke("HighKickOff", 1f);
        }
        if (Input.GetKeyDown(KeyCode.F) && onSkill == false)
        {
            anim.SetBool("isBackKick", true);
            onSkill = true;
            Invoke("BackKickOff", 1f);
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
    void CrossOff()
    {
        anim.SetBool("isCross", false);
        onSkill = false;
    }
    void HookOff()
    {
        anim.SetBool("isHook", false);
        onSkill = false;
    }
    void BodyKickOff()
    {
        anim.SetBool("isBodyKick", false);
        onSkill = false;
    }
    void HighKickOff()
    {
        anim.SetBool("isHighKick", false);
        onSkill = false;
    }
    void BackKickOff()
    {
        onSkill = false;
        anim.SetBool("isBackKick", false);
    }

}

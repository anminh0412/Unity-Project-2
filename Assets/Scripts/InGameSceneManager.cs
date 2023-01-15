using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.HighDefinition;

public class InGameSceneManager : MonoBehaviourPunCallbacks
{
    public GameObject escPanel;
    public GameObject canvas;
    public Scrollbar qualityScrollbar;
    public Text qualityText;
    public bool escaped = false;
    public bool openMouse = false;

    private void Start()
    {
        Canvas cv = canvas.GetComponent<Canvas>();
        cv.overrideSorting = true;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escaped = (escaped == false) ? (true) : (false);
        }

        if (escaped)
        {
            escPanel.SetActive(true);
            Cursor.visible = true;
            openMouse = true;
        }
        else 
        { 
            escPanel.SetActive(false); 
            Cursor.visible = false;
            openMouse = false;
        }
        if(qualityScrollbar.value <= 0.33f)
        {
            qualityText.text = "Low";
            QualitySettings.SetQualityLevel(0, true);
            LowQualSetting();
        }
        else if(qualityScrollbar.value < 0.66f && qualityScrollbar.value > 0.33f)
        {
            qualityText.text = "Medium";
            QualitySettings.SetQualityLevel(1, true);
            MediumQualSetting();
        }
        else
        {
            qualityText.text = "High";
            QualitySettings.SetQualityLevel(2, true);
            HighQualSetting();
        }
    }
    void LowQualSetting()
    {
        GameObject.Find("ExampleCamera").GetComponent<HDAdditionalCameraData>().antialiasing = HDAdditionalCameraData.AntialiasingMode.None;
    }
    void MediumQualSetting()
    {
        GameObject.Find("ExampleCamera").GetComponent<HDAdditionalCameraData>().antialiasing = HDAdditionalCameraData.AntialiasingMode.FastApproximateAntialiasing;
    }
    void HighQualSetting()
    {
        GameObject.Find("ExampleCamera").GetComponent<HDAdditionalCameraData>().antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;
    }

    public void BackToLobbyScene()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }
    public void ExitGameScene()
    {
        PhotonNetwork.Disconnect();
        OnDisconnected(DisconnectCause.ApplicationQuit);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (cause == DisconnectCause.ApplicationQuit) { Application.Quit(); }
    }
    public void ResumeGame()
    {
        escaped = false;
    }
    public void ChangeFullScene()
    {
        Screen.SetResolution(1920, 1080, true);
    }
    public void Change1600x900()
    {
        Screen.SetResolution(1600, 900, false);
    }
    public void Change1920x1080()
    {
        Screen.SetResolution(1920, 1080, false);
    }
    public void Change30Fps()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
    }
    public void Change60Fps()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
    public void ChangeInfinityFps()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 300;
    }
}

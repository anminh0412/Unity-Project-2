using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public int nextScene;
    [SerializeField] private Slider LoadingSlider;
    public Image playerBackGround;
    public Sprite[] bg;
    void Start()
    {
        playerBackGround.sprite = bg[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        PhotonNetwork.LoadLevel(nextScene);
    }
    void Update()
    {
        LoadingSlider.value = PhotonNetwork.LevelLoadingProgress / 0.9f;
    }
}

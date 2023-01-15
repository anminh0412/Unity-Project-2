using UnityEngine;
using TMPro;

public class PlayerDealPanelController : MonoBehaviour
{
    public TextMeshProUGUI respawnText;
    public int respawnTime;

    void Update()
    {
        string tempString = respawnTime.ToString();
        if (respawnText != null)
            respawnText.SetText(tempString);
    }
}

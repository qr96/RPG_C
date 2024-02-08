using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopup : MonoBehaviour
{
    // Common
    public Button exitButton;

    // Inventory
    TMP_Text moneyText;

    private void Awake()
    {
        exitButton = transform.Find("ExitButton").GetComponent<Button>();

        moneyText = transform.Find("InventoryTab").Find("Property").Find("MoneyText").GetComponent<TMP_Text>();

        exitButton.onClick.AddListener(() => HidePopup());
    }

    public void ShowPopup()
    {
        SendInfoPacket();
        gameObject.SetActive(true);
    }

    public void HidePopup()
    {
        gameObject.SetActive(false);
    }

    public void SetMoney(long money)
    {
        moneyText.text = money.ToString();
    }

    #region Packet

    void SendInfoPacket()
    {
        C_InventoryInfo inventoryInfo = new C_InventoryInfo();
        Managers.Network.Send(inventoryInfo);
    }

    #endregion
}

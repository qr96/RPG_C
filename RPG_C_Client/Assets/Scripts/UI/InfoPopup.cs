using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopup : MonoBehaviour
{
    // Common
    Button exitButton;
    Button invenButton;
    Button skillButton;

    GameObject statTab;
    GameObject invenTab;
    GameObject skillTab;

    // Inventory
    TMP_Text moneyText;

    private void Awake()
    {
        exitButton = gameObject.Find<Button>("ExitButton");
        invenButton = gameObject.Find<Button>("NavigationBar/Inven");
        skillButton = gameObject.Find<Button>("NavigationBar/Skill");

        statTab = gameObject.Find("StatTab");
        invenTab = gameObject.Find("InvenTab");
        skillTab = gameObject.Find("SkillTab");

        moneyText = invenTab.Find<TMP_Text>("Property/MoneyText");

        exitButton.onClick.AddListener(() => HidePopup());
        invenButton.onClick.AddListener(() => SelectTab(1));
        skillButton.onClick.AddListener(() => SelectTab(2));
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

    void SelectTab(int index)
    {
        statTab.SetActive(index == 0);
        invenTab.SetActive(index == 1);
        skillTab.SetActive(index == 2);
    }

    #region Packet

    void SendInfoPacket()
    {
        C_InventoryInfo inventoryInfo = new C_InventoryInfo();
        Managers.Network.Send(inventoryInfo);
    }

    #endregion
}

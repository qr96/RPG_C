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
        gameObject.SetActive(true);
    }

    public void HidePopup()
    {
        Debug.Log("HidePopup");
        gameObject.SetActive(false);
    }

    public void SetMoney(long money)
    {
        moneyText.text = money.ToString();
    }
}

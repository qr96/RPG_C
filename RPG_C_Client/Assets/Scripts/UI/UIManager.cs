using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject playerInfo;
    public GuageBar playerHPBar;
    GuageBar playerMPBar;
    GuageBar playerEXPBar;

    Stack<GuageBar> otherHPBars;
    GuageBar otherHPBarPrefab;

    Stack<TMP_Text> damageTexts;
    TMP_Text damageText;

    private void Awake()
    {
        Instance = this;

        playerInfo = transform.Find("PlayerInfo").gameObject;
        playerHPBar = playerInfo.transform.Find("HPBar").GetComponent<GuageBar>();
        playerMPBar = playerInfo.transform.Find("MPBar").GetComponent <GuageBar>();
        playerEXPBar = transform.Find("EXPBar").GetComponentInChildren<GuageBar>();

        otherHPBars = new Stack<GuageBar>();
        otherHPBarPrefab = transform.Find("HPBar").GetComponent<GuageBar>();
        damageTexts = new Stack<TMP_Text>();
        damageText = transform.Find("DamageText").GetComponent<TMP_Text>();
    }

    public void SetPlayerHPBar(long nowHP, long maxHP)
    {
        playerHPBar.SetAmount(nowHP, maxHP);
    }

    public void SetPlayerEXPBar(long nowEXP, long maxEXP)
    {
        playerEXPBar.SetAmount(nowEXP, maxEXP);
    }

    public GuageBar RentOtherHPBar()
    {
        if (otherHPBars.Count > 0)
            return otherHPBars.Pop();
        else
            return Instantiate(otherHPBarPrefab, transform);
    }

    public void ReturnOtherHPBar(GuageBar bar)
    {
        bar.gameObject.SetActive(false);
        otherHPBars.Push(bar);
    }

    public TMP_Text RentDamageText()
    {
        if (damageTexts.Count > 0)
            return damageTexts.Pop();
        else
            return Instantiate(damageText, transform);
    }

    public void ReturnDamageText(TMP_Text text)
    {
        text.gameObject.SetActive(false);
        damageTexts.Push(text);
    }
}

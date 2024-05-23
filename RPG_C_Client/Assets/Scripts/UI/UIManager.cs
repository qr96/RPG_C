using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    GameObject playerInfo;
    TMP_Text playerLevel;
    GuageBar playerHPBar;
    GuageBar playerMPBar;
    GuageBar playerEXPBar;

    Button menuButton;
    InfoPopup infoPopup;

    ChattingView chattingView;

    ItemSlot hpPotion;
    ItemSlot mpPotion;

    Stack<GuageBar> otherHPBars;
    GuageBar otherHPBarPrefab;

    Stack<TMP_Text> damageTexts;
    TMP_Text damageText;

    private void Awake()
    {
        Instance = this;

        playerInfo = transform.Find("PlayerInfo").gameObject;
        playerLevel = playerInfo.transform.Find("Level").GetComponentInChildren<TMP_Text>();
        playerHPBar = playerInfo.transform.Find("HPBar").GetComponent<GuageBar>();
        playerMPBar = playerInfo.transform.Find("MPBar").GetComponent <GuageBar>();
        playerEXPBar = transform.Find("EXPBar").GetComponentInChildren<GuageBar>();

        otherHPBars = new Stack<GuageBar>();
        otherHPBarPrefab = transform.Find("HPBar").GetComponent<GuageBar>();
        damageTexts = new Stack<TMP_Text>();
        damageText = transform.Find("DamageText").GetComponent<TMP_Text>();

        menuButton = transform.Find("MenuButton").GetComponent<Button>();
        infoPopup = transform.Find("InfoPopup").GetComponent<InfoPopup>();

        chattingView = gameObject.Find<ChattingView>("ChattingView");

        hpPotion = transform.Find("HpPotion").GetComponent<ItemSlot>();
        mpPotion = transform.Find("MpPotion").GetComponent<ItemSlot>();

        menuButton.onClick.AddListener(() => infoPopup.ShowPopup());
    }

    public void SetPlayerLevel(int level)
    {
        playerLevel.text = level.ToString();
    }

    public void SetPlayerHPBar(long nowHP, long maxHP)
    {
        playerHPBar.SetAmount(nowHP, maxHP);
    }

    public void SetPlayerMPBar(long nowMP, long maxMP)
    {
        playerMPBar.SetAmount(nowMP, maxMP);
    }

    public void SetPlayerEXPBar(long nowEXP, long maxEXP)
    {
        playerEXPBar.SetAmount(nowEXP, maxEXP);
    }

    public void SetHpPotion(float coolTime)
    {
        hpPotion.StartCoolTime(coolTime);
    }

    public void SetMpPotion(float coolTime)
    {
        mpPotion.StartCoolTime(coolTime);
    }

    public void SetUserInfoPopup(long money)
    {
        infoPopup.SetMoney(money);
    }

    public void SetSkillTabPopup(int skillPoint, List<int> skillLevels)
    {
        if (infoPopup.isActiveAndEnabled)
            infoPopup.SetSkillTab(skillPoint, skillLevels);
    }

    public void RecvChat(string name, string msg)
    {
        chattingView.RecvChat(name, msg);
    }

    #region Object Pooling
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
    #endregion
}

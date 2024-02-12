using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopupSkillTab : MonoBehaviour
{
    Button learnButton;
    TMP_Text infoText;
    TMP_Text remainPoint;
    List<GameObject> skillList;
    List<int> skillLevels = new List<int>();

    int selected = 0;
    int sp = 0;

    List<string> skillNames = new List<string>()
    {
        "기본공격", "하체강화", "약점포착", "지구력 향상", "불굴"
    };
    List<string> skillEffects = new List<string>()
    {
        "공격력", "이동속도", "치명확률", "최대 스태미너", "정신력"
    };

    private void Awake()
    {
        learnButton = gameObject.Find<Button>("Info/SkillUp");
        infoText = gameObject.Find<TMP_Text>("Info/Scroll View/Viewport/Content/Text");
        remainPoint = gameObject.Find<TMP_Text>("SP/Text");

        skillList = new List<GameObject>();
        for (int i = 0; i < 5; i++)
        {
            skillList.Add(gameObject.Find($"SkillList/Viewport/Content/Skill{i}"));
            skillLevels.Add(0);
        }
        
        for (int i = 0; i < skillList.Count; i++)
        {
            var tmpIndex = i;
            skillList[i].GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectSkill(tmpIndex);
            });
        }

        learnButton.onClick.AddListener(() =>
        {
            SendSkillLevelUp(selected);
        });
    }

    private void Start()
    {
        SelectSkill(0);
    }

    void SelectSkill(int index)
    {
        selected = index;

        for (int i = 0; i < skillList.Count; i++)
        {
            var frame = skillList[i].GetComponent<Image>();
            frame.color = RBUtil.HexToColor(i == selected ? "#EDFF3E" : "#8E8E8E");
        }

        SetSkillInfo(skillNames[selected], skillEffects[selected], skillLevels[selected]);
    }

    void SetSkillInfo(string skillName, string statName, int level)
    {
        var info = $"<color=white><size=36>{skillName}</size></color>\n" +
            $"<color=#B3B3B3>[현재 레벨 {level}]\n" +
            $"{statName} +{level * 2}</color>\n" +
            $"<color=#505050>[다음 레벨]\n" +
            $"{statName} +{level * 2 + 2}]</color>";

        infoText.text = info;
    }

    public void SetSkillLevels(int sp, List<int> skillLevels)
    {
        this.skillLevels = skillLevels;
        SetRemainPoint(sp);
        SelectSkill(selected);
    }

    public void SetRemainPoint(int point)
    {
        sp = point;
        remainPoint.text = $"남은 포인트 : {point}";
        learnButton.enabled = sp > 0;
    }

    #region Packet

    void SendSkillLevelUp(int skillCode)
    {
        C_SkillLevelUp packet = new C_SkillLevelUp();
        packet.SkillCode = skillCode;
        Managers.Network.Send(packet);
    }

    #endregion
}

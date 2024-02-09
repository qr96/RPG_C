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

    int selected = 0;
    int sp = 0;

    List<string> skillNames = new List<string>()
    {
        "�⺻����", "��ü��ȭ", "��������", "������ ���", "�ұ�"
    };
    List<string> skillEffects = new List<string>()
    {
        "���ݷ�", "�̵��ӵ�", "ġ��Ȯ��", "�ִ� ���¹̳�", "���ŷ�"
    };

    private void Awake()
    {
        learnButton = gameObject.Find<Button>("Info/Button");
        infoText = gameObject.Find<TMP_Text>("Info/Scroll View/Viewport/Content/Text");
        remainPoint = gameObject.Find<TMP_Text>("SP/Text");

        skillList = new List<GameObject>();
        for (int i = 0; i < 5; i++)
            skillList.Add(gameObject.Find($"SkillList/Viewport/Content/Skill{i}"));

        for (int i = 0; i < skillList.Count; i++)
        {
            var tmpIndex = i;
            skillList[i].GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectSkill(tmpIndex);
            });
        }
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

        SetSkillInfo(skillNames[selected], skillEffects[selected], 2);
    }

    void SetSkillInfo(string skillName, string statName, int level)
    {
        var info = $"<color=white><size=36>{skillName}</size></color>\n" +
            $"<color=#B3B3B3>[���� ���� {level}]\n" +
            $"{statName} +{level * 2}</color>\n" +
            $"<color=#505050>[���� ����]\n" +
            $"{statName} +{level * 2 + 2}]</color>";

        infoText.text = info;
    }

    public void SetRemainPoint(int point)
    {
        sp = point;
        remainPoint.text = $"���� ����Ʈ : {point}";
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    Image _image;
    Image _coolTime;
    TMP_Text _count;

    Coroutine _coolTimeCo;

    private void Awake()
    {
        _image = transform.Find("Image").GetComponent<Image>();
        _coolTime = transform.Find("CoolTime").GetComponent<Image>();
        _count = transform.Find("Count").GetComponent<TMP_Text>();
    }

    public void SetItem(int itemCode)
    {
        // TODO
        // itemCode에 맞게 이미지 설정
    }

    public void SetCount(int count)
    {
        _count.text = count.ToString();
    }

    public void StartCoolTime(float coolTime)
    {
        if (_coolTimeCo != null)
            StopCoroutine(_coolTimeCo);

        _coolTimeCo = StartCoroutine(CoolTimeCo(coolTime));

        IEnumerator CoolTimeCo(float coolTime)
        {
            float remainTime = coolTime;
            while (remainTime > 0)
            {
                _coolTime.fillAmount = remainTime / coolTime;
                remainTime -= Time.deltaTime;
                yield return null;
            }
            _coolTime.fillAmount = 0f;
        }
    }
}

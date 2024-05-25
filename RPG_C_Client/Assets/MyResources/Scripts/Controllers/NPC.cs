using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    TMP_Text nameTag;

    void Start()
    {
        var tmp = Managers.Resource.Instantiate("UI/PlayerNameTag", UIManager.Instance.transform);
        nameTag = tmp.GetComponent<TMP_Text>();
        nameTag.transform.localScale = Vector3.one;
        nameTag.text = "잡화상인";
    }

    private void Update()
    {
        if (nameTag != null)
            nameTag.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0f, 1.7f, 0f));
    }

    public void OnCollisionEnter(Collision collision)
    {
        
    }
}

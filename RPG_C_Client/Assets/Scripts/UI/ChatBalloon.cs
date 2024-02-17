using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ChatBalloon : MonoBehaviour
{
    TMP_Text chat;
    GameObject target;

    Coroutine showChatCo;
    Action onShowComplete;

    private void Awake()
    {
        chat = gameObject.Find<TMP_Text>("Text");
    }

    private void LateUpdate()
    {
        if (target != null)
            transform.position = Camera.main.WorldToScreenPoint(target.transform.position + new Vector3(0f, 2f, 0f));
    }

    IEnumerator ShowChatCo()
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        if (onShowComplete != null)
            onShowComplete.Invoke();
    }

    public void ShowChat(GameObject target, string message, Action onShowComplete)
    {
        this.target = target;
        chat.text = message;
        this.onShowComplete = onShowComplete;

        if (showChatCo != null)
            StopCoroutine(showChatCo);
        showChatCo = StartCoroutine(ShowChatCo());
    }
}

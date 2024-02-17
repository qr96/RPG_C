using Castle.Core.Internal;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChattingView : MonoBehaviour
{
    RectTransform chatContent;
    TMP_Text chats;
    TMP_InputField input;
    bool inputSelected;

    private void Awake()
    {
        chatContent = gameObject.Find<RectTransform>("Viewport/Content");
        chats = chatContent.gameObject.Find<TMP_Text>("Chats");
        input = gameObject.Find<TMP_InputField>("InputField");

        input.onSelect.AddListener((value) =>
        {
            inputSelected = true;
        });

        input.onEndEdit.AddListener((value) =>
        {
            if (!input.text.IsNullOrEmpty())
                SendChat(value);
            StartCoroutine(DeselectInputCo());
        });

    }

    private void Start()
    {
        chats.text = "";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!inputSelected)
            {
                //input.Select();
                input.ActivateInputField();
            }
        }
    }

    public void RecvChat(string name, string message)
    {
        chats.text += $"\n{name} : {message}";
        chatContent.anchoredPosition = Vector2.zero;
    }

    IEnumerator DeselectInputCo()
    {
        yield return 0;
        input.MoveTextEnd(false);
        input.DeactivateInputField();
        input.text = "";
        EventSystem.current.SetSelectedGameObject(null);
        inputSelected = false;
    }

    #region Packet

    void SendChat(string chat)
    {
        Debug.Log($"Send Chat {chat}");
        C_Chat packet = new C_Chat();
        packet.Chat = chat;
        Managers.Network.Send(packet);
    }

    #endregion
}

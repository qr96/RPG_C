using Castle.Core.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginScene : MonoBehaviour
{
    InputField inputField;
    Button loginButton;

    private void Awake()
    {
        inputField = gameObject.Find<InputField>("NicknameInput");
        loginButton = gameObject.Find<Button>("LoginButton");

        loginButton.onClick.AddListener(() =>
        {
            if (inputField != null && !inputField.text.IsNullOrEmpty())
            {
                SendLoginPacket(inputField.text);
            }
        });
    }

    public void OnRecvLoginResult(int errorCode)
    {
        if (errorCode == 0)
        {
            // 성공
        }
        else if(errorCode == 1)
        {
            // 실패
        }
    }

    #region Pacekt

    void SendLoginPacket(string nickname)
    {

    }

    #endregion
}

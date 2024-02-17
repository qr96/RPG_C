using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OtherPlayer : MonoBehaviour
{
    public int Id
    {
        set
        {
            id = value;
            if(nameTag != null)
                nameTag.text = id.ToString();
        }
        get { return id; }
    }
    private int id;

    public float speed = 1f;
    public float rotateSpeed = 2f;

    Rigidbody rigid;
    GameObject avatar;
    Animator animator;
    TMP_Text nameTag;
    ChatBalloon chatBalloon;

    public Vector3 desPos;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        avatar = transform.Find("Avatar").gameObject;
        animator = avatar.GetComponent<Animator>();
    }

    private void Update()
    {
        if (desPos != Vector3.zero)
        {
            //var movePos = Vector3.Lerp(transform.position, desPos, Time.deltaTime * speed);
            var movePos = Vector3.MoveTowards(transform.position, desPos, Time.deltaTime * speed);
            var lookVec = RBUtil.RemoveY(desPos - transform.position);
            if (lookVec != Vector3.zero)
            {
                var lookRot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookVec), Time.deltaTime * rotateSpeed);
                transform.SetPositionAndRotation(movePos, lookRot);
            }
            else
                transform.position = movePos;

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                if (RBUtil.RemoveY(desPos - transform.position).sqrMagnitude > 0.01f)
                    animator.Play("Move");
                else
                    animator.Play("Idle");
            }
        }

        if (nameTag != null)
            nameTag.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0f, 1.7f, 0f));
    }

    private void OnEnable()
    {
        var tmp = Managers.Resource.Instantiate("UI/PlayerNameTag", UIManager.Instance.transform);
        nameTag = tmp.GetComponent<TMP_Text>();
        nameTag.transform.localScale = Vector3.one;
        nameTag.text = Id.ToString();
    }

    private void OnDisable()
    {
        if (nameTag != null)
            Managers.Resource.Destroy(nameTag.gameObject);
        nameTag = null;
    }

    public void SetDesPos(Vector3 desPos)
    {
        this.desPos = desPos;
    }

    public void AttackMotion()
    {
        animator.Play("Attack");
    }

    public void ShowChatBalloon(string chat)
    {
        if (chatBalloon == null)
        {
            var go = Managers.Resource.Instantiate("UI/ChatBalloon", UIManager.Instance.transform);
            if (go == null)
                return;

            go.transform.localScale = Vector3.one;
            chatBalloon = go.GetComponent<ChatBalloon>();
        }

        chatBalloon.ShowChat(gameObject, chat, () =>
        {
            Managers.Resource.Destroy(chatBalloon.gameObject);
            chatBalloon = null;
        });
    }
}

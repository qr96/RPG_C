using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyPlayer : MonoBehaviour
{
    public int Id
    {
        set
        {
            id = value;
            if (nameTag != null)
                nameTag.text = id.ToString();
        }
        get { return id; }
    }

    private int id;
    public float speed;
    public float attackDelay;

    Rigidbody rigid;
    GameObject avatar;
    Animator animator;
    GameObject attackImpact;
    TMP_Text nameTag;
    ChatBalloon chatBalloon;

    DateTime attackEnd;
    Vector2 input;

    Dictionary<int, DateTime> itemCoolEndTimes = new Dictionary<int, DateTime>();

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        avatar = transform.Find("Avatar").gameObject;
        animator = avatar.GetComponent<Animator>();
        attackImpact = transform.Find("AttackImpact").gameObject;
    }

    private void Start()
    {
        StartCoroutine(SendMoveCo());
        itemCoolEndTimes.Add(1, DateTime.MinValue);
        itemCoolEndTimes.Add(2, DateTime.MinValue);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && DateTime.Now > itemCoolEndTimes[1])
        {
            itemCoolEndTimes[1] = DateTime.Now.AddSeconds(0.3f);
            UIManager.Instance.SetHpPotion(0.3f);
            SendUseItem(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && DateTime.Now > itemCoolEndTimes[2])
        {
            itemCoolEndTimes[2] = DateTime.Now.AddSeconds(0.3f);
            UIManager.Instance.SetMpPotion(0.3f);
            SendUseItem(2);
        }
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y + 16f, transform.position.z - 16f);

        if (nameTag != null)
            nameTag.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0f, 1.7f, 0f));
    }

    private void FixedUpdate()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");
        }
        else
        {
            input = Vector2.zero;
        }

        if (DateTime.Now >= attackEnd)
        {
            attackImpact.SetActive(false);
            rigid.velocity = new Vector3(input.x * speed * Time.fixedDeltaTime, rigid.velocity.y, input.y * speed * Time.fixedDeltaTime);

            if (input.x != 0 || input.y != 0)
            {
                animator.Play("Move");
                rigid.rotation = Quaternion.LookRotation(new Vector3(input.x, 0f, input.y));
            }
            else
            {
                animator.Play("Idle");
            }
        }
    }

    private void OnEnable()
    {
        var tmp = Managers.Resource.Instantiate("UI/PlayerNameTag", UIManager.Instance.transform);
        nameTag = tmp.GetComponent<TMP_Text>();
        nameTag.transform.localScale = Vector3.one;

        var tmpText = nameTag.GetComponent<TMP_Text>();
        tmpText.text = Id.ToString();
    }

    private void OnDisable()
    {
        if (nameTag != null)
            Managers.Resource.Destroy(nameTag.gameObject);
        nameTag = null;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "Enemy" && DateTime.Now >= attackEnd)
        {
            var enemy = collision.transform.GetComponent<Monster>();
            var enemyVec = RBUtil.RemoveY(enemy.transform.position - transform.position);
            var inputVec = new Vector3(input.x, 0f, input.y);
            int attackDir = 0;

            var deg = Mathf.Abs(Vector3.Angle(enemyVec, inputVec));

            if (deg > 90f)
                return;

            if (input.x == 0 && input.y == 0)
            {
                if (!enemy.IsTargetGo(gameObject))
                    return;

                attackDir = RBUtil.AttackVecToDirec(enemyVec);
                rigid.rotation = Quaternion.LookRotation(enemyVec);
            }
            else
            {
                attackDir = RBUtil.AttackVecToDirec(input);
            }

            SendAttack(attackDir, enemy.Id);
            enemy.OnDamagedClient(gameObject, attackDir);

            animator.Play("Attack");
            attackEnd = DateTime.Now.AddSeconds(attackDelay);
            rigid.velocity = Vector3.zero;

            attackImpact.transform.position = collision.transform.position;
            attackImpact.SetActive(true);
        }
    }

    IEnumerator SendMoveCo()
    {
        Vector3 prevPos = transform.position;
        while (true)
        {
            if (prevPos != transform.position)
            {
                SendMove(transform.position);
                prevPos = transform.position;
            }
            yield return new WaitForSeconds(0.2f);
        }
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

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetAttackDelay(float delay)
    {
        this.attackDelay = delay;
    }

    #region packet

    private void SendAttack(int direction, int targetId)
    {
        C_Attack packet = new C_Attack();
        packet.Direction = direction;
        packet.TargetId = targetId;
        Managers.Network.Send(packet);
    }

    private void SendSkill(int skillId, int targetId)
    {
        C_Skill skill = new C_Skill();
        skill.SkillId = skillId;
        skill.TargetId = targetId;
        Managers.Network.Send(skill);
    }

    private void SendMove(Vector3 pos)
    {
        C_Move move = new C_Move();
        move.PosInfo = new PositionInfo() { PosX = pos.x, PosY = pos.y, PosZ = pos.z };
        Managers.Network.Send(move);
    }

    private void SendUseItem(int itemId)
    {
        C_UseItem item = new C_UseItem();
        item.ItemCode = itemId;
        Managers.Network.Send(item);
    }

    #endregion
}

using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : MonoBehaviour
{
    public int Id;

    // Must Move to Server
    public float speed;
    public float attackDelay;
    public long attack;
    public long nowHP;
    public long maxHP;    
    public long nowEXP;
    public long maxEXP;

    Rigidbody rigid;
    GameObject avatar;
    Animator animator;
    GameObject attackImpact;

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
        //UIManager.Instance.SetPlayerHPBar(nowHP, maxHP);
        //UIManager.Instance.SetPlayerEXPBar(nowEXP, maxEXP);
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
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y + 16, transform.position.z - 16);
    }

    private void FixedUpdate()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

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

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "Enemy" && DateTime.Now >= attackEnd)
        {
            var enemy = collision.transform.GetComponent<Monster>();
            var enemyVec = RBUtil.RemoveY(enemy.transform.position - transform.position);
            var inputVec = new Vector3(input.x, 0f, input.y);

            if (Mathf.Abs(Mathf.Acos(Vector3.Dot(inputVec, enemyVec) / (inputVec.magnitude * enemyVec.magnitude) * Mathf.Rad2Deg)) > 90f)
                return;

            if (input.x == 0 && input.y == 0)
            {
                rigid.rotation = Quaternion.LookRotation(enemyVec);
                enemy.OnDamagedClient(gameObject, enemyVec);
            }
            else
            {
                enemy.OnDamagedClient(gameObject, inputVec);
            }

            SendSkill(1, enemy.Id);

            animator.Play("Attack");
            attackEnd = DateTime.Now.AddSeconds(attackDelay);
            rigid.velocity = Vector3.zero;

            attackImpact.transform.position = collision.transform.position;
            attackImpact.SetActive(true);
        }
    }

    IEnumerator SendMoveCo()
    {
        while (true)
        {
            SendMove(transform.position);
            yield return new WaitForSeconds(0.2f);
        }
    }

    #region packet

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

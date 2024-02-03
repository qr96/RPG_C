using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MyPlayer : MonoBehaviour
{
    public int ID;

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

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        avatar = transform.Find("Avatar").gameObject;
        animator = avatar.GetComponent<Animator>();
        attackImpact = transform.Find("AttackImpact").gameObject;
    }

    private void Start()
    {
        Managers.Object.MyPlayer = this;
        StartCoroutine(SendMoveCo());
        //UIManager.Instance.SetPlayerHPBar(nowHP, maxHP);
        //UIManager.Instance.SetPlayerEXPBar(nowEXP, maxEXP);
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
                animator.Play("Run");
                transform.rotation = Quaternion.LookRotation(new Vector3(input.x, 0f, input.y));
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
            var enemy = collision.transform.GetComponent<Enemy>();
            var enemyVec = RBUtil.RemoveY(enemy.transform.position - transform.position);
            var inputVec = new Vector3(input.x, 0f, input.y);

            if (Mathf.Abs(Mathf.Acos(Vector3.Dot(inputVec, enemyVec) / (inputVec.magnitude * enemyVec.magnitude))) > 90f)
                return;

            if (input.x == 0 && input.y == 0)
            {
                transform.rotation = Quaternion.LookRotation(enemyVec);
                enemy.OnDamaged(enemyVec, attack, this);
            }
            else
                enemy.OnDamaged(inputVec, attack, this);

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

    private void SendSkill(int skillId, bool facingRight, Vector2 pos)
    {
        C_Skill skill = new C_Skill() { Info = new SkillInfo() };
        skill.Info.SkillId = skillId;
        skill.Position = new PositionInfo() { PosX = pos.x, PosY = pos.y };
        skill.FacingRight = facingRight;
        Managers.Network.Send(skill);
    }

    private void SendMove(Vector3 pos)
    {
        C_Move move = new C_Move();
        move.PosInfo = new PositionInfo() { PosX = pos.x, PosY = pos.y, PosZ = pos.z };
        Managers.Network.Send(move);
    }

    #endregion

    public void OnDamage(long damage)
    {
        //Server
        nowHP -= damage;

        UIManager.Instance.SetPlayerHPBar(nowHP, maxHP);
    }

    public void AddEXP(long exp)
    {
        nowEXP += exp;
        UIManager.Instance.SetPlayerEXPBar(nowEXP, maxEXP);
    }
}

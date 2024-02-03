using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Server
    public float moveSpeed = 1f;
    public long maxHp;
    public long nowHp;
    public long attack;
    public float pushPower = 0.5f;
    public List<Vector3> movePoints;
    public double maxChaseDis;
    public float knockBackDelay;
    int nowMovePoint;
    DateTime idleTime;
    DateTime moveTime;
    DateTime knockBackTime;
    GameObject target;

    StateMachine<MonsterState> sm;

    public enum MonsterState
    {
        None,
        Idle,
        Move,
        Chase,
        Damaged,
        Die
    }

    public Vector3 desPos;

    GuageBar hpBar;
    public GameObject deathEffect;

    Rigidbody rigid;
    GameObject body;
    Animator animator;

    public float hpBarDuration = 5f;
    DateTime hideHpBarTime;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        body = transform.Find("Body").gameObject;
        animator = body.GetComponent<Animator>();
        desPos = transform.position;

        System.Random random = new System.Random();

        sm = new StateMachine<MonsterState>();
        sm.SetEvent(MonsterState.Idle,
            (prev) =>
            {
                idleTime = DateTime.Now.AddSeconds(random.Next(3, 6));
            },
            () =>
            {
                if (DateTime.Now > idleTime)
                    sm.SetState(MonsterState.Move);
            });
        sm.SetEvent(MonsterState.Move,
            (prev) =>
            {
                moveTime = DateTime.Now.AddSeconds(random.Next(5, 10));
                nowMovePoint = random.Next(movePoints.Count);
                desPos = movePoints[nowMovePoint];
            },
            () =>
            {
                if (DateTime.Now > moveTime)
                    sm.SetState(MonsterState.Idle);
            });
        sm.SetEvent(MonsterState.Chase, null,
            () =>
            {
                if ((transform.position - movePoints[nowMovePoint]).sqrMagnitude > maxChaseDis)
                    sm.SetState(MonsterState.Move);
                else if (target != null)
                    desPos = target.transform.position;
                else
                    sm.SetState(MonsterState.Idle);
            });
        sm.SetEvent(MonsterState.Damaged, null,
            () =>
            {
                if (DateTime.Now > knockBackTime)
                    sm.SetState(MonsterState.Chase);
            });

        Spawn();
    }

    private void Update()
    {
        sm.Update();

        var lookVec = RBUtil.RemoveY(desPos - transform.position);
        if (lookVec != Vector3.zero && sm.GetState() != MonsterState.Damaged)
            transform.rotation = Quaternion.LookRotation(lookVec);
        transform.position = Vector3.MoveTowards(transform.position, RBUtil.InsertY(desPos, transform.position.y), Time.deltaTime * moveSpeed);

        if (RBUtil.RemoveY(desPos - transform.position).sqrMagnitude > 1f)
            animator.Play("Move");
        else
            animator.Play("Idle");

        if (hpBar != null)
        {
            if (DateTime.Now < hideHpBarTime)
                hpBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0f, 1.5f, 0f));
            else
            {
                UIManager.Instance.ReturnOtherHPBar(hpBar);
                hpBar = null;
            }
        }
    }

    /*
    private void FixedUpdate()
    {
        if (sm.GetState() == MonsterState.Damaged)
            rigid.MovePosition(Vector3.Lerp(transform.position, new Vector3(desPos.x, transform.position.y, desPos.z), Time.fixedDeltaTime * knockBackSpeed));
        else
            rigid.MovePosition(Vector3.Lerp(transform.position, new Vector3(desPos.x, transform.position.y, desPos.z), Time.fixedDeltaTime * moveSpeed));
    }
    */

    void ShowDamageText(long damage)
    {
        var damageText = UIManager.Instance.RentDamageText();
        damageText.gameObject.SetActive(true);
        damageText.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0f, 1.5f, 0f));
        damageText.text = damage.ToString();
        damageText.alpha = 1f;
        damageText.rectTransform.DOAnchorPosY(200, 2f);
        damageText.DOFade(0f, 2f).OnComplete(() =>
        {
            damageText.gameObject.SetActive(false);
            UIManager.Instance.ReturnDamageText(damageText);
        });
    }

    void OnDead(MyPlayer attacker)
    {
        //Server
        attacker.AddEXP(10);

        deathEffect.transform.position = transform.position;
        deathEffect.SetActive(true);
        gameObject.SetActive(false);

        if (hpBar != null)
        {
            UIManager.Instance.ReturnOtherHPBar(hpBar);
            hpBar = null;
        }
    }

    public void OnDamaged(Vector3 direction, long damage, MyPlayer attacker)
    {
        //Server
        knockBackTime = DateTime.Now.AddSeconds(knockBackDelay);
        sm.SetState(MonsterState.Damaged);
        nowHp -= damage;
        target = attacker.gameObject;
        attacker.OnDamage(attack);

        Vector3 pushVec = direction.normalized * pushPower;
        desPos = transform.position + pushVec;
        transform.rotation = Quaternion.LookRotation(-pushVec);

        if (hpBar == null)
        {
            hpBar = UIManager.Instance.RentOtherHPBar();
            hpBar.gameObject.SetActive(true);
        }
        hpBar.SetAmount(nowHp, maxHp);
        hideHpBarTime = DateTime.Now.AddSeconds(hpBarDuration);
        ShowDamageText(damage);

        if (nowHp <= 0)
            OnDead(attacker);
    }

    public void Spawn()
    {
        //Server
        nowHp = maxHp;

        gameObject.SetActive(true);
        sm.SetState(MonsterState.Idle);
    }
}

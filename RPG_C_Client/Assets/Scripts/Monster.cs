using DG.Tweening;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public int Id;

    public float speed = 2f;
    public float pushPower = 0.5f;
    public float hpBarDuration = 5f;
    public float knockBackDuration;

    public Vector3 desPos;

    DateTime knockBackTime;
    DateTime hideHpBarTime;

    StateMachine<MonsterState> sm;
    GameObject body;
    Animator animator;

    GameObject target;
    GuageBar hpBar;

    public enum MonsterState
    {
        None,
        Idle,
        Move,
        Chase,
        Damaged,
        Die
    }

    private void Awake()
    {
        body = transform.Find("Body").gameObject;
        animator = body.GetComponent<Animator>();

        sm = new StateMachine<MonsterState>();
        sm.SetEvent(MonsterState.Chase, null,
            () =>
            {
                if (target != null)
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
    }

    private void Start()
    {
        desPos = transform.position;
    }

    private void Update()
    {
        sm.Update();

        if (sm.GetState() != MonsterState.Damaged)
        {
            // desPos 쳐다보기
            var lookVec = RBUtil.RemoveY(desPos - transform.position);
            if (lookVec != Vector3.zero && sm.GetState() != MonsterState.Damaged)
                transform.rotation = Quaternion.LookRotation(lookVec);
        }

        // desPos로 이동
        if (sm.GetState() != MonsterState.Damaged)
            transform.position = Vector3.MoveTowards(transform.position, RBUtil.InsertY(desPos, transform.position.y), speed * Time.deltaTime);
        else
            transform.position = Vector3.Lerp(transform.position, RBUtil.InsertY(desPos, transform.position.y), speed * Time.deltaTime);

        // 이동 animation
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

    public void RecvMonsterState(S_MonsterState monsterState)
    {
        sm.SetState((MonsterState)monsterState.State);
        desPos = RBUtil.PosToVector3(monsterState.NowPos);

        if (monsterState.TargetId != 0)
            target = Managers.Object.FindById(monsterState.TargetId);
    }

    public void OnDamagedClient(GameObject attacker, Vector3 direction)
    {
        if (attacker != null) target = attacker;

        Vector3 pushVec = direction.normalized * pushPower;
        desPos = transform.position + pushVec;
        transform.rotation = Quaternion.LookRotation(-direction);

        knockBackTime = DateTime.Now.AddSeconds(knockBackDuration);
        sm.SetState(MonsterState.Damaged);
    }

    public void OnDamagedServer(GameObject attacker, long damage, long nowHp, long maxHp)
    {
        if (attacker != null)
            target = attacker;
        else
            target = null;

        hideHpBarTime = DateTime.Now.AddSeconds(hpBarDuration);
        if (hpBar == null)
        {
            hpBar = UIManager.Instance.RentOtherHPBar();
            hpBar.gameObject.SetActive(true);
        }
        hpBar.SetAmount(nowHp, maxHp);
        ShowDamageText(damage);
    }
}

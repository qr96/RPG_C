using DG.Tweening;
using Google.Protobuf.Protocol;
using System;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public int Id;

    public float speed = 2f;
    public float pushPower = 0.5f;
    public float hpBarDuration = 5f;
    public float knockBackDuration;

    public Vector3 desPos;
    Vector3 bodyPos;
    DateTime knockBackTime;
    DateTime hideHpBarTime;

    StateMachine<MonsterState> sm;
    GameObject body;
    Rigidbody rigid;
    Animator animator;

    public GameObject target;
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
        rigid = GetComponent<Rigidbody>();
        body = transform.Find("Body").gameObject;
        animator = body.GetComponent<Animator>();
        bodyPos = body.transform.localPosition;

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

    private void FixedUpdate()
    {
        if (sm.GetState() != MonsterState.Damaged && RBUtil.RemoveY(desPos - transform.position).sqrMagnitude > 0.01f)
        {
            // desPos 쳐다보기
            var lookVec = RBUtil.RemoveY(desPos - transform.position);
            if (lookVec != Vector3.zero && sm.GetState() != MonsterState.Damaged)
                rigid.rotation = Quaternion.LookRotation(lookVec);

            // desPos로 이동
            rigid.velocity = RBUtil.InsertY(desPos - transform.position, rigid.velocity.y).normalized * speed;
        }

        // 이동 animation
        if (sm.GetState() != MonsterState.Damaged && RBUtil.RemoveY(desPos - transform.position).sqrMagnitude > 1f)
            animator.Play("Move");
        else
            animator.Play("Idle");

        body.transform.localPosition = bodyPos;
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

    void OnDead()
    {
        EffectPool.Instance.ShowDeadEffect(transform.position);

        // hpBar 반환
        if (hpBar != null)
        {
            UIManager.Instance.ReturnOtherHPBar(hpBar);
            hpBar = null;
        }

        Managers.Object.Remove(Id);
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

        sm.SetState(MonsterState.Damaged);

        Vector3 pushVec = direction.normalized * pushPower;
        rigid.velocity = RBUtil.InsertY(Vector3.zero, rigid.velocity.y);
        rigid.AddForce(pushVec, ForceMode.Impulse);
        rigid.rotation = Quaternion.LookRotation(-direction);

        knockBackTime = DateTime.Now.AddSeconds(knockBackDuration);
    }

    public void OnDamagedServer(GameObject attacker, long damage, long nowHp, long maxHp)
    {
        if (attacker != null)
            target = attacker;
        else
            target = null;

        sm.SetState(MonsterState.Damaged);

        hideHpBarTime = DateTime.Now.AddSeconds(hpBarDuration);
        if (hpBar == null)
        {
            hpBar = UIManager.Instance.RentOtherHPBar();
            hpBar.gameObject.SetActive(true);
        }
        hpBar.SetAmount(nowHp, maxHp);
        ShowDamageText(damage);

        if (nowHp <= 0)
            OnDead();
    }
}

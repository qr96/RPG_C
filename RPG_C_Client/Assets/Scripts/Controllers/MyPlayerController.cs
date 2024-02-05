using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MyPlayerController : MonoBehaviour
{
    public int id;

    public float speed = 3f;
    public float jumpPower = 8f;
    public Rigidbody2D rigid;

    public GameObject body;
    public Animator animator;

    StateMachine<PlayerState> stateMachine;

    // skill
    DateTime lastAttack;
    public float attackDelay = 0.6f;

    float horizontal;
    bool isGround;
    bool facingRight = true;

    enum PlayerState
    {
        None,
        Idle,
        MoveRight,
        MoveLeft,
        Jumping,
        Attack,
        Attacked,
        Die,
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        body = transform.Find("Body").gameObject;
        animator = body.GetComponent<Animator>();

        //Managers.Object.MyPlayer = this;
        stateMachine = new StateMachine<PlayerState>();

        stateMachine.SetEvent(PlayerState.Idle, (prev) =>
        {

        }, () =>
        {
            if (Input.GetKey(KeyCode.RightArrow))
                stateMachine.SetState(PlayerState.MoveRight);
            else if (Input.GetKey(KeyCode.LeftArrow))
                stateMachine.SetState(PlayerState.MoveLeft);

            if ((DateTime.Now - lastAttack).TotalSeconds >= attackDelay)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    lastAttack = DateTime.Now;
                    stateMachine.SetState(PlayerState.Attack);
                }
            }

            if (isGround && Input.GetKey(KeyCode.LeftAlt))
                Jump();
        });
        stateMachine.SetEvent(PlayerState.MoveRight, (prev) =>
        {
            if (!facingRight)
            {
                Vector2 localScale = transform.localScale;
                localScale.x = -localScale.x;
                transform.localScale = localScale;
                facingRight = true;
            }
        }, () =>
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                stateMachine.SetState(PlayerState.MoveLeft);
            else if (Input.GetKeyUp(KeyCode.RightArrow))
                stateMachine.SetState(PlayerState.Idle);

            if ((DateTime.Now - lastAttack).TotalSeconds >= attackDelay)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    stateMachine.SetState(PlayerState.Attack);
            }

            if (isGround && Input.GetKey(KeyCode.LeftAlt))
                Jump();
        }, () =>
        {
        });
        stateMachine.SetEvent(PlayerState.MoveLeft, (prev) =>
        {
            if (facingRight)
            {
                Vector2 localScale = transform.localScale;
                localScale.x = -localScale.x;
                transform.localScale = localScale;
                facingRight = false;
            }
        }, () =>
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
                stateMachine.SetState(PlayerState.MoveRight);
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
                stateMachine.SetState(PlayerState.Idle);

            if ((DateTime.Now - lastAttack).TotalSeconds >= attackDelay)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    stateMachine.SetState(PlayerState.Attack);
            }

            if (isGround && Input.GetKey(KeyCode.LeftAlt))
                Jump();
        }, () =>
        {
        });
        stateMachine.SetEvent(PlayerState.Attack, (prev) =>
        {
            animator.Play("Attack");
            lastAttack = DateTime.Now;
            SendSkill(2, facingRight, transform.position);
        }, () =>
        {
            if ((DateTime.Now - lastAttack).TotalSeconds >= attackDelay)
                stateMachine.SetState(PlayerState.Idle);
        });

        stateMachine.SetState(PlayerState.Idle);
    }

    private void Update()
    {
        stateMachine.Update();

        /* 모바일 대각선 위해 남겨둠
         if (Input.GetKey(KeyCode.RightArrow))
                    rigid.AddForce(new Vector2(speed * 0.1f, 1f) * jumpPower, ForceMode2D.Impulse);
                else if (Input.GetKey(KeyCode.LeftArrow))
                    rigid.AddForce(new Vector2(-speed * 0.1f, 1f) * jumpPower, ForceMode2D.Impulse);
                else
                    rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
         */
    }

    private void FixedUpdate()
    {
        if (stateMachine.GetState() == PlayerState.MoveRight)
            rigid.velocity = new Vector2(speed, rigid.velocity.y);
        else if (stateMachine.GetState() == PlayerState.MoveLeft)
            rigid.velocity = new Vector2(-speed, rigid.velocity.y);
    }

    void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (rigid.velocity.y <= 0)
                isGround = true;
        }
    }

    #region packet

    private void SendSkill(int skillId, bool facingRight, Vector2 pos)
    {

    }

    #endregion

    private void Jump()
    {
        //SendMoveCommand(MoveCommandType.MoveJump, true, transform.position);
        isGround = false;
        rigid.velocity = new Vector2(rigid.velocity.x, 0f);
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }
}

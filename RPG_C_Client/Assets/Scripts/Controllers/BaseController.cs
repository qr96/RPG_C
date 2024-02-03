using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public int id;

    public float speed = 3f;
    public float jumpPower = 8f;
    public Rigidbody2D rigid;

    public GameObject body;
    public Animator animator;

    StateMachine<PlayerState> stateMachine;

    bool isGround;
    public bool facingRight = true;

    public enum PlayerState
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

        stateMachine = new StateMachine<PlayerState>();

        stateMachine.SetEvent(PlayerState.MoveRight, (prev) =>
        {
            if (!facingRight)
            {
                Vector2 localScale = transform.localScale;
                localScale.x = -localScale.x;
                transform.localScale = localScale;
                facingRight = true;
            }
        }, null, null);
        stateMachine.SetEvent(PlayerState.MoveLeft, (prev) =>
        {
            if (facingRight)
            {
                Vector2 localScale = transform.localScale;
                localScale.x = -localScale.x;
                transform.localScale = localScale;
                facingRight = false;
            }
        }, null, null);
        stateMachine.SetEvent(PlayerState.Attack, (prev) =>
        {
            animator.Play("Attack");
        }, null);

        stateMachine.SetState(PlayerState.Idle);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        if (stateMachine.GetState() == PlayerState.MoveRight)
            rigid.velocity = new Vector2(speed, rigid.velocity.y);
        else if (stateMachine.GetState() == PlayerState.MoveLeft)
            rigid.velocity = new Vector2(-speed, rigid.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (rigid.velocity.y <= 0)
                isGround = true;
        }
    }

    public void Jump()
    {
        if (isGround)
        {
            isGround = false;
            rigid.velocity = new Vector2(rigid.velocity.x, 0f);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    public void JumpSide(bool isRight)
    {
        if (isGround)
        {
            isGround = false;
            rigid.velocity = Vector2.zero;

            if (isRight)
                rigid.AddForce(new Vector2(speed * 0.125f, 1f) * jumpPower, ForceMode2D.Impulse);
            else
                rigid.AddForce(new Vector2(-speed * 0.125f, 1f) * jumpPower, ForceMode2D.Impulse);
        }
    }

    public void SetState(PlayerState state)
    {
        stateMachine.SetState(state);
    }

    public PlayerState GetState()
    {
        return stateMachine.GetState();
    }
}

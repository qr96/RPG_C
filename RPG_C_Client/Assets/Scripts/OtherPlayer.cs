using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayer : MonoBehaviour
{
    public int ID;

    // Must Move to Server
    public float speed = 200f;

    Rigidbody rigid;
    GameObject avatar;
    Animator animator;

    Vector3 desPos;

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
            rigid.MovePosition(Vector3.Lerp(transform.position, RBUtil.InsertY(desPos, transform.position.y), Time.deltaTime * speed));
            transform.rotation = Quaternion.LookRotation(RBUtil.RemoveY(desPos - transform.position));

            if (RBUtil.RemoveY(desPos - transform.position).sqrMagnitude > 1f)
                animator.Play("Move");
            else
                animator.Play("Idle");
        }
    }

    public void SetDesPos(Vector3 desPos)
    {
        this.desPos = desPos;
    }
}

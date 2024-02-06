using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayer : MonoBehaviour
{
    public int ID;

    public float speed = 1f;
    public float rotateSpeed = 2f;

    Rigidbody rigid;
    GameObject avatar;
    Animator animator;

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
            transform.position = Vector3.Lerp(transform.position, desPos, Time.deltaTime * speed);
            var lookVec = RBUtil.RemoveY(desPos - transform.position);
            if (lookVec != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookVec), Time.deltaTime * rotateSpeed);

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

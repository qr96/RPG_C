using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : MonoBehaviour
{
    public int id;

    public Vector2 desPos;
    GameObject body;

    private void Awake()
    {
        body = transform.Find("Body").gameObject;
    }

    private void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, desPos, Time.fixedDeltaTime);
    }

    public void SetDesPos(Vector2 pos)
    {
        desPos = pos;
    }

    public void Attacked()
    {
        
    }
}

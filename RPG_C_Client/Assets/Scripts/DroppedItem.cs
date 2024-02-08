using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public int Id;
    public Action OnColliderEvent;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag.Equals("Player"))
        {
            if (OnColliderEvent != null)
                OnColliderEvent.Invoke();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public int Id;
    public Action OnColliderEvent;

    GameObject itemTag;

    private void OnEnable()
    {
        itemTag = Managers.Resource.Instantiate("UI/ItemTag", UIManager.Instance.transform);
    }

    private void OnDisable()
    {
        Managers.Resource.Destroy(itemTag);
    }

    private void Update()
    {
        itemTag.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0f, 1.5f, 0f));
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag.Equals("Player"))
        {
            if (OnColliderEvent != null)
                OnColliderEvent.Invoke();
        }
    }
}

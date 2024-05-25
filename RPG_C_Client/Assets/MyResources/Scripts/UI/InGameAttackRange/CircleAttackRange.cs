using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleAttackRange : MonoBehaviour
{
    public Image fill;
    bool show = true;
    float time = 3f;
    float timer = 0f;
    Action onComplete;

    public void Init(float time, Action onComplete)
    {
        show = false;
        fill.transform.localScale = Vector3.zero;
        this.time = time;
        this.onComplete = onComplete;
    }

    public void Show()
    {
        show = true;
    }

    private void Update()
    {
        if (show && time > 0)
        {
            var percent = timer / time;
            fill.transform.localScale = new Vector3(percent, percent, percent);
            timer += Time.deltaTime;

            if (timer >= time)
            {
                show = false;
                if (onComplete != null)
                    onComplete();
            }
        }
    }
}

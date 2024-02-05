using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoCallBack : MonoBehaviour
{
    public Action onDisable;

    private void OnDisable()
    {
        if (onDisable != null) onDisable();
    }
}

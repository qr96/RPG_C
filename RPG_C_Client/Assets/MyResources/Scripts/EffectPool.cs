using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    public static EffectPool Instance;

    Stack<MonoCallBack> deadEffectPool = new Stack<MonoCallBack>();

    private void Awake()
    {
        Instance = this;
    }

    public void ShowDeadEffect(Vector3 position)
    {
        MonoCallBack deadEffect;
        if (deadEffectPool.Count > 0)
            deadEffect = deadEffectPool.Pop();
        else
        {
            var instantiated = Managers.Resource.Instantiate("Effect/DeadEffect");
            deadEffect = instantiated.GetComponent<MonoCallBack>();
        }
        deadEffect.onDisable = () =>
        {
            deadEffect.gameObject.SetActive(false);
            deadEffectPool.Push(deadEffect);
        };
        deadEffect.transform.position = position;
        deadEffect.gameObject.SetActive(true);
    }
}

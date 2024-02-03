using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCreature : MonoBehaviour
{
    public int id;

    enum CreatureState
    {
        None,
        IdleFront,
        IdleBack,
        IdleLeft,
        IdleRight,
        MoveFront,
        MoveBack,
        MoveLeft,
        MoveRight,
    }
}

using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// RPG_B 프로젝트만을 위한 유틸
public class RBUtil
{
    // y축 쓰지 않음, 전적으로 유니티 물리엔진 의존
    public static Vector3 RemoveY(Vector3 vec)
    {
        return new Vector3(vec.x, 0f, vec.z);
    }

    public static Vector3 InsertY(Vector3 vec, float y)
    {
        return new Vector3(vec.x, y, vec.z);
    }

    public static Color HexToColor(string hex)
    {
        hex = hex.Replace("#", "");
        if (hex.Length != 6 || hex.Length != 8)
        {
            Debug.LogError("올바른 HEX 값이 아닙니다.");
            return Color.white;
        }

        float r = System.Convert.ToInt32(hex.Substring(0, 2), 16) / 255f;
        float g = System.Convert.ToInt32(hex.Substring(2, 2), 16) / 255f;
        float b = System.Convert.ToInt32(hex.Substring(4, 2), 16) / 255f;
        float a = hex.Length == 8 ? System.Convert.ToInt32(hex.Substring(6, 2), 16) / 255f : 1f;

        return new Color(r, g, b, a);
    }

    public static Vector3 PosToVector3(PositionInfo posInfo)
    {
        return new Vector3(posInfo.PosX, posInfo.PosY, posInfo.PosZ);
    }
}

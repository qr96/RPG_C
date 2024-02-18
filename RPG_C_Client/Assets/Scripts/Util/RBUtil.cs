using Google.Protobuf.Protocol;
using System;
using UnityEngine;

// RPG_B 프로젝트만을 위한 유틸
public class RBUtil
{
    const float COS45 = 0.70710678f;

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
        if (hex.Length != 6 && hex.Length != 8)
        {
            Debug.LogError("올바른 HEX 값이 아닙니다.");
            return Color.white;
        }

        float r = Convert.ToInt32(hex.Substring(0, 2), 16) / 255f;
        float g = Convert.ToInt32(hex.Substring(2, 2), 16) / 255f;
        float b = Convert.ToInt32(hex.Substring(4, 2), 16) / 255f;
        float a = hex.Length == 8 ? System.Convert.ToInt32(hex.Substring(6, 2), 16) / 255f : 1f;

        return new Color(r, g, b, a);
    }

    public static Vector3 PosToVector3(PositionInfo posInfo)
    {
        return new Vector3(posInfo.PosX, posInfo.PosY, posInfo.PosZ);
    }

    public static Vector2 AttackDirec2(int direction)
    {
        switch(direction)
        {
            case 0:
                return Vector2.zero;
            case 1:
                return new Vector2(0f, -1f);
            case 2:
                return new Vector2(-COS45, -COS45);
            case 3:
                return new Vector2(-1f, 0f);
            case 4:
                return new Vector2(-COS45, COS45);
            case 5:
                return new Vector2(0f, 1f);
            case 6:
                return new Vector2(COS45, COS45);
            case 7:
                return new Vector2(1f, 0f);
            case 8:
                return new Vector2(COS45, -COS45);
            default:
                return Vector2.zero;
        }
    }

    public static Vector3 AttackDirec3(int direction)
    {
        var vec2 = AttackDirec2(direction);
        return new Vector3(vec2.x, 0f, vec2.y);
    }

    public static int AttackVecToDirec(Vector3 vec)
    {
        return AttackVecToDirec(new Vector2(vec.x, vec.z));
    }

    public static int AttackVecToDirec(Vector2 vec)
    {
        // (0,1)이 0도에서 시계방향으로 증가, [-180~180]
        var angle = Mathf.Atan2(vec.x, vec.y) * Mathf.Rad2Deg;

        if (angle >= -202.5f && angle <= -157.5f)
            return 1;
        if (angle < -112.5f)
            return 2;
        else if (angle <= -67.5f)
            return 3;
        else if (angle < -22.5f)
            return 4;
        else if (angle <= 22.5f)
            return 5;
        else if (angle < 67.5f)
            return 6;
        else if (angle <= 112.5f)
            return 7;
        else if (angle < 157.5f)
            return 8;
        else if (angle <= 202.5f)
            return 1;
        
        return 0;
    }
}

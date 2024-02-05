using Google.Protobuf.Protocol;
using Server.Util;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml;

namespace Server.Game
{
    public class Monster : GameObject
	{
        long maxHP;
        long nowHP;
        long power;
        float speed = 2f;

        public Monster()
        {
            ObjectType = GameObjectType.Monster;

            maxHP = 1000;
            nowHP = maxHP; 
        }

        public void OnDamaged(Player attacker, int damage)
        {
            if (Room == null) return;

            nowHP -= damage;

            S_Ondamage packet = new S_Ondamage();
            packet.ObjectId = Id;
            packet.AttackerId = attacker.Id;
            packet.Damage = damage;
            packet.RemainHp = nowHP;
            packet.MaxHp = maxHP;
            Room.Broadcast(packet);
        }
    }
}



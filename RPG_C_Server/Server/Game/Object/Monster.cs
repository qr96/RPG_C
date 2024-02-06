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
        DateTime respawnTime;

        public Monster()
        {
            ObjectType = GameObjectType.Monster;

            maxHP = 100;
            power = 1;
        }

        public void Spawn()
        {
            nowHP = maxHP;
        }

        // 부활 가능 상태 체크
        public bool PossibleRespawn()
        {
            return nowHP <= 0 && DateTime.Now > respawnTime;
        }

        public void OnDamaged(Player attacker, long damage)
        {
            if (Room == null) return;

            nowHP -= damage;
            if (nowHP <= 0)
            {
                attacker.AddExp(10);
                respawnTime = DateTime.Now.AddSeconds(5f);
            }

            S_Ondamage packet = new S_Ondamage();
            packet.ObjectId = Id;
            packet.AttackerId = attacker.Id;
            packet.Damage = damage;
            packet.RemainHp = nowHP;
            packet.MaxHp = maxHP;
            Room.Broadcast(packet);

            attacker.OnDamaged(this, power);
        }
    }
}



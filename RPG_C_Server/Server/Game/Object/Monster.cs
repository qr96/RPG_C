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
                attacker.AddExp(1000);
                //attacker.AddMoney(100);
                respawnTime = DateTime.Now.AddSeconds(15f);
                SendSpawnItem(attacker, new List<ItemInfo>() { new ItemInfo() { ItemCode = 1, Count = 100 } });
            }

            SendOnDamage(attacker, damage);

            attacker.OnDamaged(this, power);
        }

        #region Packet

        void SendOnDamage(Player attacker, long damage)
        {
            S_Ondamage packet = new S_Ondamage();
            packet.ObjectId = Id;
            packet.AttackerId = attacker.Id;
            packet.Damage = damage;
            packet.RemainHp = nowHP;
            packet.MaxHp = maxHP;
            Room.Broadcast(packet);
        }

        void SendSpawnItem(Player player, List<ItemInfo> items)
        {
            S_SpawnItem packet = new S_SpawnItem();
            packet.TargetId = Id;
            foreach (var item in items)
                packet.ItemInfos.Add(item);

            player.Session.Send(packet);
        }

        #endregion
    }
}


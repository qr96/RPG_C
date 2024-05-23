using Google.Protobuf.Protocol;
using Server.Data;
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
        DateTime idleEndTime;
        DateTime moveEndTime;
        Vector3 nowPos;
        Vector3 desPos;
        Player target;

        List<Vector3> movePoints = new List<Vector3>();

        StateMachine<MonsterState> sm = new StateMachine<MonsterState>();

        public enum MonsterState
        {
            None,
            Idle,
            Move,
            Chase,
            Damaged,
            Die
        }

        public Monster()
        {
            ObjectType = GameObjectType.Monster;

            maxHP = 100;
            power = 5;

            sm.SetEvent(MonsterState.Idle,
                (prev) =>
                {
                    var idleTime = DataManager.Rand.NextDouble() * 4 + 4f;
                    idleEndTime = DateTime.Now.AddSeconds(idleTime);
                    SendState();
                },
                () =>
                {
                    if (DateTime.Now > idleEndTime)
                        sm.SetState(MonsterState.Move);
                });
            sm.SetEvent(MonsterState.Move,
                (prev) =>
                {
                    var posIndex = Room.Rand.Next(0, movePoints.Count);
                    var moveTime = 0f;

                    desPos = movePoints[posIndex];
                    moveTime = (desPos - nowPos).Length() / speed;

                    moveEndTime = DateTime.Now.AddSeconds(moveTime);
                    SetNowPos(desPos);
                    SendState();
                },
                () =>
                {
                    if (DateTime.Now > moveEndTime)
                        sm.SetState(MonsterState.Idle);
                });
            sm.SetEvent(MonsterState.Chase, null,
                () =>
                {
                    if (target == null)
                        sm.SetState(MonsterState.Idle);
                });
        }

        public void SetNowPos(Vector3 pos)
        {
            nowPos = pos;
            PosInfo.PosX = pos.X;
            PosInfo.PosY = pos.Y;
            PosInfo.PosZ = pos.Z;
        }

        public void SetMovePoints(Vector3 firstPoint, Vector3 secondPoint)
        {
            movePoints.Add(firstPoint);
            movePoints.Add(secondPoint);
        }

        public void Update()
        {
            sm.Update();
        }

        public void Spawn()
        {
            nowHP = maxHP;
            sm.SetState(MonsterState.Idle);
        }

        // 부활 가능 상태 체크
        public bool PossibleRespawn()
        {
            return nowHP <= 0 && DateTime.Now > respawnTime;
        }

        public void OnDamaged(Player attacker, int direction, long damage)
        {
            if (Room == null) return;
            if (attacker == null) return;

            target = attacker;
            nowHP -= damage;
            if (nowHP <= 0)
            {
                attacker.AddExp(100);
                //attacker.AddMoney(100);
                respawnTime = DateTime.Now.AddSeconds(15f);
                SendSpawnItem(attacker, new List<ItemInfo>() { new ItemInfo() { ItemCode = 1, Count = 100 } });
            }
            else
            {
                sm.SetState(MonsterState.Damaged);
            }
            SendOnDamage(attacker, direction, damage);

            attacker.OnDamaged(this, power);
        }

        #region Packet

        void SendState()
        {
            S_MonsterState packet = new S_MonsterState();
            packet.ObjectId = Id;
            packet.State = (int)sm.GetState();
            packet.NowPos = PosInfo;
            Room.Broadcast(packet);
        }

        void SendOnDamage(Player attacker, int direction, long damage)
        {
            S_Ondamage packet = new S_Ondamage();
            packet.ObjectId = Id;
            packet.AttackerId = attacker.Id;
            packet.Direction = direction;
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


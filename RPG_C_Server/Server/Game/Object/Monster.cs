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
        double maxChaseDis = 400f;

        List<Vector3> movePoints = new List<Vector3>();
        StateMachine<MonsterState> sm = new StateMachine<MonsterState>();

        ushort nowDesPoint;
        Vector3 nowPos; // 현재 위치
        Vector3 moveVector; // 진행 벡터
        Vector3 chaseStart; // 추적 시작 위치
        DateTime idleTime;
        DateTime arriveTime;
        DateTime knockBackTime;
        Player target;

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
        }

        public void Init()
        {
            movePoints.Add(new Vector3(6f, 0f, 5f));
            movePoints.Add(new Vector3(32f, 0f, 5f));
            nowPos = movePoints[0];

            sm.SetEvent(MonsterState.Idle,
            (prev) =>
            {
                // 쉬는 시간 리스트로 정해두는게 좋을듯
                idleTime = DateTime.Now.AddSeconds(4);
            },
            () =>
            {
                if (DateTime.Now > idleTime)
                    sm.SetState(MonsterState.Move);
            });
            sm.SetEvent(MonsterState.Move,
                (prev) =>
                {
                    if (prev == MonsterState.Chase)
                    {
                        moveVector = Vector3.Normalize(chaseStart - nowPos) * speed / 10;
                        arriveTime = DateTime.Now.AddSeconds((chaseStart - nowPos).Length() / speed);
                    }
                    else
                    {
                        nowDesPoint = (ushort)(nowDesPoint++ % movePoints.Count);
                        moveVector = Vector3.Normalize(movePoints[nowDesPoint] - nowPos) * speed / 10;
                        arriveTime = DateTime.Now.AddSeconds((movePoints[nowDesPoint] - nowPos).Length() / speed);
                    }
                },
                () =>
                {
                    if (DateTime.Now > arriveTime)
                        sm.SetState(MonsterState.Idle);
                    nowPos += moveVector;
                });
            sm.SetEvent(MonsterState.Chase,
                (prev) =>
                {
                    SendState();
                    chaseStart = nowPos;
                },
                () =>
                {
                    if ((nowPos - chaseStart).LengthSquared() > maxChaseDis)
                        sm.SetState(MonsterState.Move);
                    else if (target != null)
                    {
                        if ((RBUtil.PosInfoToVector(target.PosInfo) - nowPos).Length() > 0.5f)
                            moveVector = Vector3.Normalize(RBUtil.PosInfoToVector(target.PosInfo) - nowPos) * speed;
                        nowPos += moveVector;
                    }
                    else
                        sm.SetState(MonsterState.Move);
                });
            sm.SetEvent(MonsterState.Damaged, null,
                () =>
                {
                    if (DateTime.Now > knockBackTime)
                        sm.SetState(MonsterState.Chase);
                });

            sm.SetState(MonsterState.Idle);
        }

        public override void Update()
        {
            sm.Update();
            PosInfo.PosX = nowPos.X; 
            PosInfo.PosY = nowPos.Y;
            PosInfo.PosZ = nowPos.Z;

            //SendMove();
        }

        /*
        private void SendMove()
        {
            S_Move resMovePacket = new S_Move();
            resMovePacket.ObjectId = Info.ObjectId;
            resMovePacket.PosInfo = PosInfo;

            Room.Broadcast(resMovePacket);
        }
        */

        /** 클라에서 처리할 일
         * Idle : NowPos 로 Lerp로 이동
         * Move : NowPos로 바로 이동, MoveVec 방향으로 이동
         * Chase : 클라에서 처리. 서버는 되돌아가는 로직만
         * Damaged : 별도의 처리 X. 클라에서 처리
         */
        void SendState()
        {
            S_MonsterState packet = new S_MonsterState();
            packet.ObjectId = Info.ObjectId;
            packet.State = (int)sm.GetState();
            packet.NowPos = PosInfo;
            packet.MoveVec = RBUtil.VectorToPosInfo(moveVector);
            packet.TargetId = target != null ? target.Id : 0;

            Room.Broadcast(packet);
        }
    }
}

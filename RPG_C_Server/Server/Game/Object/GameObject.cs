using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server.Game
{
	public class GameObject
	{
		public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
		public int Id
		{
			get { return Info.ObjectId; }
			set { Info.ObjectId = value; }
		}

		public GameRoom Room { get; set; }

		public ObjectInfo Info { get; set; } = new ObjectInfo();
		public PositionInfo PosInfo = new PositionInfo();
		public StatInfo Stat { get; private set; } = new StatInfo();

		public float Speed
		{
			get { return Stat.Speed; }
			set { Stat.Speed = value; }
		}

		public int Hp
		{
			get { return Stat.Hp; }
			set { Stat.Hp = Math.Clamp(value, 0, Stat.MaxHp); }
		}

		public GameObject()
		{
			Info.PosInfo = PosInfo;
			Info.StatInfo = Stat;
		}

        public virtual void Update()
        {

        }

        public virtual void OnDamaged(GameObject attacker, long damage)
		{
			/*
			if (Room == null)
				return;

			Stat.Hp = Math.Max(Stat.Hp - damage, 0);

			S_ChangeHp changePacket = new S_ChangeHp();
			changePacket.ObjectId = Id;
			changePacket.Hp = Stat.Hp;
			Room.Broadcast(changePacket);

			if (Stat.Hp <= 0)
			{
				OnDead(attacker);
			}
			*/
		}

		public virtual void OnDead(GameObject attacker)
		{
			if (Room == null)
				return;

			GameRoom room = Room;
			room.LeaveGame(Id);

			Stat.Hp = Stat.MaxHp;
			PosInfo.PosX = 0;
			PosInfo.PosY = 0;

			room.EnterGame(this);
		}
	}
}

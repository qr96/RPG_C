using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.Game.Collider;
using Server.Game.Object;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server.Game
{
	public class GameRoom : JobSerializer
	{
		public int RoomId { get; set; }

		Dictionary<int, Player> _players = new Dictionary<int, Player>();
		Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
		List<BaseMonster> _baseMonsters = new List<BaseMonster>();

		public Random rand = new Random();

		public Map Map { get; private set; } = new Map();

		public void Init(int mapId)
		{
			//Map.LoadMap(mapId);

			Monster monster = ObjectManager.Instance.Add<Monster>();
			monster.Info.PosInfo = new PositionInfo() { PosX = 6f, PosY = 0f, PosZ = 5f };
			EnterGame(monster);
        }

		// 누군가 주기적으로 호출해줘야 한다
		public void Update()
		{
			foreach(var player in _players.Values)
			{
				player.Update();
			}

			foreach (var monster in _monsters.Values)
				monster.Update();

			Flush();
		}

		public void EnterGame(GameObject gameObject)
		{
			if (gameObject == null)
				return;

			GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

			if (type == GameObjectType.Player)
			{
				Player player = gameObject as Player;
				_players.Add(gameObject.Id, player);
				player.Room = this;

				// 본인한테 정보 전송
				{
					S_EnterGame enterPacket = new S_EnterGame();
					enterPacket.Player = player.Info;
					player.Session.Send(enterPacket);

					S_Spawn spawnPacket = new S_Spawn();
					foreach (Player p in _players.Values)
					{
						if (player != p)
							spawnPacket.Objects.Add(p.Info);
					}

					foreach (var m in _monsters.Values)
						spawnPacket.Objects.Add(m.Info);

                    player.Session.Send(spawnPacket);
				}
			}
			else if (type == GameObjectType.Monster)
			{
				Monster monster = gameObject as Monster;
				_monsters.Add(monster.Id, monster);
				monster.Room = this;
            }

			// 타인한테 정보 전송
			{
                S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.Objects.Add(gameObject.Info);
                foreach (Player p in _players.Values)
                {
                    if (p.Id != gameObject.Id)
                        p.Session.Send(spawnPacket);
                }
            }
        }

		public void LeaveGame(int objectId)
		{
			GameObjectType type = ObjectManager.GetObjectTypeById(objectId);

			if (type == GameObjectType.Player)
			{
				Player player = null;
				if (_players.Remove(objectId, out player) == false)
					return;

				player.Room = null;

				// 본인한테 정보 전송
				{
					S_LeaveGame leavePacket = new S_LeaveGame();
					player.Session.Send(leavePacket);
				}
			}
			else if (type == GameObjectType.Monster)
			{
				Monster monster = null;
				if (_monsters.Remove(objectId, out monster) == false)
					return;

				//Map.ApplyLeave(monster);
				monster.Room = null;
			}

			// 타인한테 정보 전송
			{
				S_Despawn despawnPacket = new S_Despawn();
				despawnPacket.ObjectIds.Add(objectId);
				foreach (Player p in _players.Values)
				{
					if (p.Id != objectId)
						p.Session.Send(despawnPacket);
				}
			}
		}

		public void HandleMove(Player player, C_Move movePacket)
		{
			if (player == null)
				return;

			player.PosInfo = movePacket.PosInfo;
			player.Info.PosInfo = movePacket.PosInfo;

			// 다른 플레이어한테도 알려준다
			S_Move resMovePacket = new S_Move();
			resMovePacket.ObjectId = player.Info.ObjectId;
			resMovePacket.PosInfo = movePacket.PosInfo;

			Broadcast(resMovePacket);
		}

		public void HandleSkill(Player player, C_Skill skillPacket)
		{
			if (player == null)
				return;

			if (player.CheckSkillCool(skillPacket.SkillId))
			{
				Monster monster;
				if (!_monsters.TryGetValue(skillPacket.TargetId, out monster))
					return;

				player.UseSkill(skillPacket.SkillId);
				if (skillPacket.SkillId == 1)
					monster.OnDamaged(player, 10);
            }
		}

		public Player FindPlayer(Func<Player, bool> condition)
		{
			foreach (Player player in _players.Values)
			{
				if (condition.Invoke(player))
					return player;
			}

			return null;
		}

		public void Broadcast(IMessage packet)
		{
			foreach (Player p in _players.Values)
			{
				p.Session.Send(packet);
			}
		}
	}
}

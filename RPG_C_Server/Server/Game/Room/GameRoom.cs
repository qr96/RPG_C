using Google.Protobuf;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Server.Game
{
	public class GameRoom : JobSerializer
	{
		public int RoomId { get; set; }

		Dictionary<int, Player> _players = new Dictionary<int, Player>();
		Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();

		DateTime respawnTime = DateTime.MinValue;
		float respawnDelay = 20f;

		public Random Rand = new Random();

		public Map Map { get; private set; } = new Map();
		List<Vector3> monsterPosList = new List<Vector3>()
		{
			new Vector3(6f, 0f, 5f), new Vector3(3f, 0f,1f), new Vector3(10f, 0f, -4f),
			new Vector3(-16f, 0f, 5f), new Vector3(-30f, 0f, -20f), new Vector3(-10f, 0f, -27f), new Vector3(3f, 0f, -20f), new Vector3(20f, 0f, -20f)
		};

		List<Vector3> monsterMovePoint2 = new List<Vector3>()
		{
			new Vector3(6f, 0f, 10f), new Vector3(8f, 0f,1f), new Vector3(4f, 0f, -4f),
			new Vector3(-12f, 0f, 5f), new Vector3(-26f, 0f, -20f), new Vector3(-10f, 0f, -22f), new Vector3(9f, 0f, -20f), new Vector3(15f, 0f, -20f)
		};

		public void Init(int mapId)
		{
			//Map.LoadMap(mapId);

			for (int i = 0; i < monsterPosList.Count; i++)
			{
                Monster monster = ObjectManager.Instance.Add<Monster>();
				monster.SetNowPos(monsterPosList[i]);
				monster.SetMovePoints(monsterPosList[i], monsterMovePoint2[i]);
                EnterGame(monster);
            }
        }

		// 누군가 주기적으로 호출해줘야 한다
		public void Update()
		{
			foreach(var player in _players.Values)
                player.Update();

            foreach (var monster in _monsters.Values)
				monster.Update();

			if (DateTime.Now > respawnTime)
			{
				respawnTime = DateTime.Now.AddSeconds(respawnDelay);

				S_Spawn spawnPacket = new S_Spawn();
				foreach (var monster in _monsters.Values)
				{
					if (monster.PossibleRespawn())
					{
                        monster.Spawn();
						spawnPacket.Objects.Add(monster.Info);
                    }
				}
                Broadcast(spawnPacket);
            }

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
					player.Spawn();
					player.SendStatInfo();
				}
			}
			else if (type == GameObjectType.Monster)
			{
				Monster monster = gameObject as Monster;
				_monsters.Add(monster.Id, monster);
				monster.Room = this;
                monster.Spawn();
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

		public void HandleAttack(Player player, C_Attack packet)
		{
			if (player == null)
				return;

			if (player.CheckAttackCool())
			{
                Monster monster;
                if (!_monsters.TryGetValue(packet.TargetId, out monster))
                    return;

				player.Attack(monster, packet.Direction);
				player.SendAttack(packet.Direction);
                player.SendStatInfo();
            }
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
				
				player.UseSkill(skillPacket.SkillId, monster);
				player.SendStatInfo();
            }
		}

		public void HandleUseItem(Player player, C_UseItem itemPacket)
		{
			if (player == null)
				return;

			player.UseItem(itemPacket.ItemCode, itemPacket.UseCount);
		}

		public void HandlePickupItem(Player player, C_PickupItem itemPacket)
		{
			if (player == null)
				return;
			// TODO : 아이템 실제로 떨어진거 먹었는지 검증 필요
			if (itemPacket.PickupItem.ItemCode == 1)
				player.AddMoney(itemPacket.PickupItem.Count);
		}

		public void HandleInventoryInfo(Player player)
		{
			if (player == null)
				return;

			player.SendInventoryInfo();
        }

		public void HandleChat(Player player, string chat)
		{
			if (player == null)
				return;

			S_Chat packet = new S_Chat();
			packet.Id = player.Id;
			packet.Chat = chat;

			Broadcast(packet);
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

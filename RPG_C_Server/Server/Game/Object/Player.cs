﻿using Google.Protobuf.Protocol;
using Server.Game.Collider;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;

namespace Server.Game
{
	public class Player : GameObject
	{
		public ClientSession Session { get; set; }

		// 스킬 사용불가한 시간 딕셔너리
		Dictionary<int, DateTime> skillCoolTimes = new Dictionary<int, DateTime>();

        // 능력치
        int level;
        long maxHp;
		long maxMp;
        long maxExp;
		long attack;

        long _nowHp;
        long _nowMp;
		long _nowExp;

		long _money;
		
		public Player()
		{
			ObjectType = GameObjectType.Player;

			level = 1;
			maxHp = 100;
			maxMp = 100;
			maxExp = 100;
			attack = 10;
		}

		public void Spawn()
		{
			_nowHp = maxHp;
			_nowMp = maxMp;
		}

        public override void OnDamaged(GameObject attacker, long damage)
		{
			_nowHp -= damage;
		}

		public override void OnDead(GameObject attacker)
		{
			base.OnDead(attacker);
		}

		public bool CheckSkillCool(int skillId)
		{
			if (!skillCoolTimes.ContainsKey(skillId)) return true;
			return DateTime.Now > skillCoolTimes[skillId];
		}

		public void UseSkill(int skillId, Monster target)
		{
			if (!skillCoolTimes.ContainsKey(skillId))
				skillCoolTimes.Add(skillId, DateTime.Now);
			else
				skillCoolTimes[skillId] = DateTime.Now;

            _nowMp -= 1;

			if (skillId == 1)
				target.OnDamaged(this, attack);
        }

		public void UseItem(int itemId, int count)
		{
			if (itemId == 1)
			{
				_nowHp = Math.Min(_nowHp + 10, maxHp);
				SendStatInfo();
			}
			else if (itemId == 2)
			{
                _nowMp = Math.Min(_nowMp + 10, maxMp);
                SendStatInfo();
            }
		}

		public void AddExp(long exp)
		{
			_nowExp += exp;
			while (_nowExp >= maxExp)
			{
                level++;
				_nowExp -= maxExp;
                maxExp = 100 + 20 * level;
                _nowHp = maxHp;
                _nowMp = maxMp;
                attack += 2;
            }
		}

		public void AddMoney(long money)
		{
			_money += money;
			Console.WriteLine(_money);
		}

        #region Packet

        public void SendStatInfo()
		{
			S_ChangeStatus packet = new S_ChangeStatus();
			packet.ObjectId = Id;
			packet.Level = level;
			packet.MaxHp = maxHp;
			packet.MaxMp = maxMp;
			packet.MaxExp = maxExp;
			packet.NowHp = _nowHp;
			packet.NowMp = _nowMp;
			packet.NowExp = _nowExp;

			Session.Send(packet);
		}

		public void SendInventoryInfo()
		{
			S_InventoryInfo packet = new S_InventoryInfo();
			packet.Money = _money;
			Session.Send(packet);
		}

        #endregion
    }
}

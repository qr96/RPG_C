using Google.Protobuf.Protocol;
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
				target.OnDamaged(this, level * 2);
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
			if (_nowExp >= maxExp)
			{
				level++;
				_nowExp = 0;
				maxExp += 100;
				_nowHp = maxHp;
				_nowMp = maxMp;
			}
		}

		public void AddMoney(long money)
		{
			_money += money;
		}

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
	}
}

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

        long nowHp;
        long nowMp;
		long nowExp;
		
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
			nowHp = maxHp;
			nowMp = maxMp;
		}

        public override void OnDamaged(GameObject attacker, long damage)
		{
			nowHp -= damage;
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

            nowMp -= 1;

			if (skillId == 1)
				target.OnDamaged(this, level * 2);
        }

		public void AddExp(long exp)
		{
			nowExp += exp;
			if (nowExp >= maxExp)
			{
				level++;
				nowExp = 0;
				maxExp += 100;
				nowHp = maxHp;
				nowMp = maxMp;
			}
		}

		public void SendStatInfo()
		{
			S_ChangeStatus packet = new S_ChangeStatus();
			packet.ObjectId = Id;
			packet.Level = level;
			packet.MaxHp = maxHp;
			packet.MaxMp = maxMp;
			packet.MaxExp = maxExp;
			packet.NowHp = nowHp;
			packet.NowMp = nowMp;
			packet.NowExp = nowExp;

			Session.Send(packet);
		}
	}
}

using Google.Protobuf.Protocol;
using Server.Game.Collider;
using System;
using System.Collections.Generic;

namespace Server.Game
{
	public class Player : GameObject
	{
		public ClientSession Session { get; set; }

		// 스킬 사용불가한 시간 딕셔너리
		Dictionary<int, DateTime> skillCoolTimes = new Dictionary<int, DateTime>();

		List<int> _skillLevels = new List<int>();

        // 능력치
        int _level;
        long _maxExp;
        long _maxHp;
		long _maxMp;
		long _attack;
		int _mental;
		float _moveSpeed;

		long _skillMaxHp;
		long _skillMaxMp;
		long _skillAttack;
		int _skillMental;
		float _skillMoveSpeed;

        long _nowHp;
        long _nowMp;
		long _nowExp;

		long _money;
		int _skillPoint;

		public Player()
		{
			ObjectType = GameObjectType.Player;

            _level = 1;
            _maxHp = 100;
			_maxMp = 100;
            _maxExp = 100;
            _attack = 10;
			_moveSpeed = 200;

            _skillLevels.Add(1);
            for (int i = 1; i < 5; i++)
				_skillLevels.Add(0);
		}

		public void Spawn()
		{
			_nowHp = _maxHp;
			_nowMp = _maxMp;
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

            if (skillId == 1)
			{
                if (_nowMp < 10)
				{
					target.OnDamaged(this, 0);
                }
				else
				{
					var damage = (_attack + _skillAttack) * ((_nowMp * 100 / _maxMp - 100) * (100 - _mental - _skillMental) / 100 + 100) / 100;
                    target.OnDamaged(this, damage);
                    _nowMp -= 10;
                }
            }
        }

		public void UseItem(int itemId, int count)
		{
			if (itemId == 1)
			{
				_nowHp = Math.Min(_nowHp + 50, _maxHp);
				SendStatInfo();
			}
			else if (itemId == 2)
			{
                _nowMp = Math.Min(_nowMp + 50, _maxMp);
                SendStatInfo();
            }
		}

		public void AddExp(long exp)
		{
			_nowExp += exp;
			while (_nowExp >= _maxExp)
			{
                _level++;
				_nowExp -= _maxExp;
                _maxExp = 100 + 20 * _level;
                _nowHp = _maxHp;
                _nowMp = _maxMp;
                _attack += 2;
				_skillPoint += 30;
            }
		}

		public void AddMoney(long money)
		{
			_money += money;
		}

		public void UpdateSkillStats()
		{
			_skillAttack = _skillLevels[0] * 2;
			_skillMoveSpeed = _skillLevels[1] * 2;
			//2
			_skillMaxMp = _skillLevels[3] * 10;
			_skillMental = _skillLevels[4] * 1;
		}

		public void LearnSkill(int skillId)
		{
			if (skillId >= _skillLevels.Count)
				return;

			if (_skillPoint <= 0)
				return;

            _skillPoint--;
            _skillLevels[skillId]++;

            UpdateSkillStats();
            SendSkillTabInfo();
        }

        #region Packet

        public void SendStatInfo()
		{
			S_ChangeStatus packet = new S_ChangeStatus();
			packet.ObjectId = Id;
			packet.Level = _level;
			packet.MaxHp = _maxHp;
			packet.MaxMp = _maxMp;
			packet.MaxExp = _maxExp;
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

		public void SendSkillTabInfo()
		{
			S_SkillTabInfo packet = new S_SkillTabInfo();
			packet.SkillPoint = _skillPoint;
			foreach (var skill in _skillLevels)
				packet.SkillLevels.Add(skill);
			Session.Send(packet);
		}

        #endregion
    }
}

using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

class PacketHandler
{
	public static void S_LoginGameHandler(PacketSession session, IMessage message)
	{
		S_LoginGame packet = message as S_LoginGame;

	}

	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;
		Managers.Object.Add(enterGamePacket.Player, myPlayer: true);
	}

	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		S_LeaveGame leaveGameHandler = packet as S_LeaveGame;
		Managers.Object.Clear();
	}

	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;
		foreach (ObjectInfo obj in spawnPacket.Objects)
            Managers.Object.Add(obj, myPlayer: false);
    }

	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;
		foreach (int id in despawnPacket.ObjectIds)
		{
			Managers.Object.Remove(id);
		}
	}

	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		S_Move movePacket = packet as S_Move;

        GameObject go = Managers.Object.FindById(movePacket.ObjectId);
		if (go == null)
			return;

		if (Managers.Object.MyPlayer.Id == movePacket.ObjectId)
			return;

		if (ObjectManager.GetObjectTypeById(movePacket.ObjectId) == GameObjectType.Monster)
		{
			go.transform.position = RBUtil.InsertY(RBUtil.PosToVector3(movePacket.PosInfo), go.transform.position.y);
        }
		else
		{
			OtherPlayer op = go.GetComponent<OtherPlayer>();
			op.SetDesPos(new Vector3(movePacket.PosInfo.PosX, movePacket.PosInfo.PosY, movePacket.PosInfo.PosZ));
        }
    }

	public static void S_AttackHandler(PacketSession session, IMessage message)
	{
		S_Attack packet = message as S_Attack;

        var go = Managers.Object.FindById(packet.ObjectId);
        if (go == null)
            return;

        if (packet.ObjectId == Managers.Object.MyPlayer.Id)
			return;

		var other = go.GetComponent<OtherPlayer>();
		other.AttackMotion(packet.Direction);
	}

	public static void S_SkillHandler(PacketSession session, IMessage packet)
	{
		S_Skill skillPacket = packet as S_Skill;

		GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
		if (go == null)
			return;

		if (Managers.Object.MyPlayer.Id == skillPacket.ObjectId)
            return;
	}

	public static void S_OnDamageHandler(PacketSession session, IMessage packet)
	{
		S_Ondamage damagePacket = packet as S_Ondamage;

        GameObject go = Managers.Object.FindById(damagePacket.ObjectId);
        if (go == null)
            return;

		Monster monster = go.GetComponent<Monster>();
		if (monster == null)
			return;

		GameObject attacker = Managers.Object.FindById(damagePacket.AttackerId);
		if (damagePacket.AttackerId != Managers.Object.MyPlayer.Id)
		{
			/*
			var player = attacker.GetComponent<OtherPlayer>();
			if (player != null)
				player.AttackMotion();
			*/
			Debug.Log("Attacked");
            monster.OnDamagedClient(attacker, damagePacket.Direction);
        }

		monster.OnDamagedServer(attacker, damagePacket.Damage, damagePacket.RemainHp, damagePacket.MaxHp);
    }

	public static void S_S_ChangeStatusHandler(PacketSession session, IMessage packet)
	{
        S_ChangeStatus statusPacket = packet as S_ChangeStatus;
		UIManager.Instance.SetPlayerLevel(statusPacket.Level);
		UIManager.Instance.SetPlayerHPBar(statusPacket.NowHp, statusPacket.MaxHp);
		UIManager.Instance.SetPlayerMPBar(statusPacket.NowMp, statusPacket.MaxMp);
		UIManager.Instance.SetPlayerEXPBar(statusPacket.NowExp, statusPacket.MaxExp);
    }


    public static void S_MonsterStateHandler(PacketSession session, IMessage packet)
	{
        S_MonsterState statePacket = packet as S_MonsterState;
		//Debug.Log($"S_MonsterState {statePacket.State}");

        GameObject go = Managers.Object.FindById(statePacket.ObjectId);
        if (go == null)
            return;

        if (ObjectManager.GetObjectTypeById(statePacket.ObjectId) == GameObjectType.Monster)
		{
            var monster = go.GetComponent<Monster>();
			monster.RecvMonsterState(statePacket);
        }
    }

	public static void S_SpawnItemHandler(PacketSession session, IMessage packet)
	{
		S_SpawnItem itemPacket = packet as S_SpawnItem;

		foreach (var item in itemPacket.ItemInfos)
		{
            var money = Managers.Resource.Instantiate("Item/GoldCoin");
            var droppedItem = money.GetComponent<DroppedItem>();
			var itemTmp = item;

            droppedItem.OnColliderEvent = () =>
            {
				C_PickupItem c_PickupItem = new C_PickupItem();
				c_PickupItem.PickupItem = itemTmp;
				Managers.Network.Send(c_PickupItem);
				Managers.Resource.Destroy(money);
            };

            GameObject go = Managers.Object.FindById(itemPacket.TargetId);
			if (go == null)
				money.transform.position = Managers.Object.MyPlayer.transform.position;
			else
				money.transform.position = go.transform.position;
        }
	}

	public static void S_InventoryInfoHandler(PacketSession session, IMessage packet)
	{
		S_InventoryInfo invenPacket = packet as S_InventoryInfo;

		UIManager.Instance.SetUserInfoPopup(invenPacket.Money);
    }

	public static void S_SKillTabInfoHandler(PacketSession session, IMessage packet)
	{
		S_SkillTabInfo skillPacket = packet as S_SkillTabInfo;

		int skillPoint = skillPacket.SkillPoint;
		List<int> skillLevels = new List<int>();
		foreach (var skillLevel in skillPacket.SkillLevels)
			skillLevels.Add(skillLevel);

		UIManager.Instance.SetSkillTabPopup(skillPoint, skillLevels);

		// 임시처리
		Managers.Object.MyPlayer.SetSpeed(200f + 10f * Math.Min(30f, skillLevels[1]));
        var attackCoolDown = 0.01f * Math.Min(30f, skillLevels[2]);
        var attackCool = DateTime.Now.AddSeconds(0.5f - attackCoolDown);
		Managers.Object.MyPlayer.SetAttackDelay(attackCoolDown);
    }

	public static void S_SkillLevelUpHandler(PacketSession session, IMessage packet)
	{
		S_SKillLevelUp s_SKillLevelUp = packet as S_SKillLevelUp;

	}

	public static void S_ChatHandler(PacketSession session, IMessage message)
	{
		S_Chat packet = message as S_Chat;

		var target = Managers.Object.FindById(packet.Id);
		if (target == null)
			return;

		if (packet.Id == Managers.Object.MyPlayer.Id)
			target.GetComponent<MyPlayer>().ShowChatBalloon(packet.Chat);
		else
            target.GetComponent<OtherPlayer>().ShowChatBalloon(packet.Chat);

        UIManager.Instance.RecvChat(packet.Id.ToString(), packet.Chat);
    }
}

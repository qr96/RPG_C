using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{
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

	public static void S_SkillHandler(PacketSession session, IMessage packet)
	{
		S_Skill skillPacket = packet as S_Skill;

		GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
		if (go == null)
			return;

		if (Managers.Object.MyPlayer.Id == skillPacket.ObjectId)
            return;

        //PlayerController cc = go.GetComponent<PlayerController>();
		//if (cc != null)
		//{
		//	cc.UseSkill();
		//}
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

        GameObject go = Managers.Object.FindById(statePacket.ObjectId);
        if (go == null)
            return;

		if (ObjectManager.GetObjectTypeById(statePacket.ObjectId) == GameObjectType.Monster)
		{
            var monster = go.GetComponent<Monster>();
			monster.RecvMonsterState(statePacket);
        }
    }
}



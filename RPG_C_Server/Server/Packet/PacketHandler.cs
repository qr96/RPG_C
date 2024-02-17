using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.Data;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    public static void C_LoginGameHandler(PacketSession session, IMessage message)
    {
        C_LoginGame packet = message as C_LoginGame;
        ClientSession clientSession = session as ClientSession;

        //var resultCode = DataManager.Instance.TryGetPlayer(packet.Uno, out clientSession.MyPlayer);
        var resultCode = 0;

        S_LoginGame sendPacket = new S_LoginGame();
        sendPacket.ResultCode = resultCode;
        clientSession.Send(sendPacket);
    }

    public static void C_EnterGameHandler(PacketSession session, IMessage message)
    {
        C_EnterGame packet = message as C_EnterGame;
        ClientSession clientSession = session as ClientSession;


    }

	public static void C_MoveHandler(PacketSession session, IMessage packet)
	{
		C_Move movePacket = packet as C_Move;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleMove, player, movePacket);
	}

	public static void C_SkillHandler(PacketSession session, IMessage packet)
	{
		C_Skill skillPacket = packet as C_Skill;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleSkill, player, skillPacket);
	}

	public static void C_UseItemHandler(PacketSession session, IMessage packet)
	{
		C_UseItem itemPacket = packet as C_UseItem;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleUseItem, player, itemPacket);
    }

	public static void C_PickupItemHandler(PacketSession session, IMessage packet)
	{
        C_PickupItem itemPacket = packet as C_PickupItem;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

		room.Push(room.HandlePickupItem, player, itemPacket);
    }

	public static void C_InventoryInfoHandler(PacketSession session, IMessage packet)
	{
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

		room.Push(room.HandleInventoryInfo, player);
    }

	public static void C_SkillTabInfoHandler(PacketSession session, IMessage packet)
	{
		ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

		room.Push(player.SendSkillTabInfo);
    }

	public static void C_SkillLevelUpHandler(PacketSession session, IMessage packet)
	{
        C_SkillLevelUp c_skillLevelup = packet as C_SkillLevelUp;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(player.LearnSkill, c_skillLevelup.SkillCode);
    }

    public static void C_ChatHandler(PacketSession packetSession, IMessage message)
    {
        C_Chat packet = message as C_Chat;
        ClientSession session = packetSession as ClientSession;

        Player player = session.MyPlayer;
        if (player == null) 
            return;

        GameRoom room = player.Room;
        if (room == null)
            return;

        room.Push(room.HandleChat, player, packet.Chat);
    }
}

using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
		
	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

	public void Register()
	{
        _onRecv.Add((ushort)MsgId.CLoginGame, MakePacket<C_LoginGame>);
        _handler.Add((ushort)MsgId.CLoginGame, PacketHandler.C_LoginGameHandler);
        _onRecv.Add((ushort)MsgId.CEnterGame, MakePacket<C_EnterGame>);
        _handler.Add((ushort)MsgId.CEnterGame, PacketHandler.C_EnterGameHandler);
        _onRecv.Add((ushort)MsgId.CMove, MakePacket<C_Move>);
		_handler.Add((ushort)MsgId.CMove, PacketHandler.C_MoveHandler);
        _onRecv.Add((ushort)MsgId.CAttack, MakePacket<C_Attack>);
        _handler.Add((ushort)MsgId.CAttack, PacketHandler.C_AttackHandler);
        _onRecv.Add((ushort)MsgId.CSkill, MakePacket<C_Skill>);
		_handler.Add((ushort)MsgId.CSkill, PacketHandler.C_SkillHandler);
        _onRecv.Add((ushort)MsgId.CUseItem, MakePacket<C_UseItem>);
        _handler.Add((ushort)MsgId.CUseItem, PacketHandler.C_UseItemHandler);
        _onRecv.Add((ushort)MsgId.CPickupItem, MakePacket<C_PickupItem>);
        _handler.Add((ushort)MsgId.CPickupItem, PacketHandler.C_PickupItemHandler);
        _onRecv.Add((ushort)MsgId.CInventoryInfo, MakePacket<C_InventoryInfo>);
        _handler.Add((ushort)MsgId.CInventoryInfo, PacketHandler.C_InventoryInfoHandler);
        _onRecv.Add((ushort)MsgId.CSkillTabInfo, MakePacket<C_SkillTabInfo>);
		_handler.Add((ushort)MsgId.CSkillTabInfo, PacketHandler.C_SkillTabInfoHandler);
        _onRecv.Add((ushort)MsgId.CSkillLevelUp, MakePacket<C_SkillLevelUp>);
        _handler.Add((ushort)MsgId.CSkillLevelUp, PacketHandler.C_SkillLevelUpHandler);
        _onRecv.Add((ushort)MsgId.CChat, MakePacket<C_Chat>);
		_handler.Add((ushort)MsgId.CChat, PacketHandler.C_ChatHandler);
    }

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		try
		{
            T pkt = new T();
            pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

            if (CustomHandler != null)
            {
                CustomHandler.Invoke(session, pkt, id);
            }
            else
            {
                Action<PacketSession, IMessage> action = null;
                if (_handler.TryGetValue(id, out action))
                    action.Invoke(session, pkt);
            }
        }
		catch(Exception ex)
		{
			Console.WriteLine($"[{DateTime.Now}]{ex}");
		}
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}
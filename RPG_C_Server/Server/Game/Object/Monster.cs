using Google.Protobuf.Protocol;


namespace Server.Game
{
	public class Monster : GameObject
	{
		public Monster()
		{
			ObjectType = GameObjectType.Monster;

			// TEMP
			Stat.Level = 1;
			Stat.Hp = 100;
			Stat.MaxHp = 100;
			Stat.Speed = 5.0f;
		}
	}
}

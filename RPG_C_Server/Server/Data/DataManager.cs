using Google.Protobuf.Protocol;
using Server.Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server.Data
{
	public interface ILoader<Key, Value>
	{
		Dictionary<Key, Value> MakeDict();
	}

	public class DataManager
	{
		public static DataManager Instance;

		public static Dictionary<int, StatInfo> StatDict { get; private set; } = new Dictionary<int, StatInfo>();
		public static Dictionary<int, Data.Skill> SkillDict { get; private set; } = new Dictionary<int, Data.Skill>();

		static Dictionary<string, Player> playerData = new Dictionary<string, Player>(); // DB 연동 필요

		object _playerLock = new object();

		DataManager()
		{
			Instance = this;
		}

		public static void LoadData()
		{
			//StatDict = LoadJson<Data.StatData, int, StatInfo>("StatData").MakeDict();
			//SkillDict = LoadJson<Data.SkillData, int, Data.Skill>("SkillData").MakeDict();
		}

		static Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
		{
			string text = File.ReadAllText($"{ConfigManager.Config.dataPath}/{path}.json");
			return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(text);
		}

		public int TryGetPlayer(string uno)
		{
			// 0: 성공, 1: 정보 없음, 2: 실패, 3: 캐릭터 생성 실패
			int result = 2;

            lock (_playerLock)
			{
				if (!playerData.ContainsKey(uno))
				{
					if (playerData.TryAdd(uno, new Player()))
						result = 0;
					else
						result = 3;
				}
            }

            return result;
		}
	}
}

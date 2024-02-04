using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
	public MyPlayer MyPlayer { get; set; }
	Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
	
	public static GameObjectType GetObjectTypeById(int id)
	{
		int type = (id >> 24) & 0x7F;
		return (GameObjectType)type;
	}

	public void Add(ObjectInfo info, bool myPlayer = false)
	{
		GameObjectType objectType = GetObjectTypeById(info.ObjectId);
		if (objectType == GameObjectType.Player)
		{
            GameObject go = Managers.Resource.Instantiate("Creature/Player");
            go.name = info.Name;
            _objects.Add(info.ObjectId, go);
			go.transform.position = RBUtil.PosToVector3(info.PosInfo);

            //PlayerController rpc = go.GetComponent<PlayerController>();
            //rpc.id = info.ObjectId;
        }
		else if (objectType == GameObjectType.Monster)
		{
			GameObject go = Managers.Resource.Instantiate("Creature/Monster");
			go.name = info.Name;
			_objects.Add(info.ObjectId, go);

			//MonsterController mc = go.GetComponent<MonsterController>();
			//mc.id = info.ObjectId;
			//mc.transform.position = new Vector2(info.PosInfo.PosX, info.PosInfo.PosY);
			//mc.SetDesPos(mc.transform.position);
		}
	}

	public void Remove(int id)
	{
		GameObject go = FindById(id);
		if (go == null)
			return;

		_objects.Remove(id);
		Managers.Resource.Destroy(go);
	}

	public GameObject FindById(int id)
	{
		GameObject go = null;
		_objects.TryGetValue(id, out go);
		return go;
	}

	public GameObject Find(Func<GameObject, bool> condition)
	{
		foreach (GameObject obj in _objects.Values)
		{
			if (condition.Invoke(obj))
				return obj;
		}

		return null;
	}

	public void Clear()
	{
		foreach (GameObject obj in _objects.Values)
			Managers.Resource.Destroy(obj);
		_objects.Clear();
		MyPlayer = null;
	}
}

﻿using Google.Protobuf.Protocol;
using Server.Game.Collider;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server.Game
{
	public class Player : GameObject
	{
		public ClientSession Session { get; set; }

		public Player()
		{
			ObjectType = GameObjectType.Player;
		}

        public override void Update()
        {

        }

        public override void OnDamaged(GameObject attacker, int damage)
		{
			base.OnDamaged(attacker, damage);
		}

		public override void OnDead(GameObject attacker)
		{
			base.OnDead(attacker);
		}
	}
}

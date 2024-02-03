using Google.Protobuf.Protocol;
using Server.Game.Collider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Object
{
    public class BaseMonster : GameObject
    {
        RectCollider bodyCollider = new RectCollider();
        RectCollider attackCollider = new RectCollider();

        Vector2 position = Vector2.Zero;
        long hp = 100;
        CreatureState state;
        long skillDelay;
        long lastSkillTime;

        public BaseMonster()
        {
            ObjectType = GameObjectType.Monster;
            state = CreatureState.Idle;
            skillDelay = 2000;
        }

        public void SetBodyCollider(Vector2 offset, float width, float height)
        {
            bodyCollider.SetCollider(offset, width, height);
        }

        public void SetAttackCollider(Vector2 offset, float width, float height)
        {
            attackCollider.SetCollider(offset, width, height);
        }

        public void SetPosition(Vector2 position)
        {
            bodyCollider.SetPosition(position);
            attackCollider.SetPosition(position);
            this.position = position;
            PosInfo.PosX = position.X;
            PosInfo.PosY = position.Y;
        }

        public override void Update()
        {

        }
    }
}

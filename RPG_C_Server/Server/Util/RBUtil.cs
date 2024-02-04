using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Util
{
    public class RBUtil
    {
        public static Vector3 PosInfoToVector(PositionInfo pos)
        {
            return new Vector3(pos.PosX, pos.PosY, pos.PosZ);
        }

        public static PositionInfo VectorToPosInfo(Vector3 pos)
        {
            return new PositionInfo() { PosX = pos.X, PosY = pos.Y, PosZ = pos.Z };
        }
    }
}

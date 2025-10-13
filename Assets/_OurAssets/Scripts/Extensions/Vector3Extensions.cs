using UnityEngine;

namespace CursedOnion.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3Int CastToVectorInt(this Vector3 vector)
        {
            return new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z);
        }

        public static void Truncate(ref this Vector3 vector)
        {
            vector = new Vector3((int)vector.x, (int)vector.y, (int)vector.z);
        }
        public static void Round(ref this Vector3 vector)
        {
            vector.x = Mathf.Round(vector.x);
            vector.y = Mathf.Round(vector.y);
            vector.z = Mathf.Round(vector.z);
        }

        public static void Floor(ref this Vector3 vector)
        {
            vector.x = Mathf.Floor(vector.x);
            vector.y = Mathf.Floor(vector.y);
            vector.z = Mathf.Floor(vector.z);
        }

        public static void Ceil(ref this Vector3 vector)
        {
            vector.x = Mathf.Ceil(vector.x);
            vector.y = Mathf.Ceil(vector.y);
            vector.z = Mathf.Ceil(vector.z);
        }

        public static Vector3 SwizzleXZY(this Vector3 vector)
        {
            return new Vector3(vector.x, vector.z, vector.y);
        }

        public static Vector3 Center(ref this Vector3 vector)
        {
            vector.Floor();
            return vector + new Vector3(0.5f, 0.5f, 0.5f);
        }
        public static Vector3 CenterOnTile(ref this Vector3 vector)
        {
            vector.Floor();
            return vector + new Vector3(0.5f, 0, 0.5f);
        }
    }
}

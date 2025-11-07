using System;
using System.Collections.Generic;
using CursedOnion.Extensions;
using UnityEngine;

namespace CursedOnion.Helpers
{
    [Flags, System.Serializable]
    public enum DirectionFlag
    {
        None        = 0,
        
        Right       = 1 << 0,
        Left        = 1 << 1,
        Forward     = 1 << 4,
        Back        = 1 << 5,
        Up          = 1 << 2,
        Down        = 1 << 3,
        
        ForwardUp   = 1 << 6, 
        ForwardDown = 1 << 7, 
        BackUp      = 1 << 8, 
        BackDown    = 1 << 9, 
        RightUp     = 1 << 10,
        RightDown   = 1 << 11,
        LeftUp      = 1 << 12,
        LeftDown    = 1 << 13,
    }
    public static class DirectionHelper
    {
        private static readonly Dictionary<DirectionFlag, Vector3> Directions = new()
        {
            { DirectionFlag.Right,       new( 1, 0,  0) },
            { DirectionFlag.Left,        new(-1, 0,  0) },
            { DirectionFlag.Up,          new( 0, 1,  0) },
            { DirectionFlag.Down,        new( 0,-1,  0) },
            { DirectionFlag.Forward,     new( 0, 0,  1) },
            { DirectionFlag.Back,        new( 0, 0, -1) },
        
            { DirectionFlag.ForwardUp,   new( 0, 1,  1) },
            { DirectionFlag.ForwardDown, new( 0,-1,  1) },
            { DirectionFlag.BackUp,      new( 0, 1, -1) },
            { DirectionFlag.BackDown,    new( 0,-1, -1) },
            { DirectionFlag.RightUp,     new( 1, 1,  0) },
            { DirectionFlag.RightDown,   new( 1,-1,  0) },
            { DirectionFlag.LeftUp,      new(-1, 1,  0) },
            { DirectionFlag.LeftDown,    new(-1,-1,  0) },
        };
        
        public static Vector3 GetDirectionVector(DirectionFlag flag) => Directions.TryGetValue(flag, out var vector) ? vector : Vector3.zero;
        public static DirectionFlag GetDirectionFlag(Vector3 dir)
        {
            foreach (var pair in Directions)
            {
                if (pair.Value == dir) return pair.Key;
            }
            
            return DirectionFlag.None;
        }
        public static List<Vector3> GetDirectionVectors(DirectionFlag flags)
        {
            var vectors = new List<Vector3>();
            
            foreach (var pair in Directions)
            {
                if ((flags & pair.Key) == pair.Key)
                {
                    vectors.Add(pair.Value);
                }
            }
            
            return vectors;
        }
        public static DirectionFlag GetDirectionFlags(List<Vector3> vectors)
        {
            DirectionFlag result = DirectionFlag.None;
            
            foreach (var vector in vectors)
            {
                foreach (var pair in Directions)
                {
                    if (pair.Value == vector)
                    {
                        result |= pair.Key;
                        break;
                    }
                }
            }
            
            return result;
        }
        public static void RotateFlagsAroundYAxis(ref DirectionFlag flags, float degrees)
        {
            DirectionFlag newFlags = DirectionFlag.None;
            Debug.Log(degrees);
            foreach (var pair in Directions)
            {
                if ((flags & pair.Key) == pair.Key)
                {
                    Vector3 vector = pair.Value;
                    Vector3 rotatedVector = Quaternion.AngleAxis(degrees, Vector3.up) * vector;
                    
                    
                    
                    rotatedVector.Round();
                    
                    DirectionFlag rotatedFlag = GetDirectionFlag(rotatedVector);
                    if (rotatedFlag != DirectionFlag.None)
                    {
                        newFlags |= rotatedFlag;
                    }
                }
            }
            
            flags = newFlags;
        }

    }
}

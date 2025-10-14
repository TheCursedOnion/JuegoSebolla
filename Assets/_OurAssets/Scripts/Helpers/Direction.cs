using System;
using System.Collections.Generic;
using UnityEngine;

namespace CursedOnion.Helpers
{
    [Flags]
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
        private static readonly Dictionary<DirectionFlag, Vector3Int> Directions = new()
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
        
        public static Vector3Int GetDirectionVector(DirectionFlag flag) => Directions.TryGetValue(flag, out var vector) ? vector : Vector3Int.zero;

        public static DirectionFlag GetDirectionFlag(Vector3Int dir)
        {
            foreach (var pair in Directions)
            {
                if (pair.Value == dir) return pair.Key;
            }
            return DirectionFlag.None;
        }
    }
}

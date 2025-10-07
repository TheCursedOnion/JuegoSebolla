using System;
using System.Collections.Generic;
using UnityEngine;

namespace CursedOnion.Helpers
{
    [Flags]
    public enum DirectionFlags
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
        private static readonly Dictionary<DirectionFlags, Vector3Int> Directions = new()
        {
            { DirectionFlags.Right,       new( 1, 0,  0) },
            { DirectionFlags.Left,        new(-1, 0,  0) },
            { DirectionFlags.Up,          new( 0, 1,  0) },
            { DirectionFlags.Down,        new( 0,-1,  0) },
            { DirectionFlags.Forward,     new( 0, 0,  1) },
            { DirectionFlags.Back,        new( 0, 0, -1) },
        
            { DirectionFlags.ForwardUp,   new( 0, 1,  1) },
            { DirectionFlags.ForwardDown, new( 0,-1,  1) },
            { DirectionFlags.BackUp,      new( 0, 1, -1) },
            { DirectionFlags.BackDown,    new( 0,-1, -1) },
            { DirectionFlags.RightUp,     new( 1, 1,  0) },
            { DirectionFlags.RightDown,   new( 1,-1,  0) },
            { DirectionFlags.LeftUp,      new(-1, 1,  0) },
            { DirectionFlags.LeftDown,    new(-1,-1,  0) },
        };
        
        public static Vector3Int GetDirectionVector(DirectionFlags flag) => Directions.TryGetValue(flag, out var vector) ? vector : Vector3Int.zero;

        public static DirectionFlags GetDirectionFlag(Vector3Int dir)
        {
            foreach (var pair in Directions)
            {
                if (pair.Value == dir) return pair.Key;
            }
            return DirectionFlags.None;
        }
    }
}

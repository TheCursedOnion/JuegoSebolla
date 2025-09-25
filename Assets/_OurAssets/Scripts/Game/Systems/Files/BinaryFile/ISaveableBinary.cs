using System.IO;
using UnityEngine;

namespace CursedOnion.Game.Systems.Files
{
    public interface ISaveableBinary
    {
        void Save(BinaryWriter writer);
        void Load(BinaryReader reader);
    }
}

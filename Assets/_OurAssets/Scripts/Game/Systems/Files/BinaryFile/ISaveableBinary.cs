using System.IO;
using UnityEngine;

namespace CursedOnion.Game.Systems.Files
{
    public interface ISaveableBinary
    {
        void BeginSave();
        void SaveProcess(BinaryWriter writer);
        void BeginLoad();
        void LoadProcess(BinaryReader reader);
    }
}

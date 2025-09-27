using System.IO;
using CursedOnion.Game.Systems.Files;
using UnityEngine;
using UnityEngine.Serialization;

namespace CursedOnion.Game.Systems.Grid
{
    [System.Serializable]
    public class Grid3dSaveableBinary : ISaveableBinary
    {
        public uint width = 0;
        public uint height = 0;
        public uint length = 0;
        
        public int blockDataLength = 0;
        public Tile3dData[] blocks;
        
        public void BeginSave()
        {
            BinaryFile file = BinaryFile.DefaultFile(this, "grid3d");
            file.SaveBinary();
        }
        public void SaveProcess(BinaryWriter writer)
        {
            using (writer)
            {
                writer.Write(width);
                writer.Write(height);
                writer.Write(length);

                writer.Write(blockDataLength);

                if (blocks != null)
                {
                    foreach (var blockData in blocks)
                    {
                        writer.Write(blockData.blockId);
                        writer.Write(blockData.metadata);
                    }
                }
            }
        }

        public void BeginLoad()
        {
            BinaryFile file = BinaryFile.DefaultFile(this, "grid3d");
            file.LoadBinary();
        }

        public void LoadProcess(BinaryReader reader)
        {
            using (reader)
            {
                width = reader.ReadUInt32();
                height = reader.ReadUInt32();
                length = reader.ReadUInt32();

                blockDataLength = reader.ReadInt32();
                blocks = new Tile3dData[blockDataLength];

                for (int i = 0; i < blockDataLength; i++)
                {
                    blocks[i].blockId = reader.ReadUInt16();
                    blocks[i].metadata = reader.ReadByte();
                }
            }
        }
    }
    
}

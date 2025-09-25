using System.IO;
using CursedOnion.Game.Systems.Files;
using UnityEngine;

namespace CursedOnion.Game.Grid3D
{
    [System.Serializable]
    public class Grid3DSaveableBinary : ISaveableBinary
    {
        public uint width = 0;
        public uint height = 0;
        public uint depth = 0;
        
        public int blockDataLength = 0;
        public BlockData[] blocks;
        
        public void Save(BinaryWriter writer)
        {
            using (writer)
            {
                writer.Write(width);
                writer.Write(height);
                writer.Write(depth);

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

        public void Load(BinaryReader reader)
        {
            using (reader)
            {
                width = reader.ReadUInt32();
                height = reader.ReadUInt32();
                depth = reader.ReadUInt32();

                blockDataLength = reader.ReadInt32();
                blocks = new BlockData[blockDataLength];

                for (int i = 0; i < blockDataLength; i++)
                {
                    blocks[i].blockId = reader.ReadUInt16();
                    blocks[i].metadata = reader.ReadByte();
                }
            }
        }
    }
    
}

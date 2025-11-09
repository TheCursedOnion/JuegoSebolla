using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CursedOnion.Tools
{
    public static class CSVReader
    {
        public static Dictionary<string, string[]> LoadCsvResourceToDictionary(string csvPath, bool hasHeaderLine, char delimiter = ',')
        {
            if (!TryGetCsvResourceLines(csvPath, out string[] lines)) return null;
            return ProcessLines(lines, hasHeaderLine, delimiter);
        }
        public static Dictionary<string, string[]> LoadCsvToDictionary(string csvPath, bool hasHeaderLine, char delimiter = ',')
        {
            if(!TryGetCsvLines(csvPath, out string[] lines)) return null;
            return ProcessLines(lines, hasHeaderLine, delimiter);
        }
        static bool TryGetCsvLines(string path, out string[] lines)
        {
            lines = null;
            
            if(File.Exists(path))
                lines = File.ReadAllLines(path);
            
            return lines != null;
        }
        static bool TryGetCsvResourceLines(string path, out string[] lines)
        {
            lines = null;
            
            TextAsset csvAsset = Resources.Load<TextAsset>(path);
            if (csvAsset != null)
                lines = csvAsset.text.Split('\n', '\r');
            
            return lines != null;
        }
        static Dictionary<string, string[]> ProcessLines(string[] lines, bool hasHeaderLine, char delimiter = ',')
        {
            var dictionary = new Dictionary<string, string[]>();
            
            int startIndex = hasHeaderLine ? 1 : 0;
            for (int i = startIndex; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                var columns = line.Split(delimiter);

                if (columns.Length == 0) continue;
                
                string key = columns[0].Trim();
                if (string.IsNullOrEmpty(key)) continue;
                
                string[] values = new string[columns.Length - 1];
                Array.Copy(columns, 1, values, 0, values.Length);
                
                dictionary[key] = values;
            }

            return dictionary;
        }
        
        
    }
}

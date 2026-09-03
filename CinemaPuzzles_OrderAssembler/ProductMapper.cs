using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaPuzzles_OrderAssembler
{
    internal class ProductMapper
    {
        public Dictionary<string, string[]> Map;
        public ProductMapper()
        {
            string[] FilmDirectories = Directory.GetDirectories(Configurator.Puzzles);
            Map = GenerateProductMapping(FilmDirectories);
        }
        private Dictionary<string, string[]> GenerateProductMapping(string[] filmDirs)
        {
            Dictionary<string, string> skuAndSubdir = new Dictionary<string, string>();
            Dictionary<string, string[]> outputMap = new Dictionary<string, string[]>();
            for (int i = 0; i < filmDirs.Length; i++) 
            {
                var chunk = GetSkuDirChunk(filmDirs[i]);
                foreach(var kvp in chunk) skuAndSubdir.Add(kvp.Key, kvp.Value);
            }
            foreach(KeyValuePair<string, string> kvp in skuAndSubdir)
            {
                KeyValuePair<string, string[]> KeyValue = GetFilePaths(kvp);
                if (KeyValue.Value.Length == 3) outputMap.Add(KeyValue.Key, KeyValue.Value);
            }
            return outputMap;
        }
        private Dictionary<string, string> GetSkuDirChunk(string dirPath)
        {
            string[] pdfPaths = Directory.GetFiles(dirPath);
            string dirName = Path.GetFileName(dirPath);
            List<string> skus = new List<string>();
            Dictionary<string, string> outputChunk = new Dictionary<string, string>();
            for (int i = 0; i < pdfPaths.Length; i++) if (Path.GetExtension(pdfPaths[i]) == ".pdf")skus.Add(GetSku(pdfPaths[i]));
            for (int i = 0; i < skus.Count; i++) outputChunk.Add(skus[i], dirName);
            return outputChunk;

        }
        private string GetSku(string path)
        {
            return Path.GetFileNameWithoutExtension(path).Substring(7);
        }
        private KeyValuePair<string, string[]> GetFilePaths(KeyValuePair<string, string> kvp)
        {
            List<string> filePaths = new List<string>();
            filePaths.Add(GetMatchingPath(Directory.GetFiles(Path.Combine(Configurator.Puzzles, kvp.Value)), kvp.Key));
            filePaths.Add(GetMatchingPath(Directory.GetFiles(Path.Combine(Configurator.Posters, kvp.Value)), kvp.Key));
            filePaths.Add(GetMatchingPath(Directory.GetFiles(Path.Combine(Configurator.Sleeves, kvp.Value)), kvp.Key));
            return new KeyValuePair<string, string[]>(kvp.Key, filePaths.ToArray());
        }
        private string GetMatchingPath(string[] filePaths, string key)
        {
            for(int i = 0; i < filePaths.Length; i++)
            {
                if (!filePaths[i].Contains("pdf")) continue;
                if (filePaths[i].Contains(key)) return filePaths[i];
            }
            Logger.WriteLog("No matches found for {0} in {1}", false, key, Path.GetDirectoryName(filePaths[0]));
            return null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaPuzzles_OrderAssembler
{
    internal class Batch
    {
        public Product[] Products;
        string ReportPath;
        string JobNumber;
        public Batch(string reportPath, string jobNumber) 
        { 
            JobNumber = jobNumber;
            ReportPath = reportPath;
            Products = GenerateProducts();
        }
        private Product[] GenerateProducts()
        {
            List<Product> productList = new List<Product>();
            StreamReader reader = new StreamReader(ReportPath);
            if (reader.ReadLine().Split(",").Length != 2) Logger.ErrorExit(["Report CSV is not formatted properly."], 104);
            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split(",");
                if (line.Length != 2)
                {
                    Logger.WriteLog("Line formatted correctly: ", false);
                    if (line.Length == 0) Logger.WriteLog("Line is empty.", false);
                    for (int i = 0; i < line.Length; i++) Logger.WriteLog(line[i], false);
                }
                else productList.Add(new Product(line[0].ToUpper(), line[1]));
            }
            reader.Close();
            return productList.ToArray();
        }
        public int[] GetPuzzleCounts()
        {
            int sPuzzles = 0, lPuzzles = 0, bPuzzles = 0, otherPuzzles = 0;
            for(int i = 0; i < Products.Count(); i++)
            {
                switch (Products[i].PuzzleSize.ToUpper())
                {
                    case "S":
                        sPuzzles++;
                        break;
                    case "L":
                        lPuzzles++;
                        break;
                    case "B":
                        bPuzzles++;
                        break;
                    default:
                        otherPuzzles++;
                        break;
                }
            }
            return [sPuzzles, lPuzzles, bPuzzles, otherPuzzles];
        }
    }
}

using PdfSharp.Drawing;
using PdfSharp.Internal;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaPuzzles_OrderAssembler
{
    internal class Assembler
    {
        public Product[] ShortSKUs;
        public Product[] LongSKUs;
        public Product[] BigSKUs;
        PdfDocument[] PuzzleDocuments;
        PdfDocument[] PosterDocuments;
        PdfDocument[] SleeveDocuments;
        string[] PuzzlePaths;
        string[] PosterPaths;
        string[] SleevePaths;
        List<PdfDocument> OutputDocuments; 

        public Assembler(Product[] products, Dictionary<string, string[]> map) 
        {
            PuzzleDocuments = new PdfDocument[3];
            PosterDocuments = new PdfDocument[3];
            SleeveDocuments = new PdfDocument[3];
            OutputDocuments = new List<PdfDocument>();
            PuzzlePaths = GetTempPaths("Puzzles");
            PosterPaths = GetTempPaths("Posters");
            SleevePaths = GetTempPaths("Sleeves");
            SortOrders(products);
            CombineIndividuals(ShortSKUs, map, "short");
            CombineIndividuals(LongSKUs, map, "long");
            CombineIndividuals(BigSKUs, map, "big");
            AddUIDs(0);
            AddUIDs(1);
            AddUIDs(2);
            CombineFinals();
        }
        private void SortOrders(Product[] products)
        {
            List<Product> shortSKUs = new List<Product>();
            List<Product> longSKUs = new List<Product>();
            List<Product> bigSKUs = new List<Product>();
            for(int i = 0; i < products.Length; i++)
            {
                switch (products[i].SKU.Split("_")[^1].ToLower()) 
                {
                    case "s":
                        shortSKUs.Add(products[i]);
                        break;
                    case "l":
                        longSKUs.Add(products[i]);
                        break;
                    case "b":
                        bigSKUs.Add(products[i]);
                        break;
                    default:
                        Logger.WriteLog("Error with sku {0}", false, products[i].SKU);
                        break;
                }
            }
            ShortSKUs = SortBySize(shortSKUs, "short");
            LongSKUs = SortBySize(longSKUs, "long");
            BigSKUs = SortBySize(bigSKUs, "big");
        }
        private Product[] SortBySize(List<Product> unsortedList, string type)
        {
            string[] pieces = new string[2] { "100", (type=="big") ? "800" : "500" };
            List<Product> JR = new List<Product>();
            List<Product> SR = new List<Product>();
            List<Product> Sorted = new List<Product>();
            for(int i = 0; i < unsortedList.Count; i++)
            {
                if (unsortedList[i].SKU.Split("_")[1] == pieces[0]) JR.Add(unsortedList[i]);
                else if (unsortedList[i].SKU.Split("_")[1] == pieces[1]) SR.Add(unsortedList[i]);
                else Logger.WriteLog("Error processing sku: {0}", false, unsortedList[i].SKU);
            }
            for(int i = 0; i < JR.Count; i++) Sorted.Add(JR[i]);
            for(int i = 0; i < SR.Count; i++) Sorted.Add(SR[i]);
            return Sorted.ToArray();
        }
        private void CombineIndividuals(Product[] products, Dictionary<string, string[]> map, string type)
        {
            int documentIndex = GetDocumentIndex(type);
            for(int i = 0; i < products.Length; i++)
            {
                if (!CheckSkuPieces(map, products[i].SKU)) continue;
                PdfDocument puzzleArt = PdfReader.Open(GetPathOfType("puzzle", map[products[i].SKU]), PdfDocumentOpenMode.Import);
                PdfDocument posterArt = PdfReader.Open(GetPathOfType("poster", map[products[i].SKU]), PdfDocumentOpenMode.Import);
                PdfDocument sleeveArt = PdfReader.Open(GetPathOfType("sleeve", map[products[i].SKU]), PdfDocumentOpenMode.Import);
                int sIndex = 1, bIndex = 1, lIndex = 1;
                for (int ii = 0; ii < products[i].Quantity; ii++)
                {
                    PuzzleDocuments[documentIndex].AddPage(puzzleArt.Pages[0]);
                    PosterDocuments[documentIndex].AddPage(posterArt.Pages[0]);
                    if (posterArt.PageCount == 2) PosterDocuments[documentIndex].AddPage(posterArt.Pages[1]);
                    else PosterDocuments[documentIndex].AddPage(new PdfPage());
                    SleeveDocuments[documentIndex].AddPage(sleeveArt.Pages[0]);
                }
                puzzleArt.Close();
                posterArt.Close();
                sleeveArt.Close();
            }
            if (PuzzleDocuments[documentIndex].Pages.Count > 0) PuzzleDocuments[documentIndex].Save(PuzzlePaths[documentIndex]);
            PuzzleDocuments[documentIndex].Close();
            if (PosterDocuments[documentIndex].Pages.Count > 0) PosterDocuments[documentIndex].Save(PosterPaths[documentIndex]);
            PosterDocuments[documentIndex].Close();
            if (SleeveDocuments[documentIndex].Pages.Count > 0) SleeveDocuments[documentIndex].Save(SleevePaths[documentIndex]);
            SleeveDocuments[documentIndex].Close();
        }
        private void CombineFinals()
        {
            OutputDocuments.Add(new PdfDocument());
            GenerateFinalDocument(OutputDocuments[^1], PuzzlePaths);
            OutputDocuments.Add(new PdfDocument());
            GenerateFinalDocument(OutputDocuments[^1], PosterPaths);
            OutputDocuments.Add(new PdfDocument());
            GenerateFinalDocument(OutputDocuments[^1], SleevePaths);
        }
        private void GenerateFinalDocument(PdfDocument document, string[] paths)
        {
            for(int i = 0; i < paths.Length; i++)
            {
                if (!File.Exists(paths[i])) continue;
                PdfDocument tempDoc = PdfReader.Open(paths[i], PdfDocumentOpenMode.Import);
                for (int ii = 0; ii < tempDoc.Pages.Count; ii++) document.AddPage(tempDoc.Pages[ii]);
                tempDoc.Close();
            }
            string outputName = Path.GetFileName(paths[0]).Replace("Short","Batch").Replace("Long", "Batch").Replace("Big","Batch");
            document.Save(Path.Combine(Configurator.BatchOutput, outputName));
        }
        private bool CheckSkuPieces(Dictionary<string, string[]> map, string sku)
        {
            if(map[sku].Length != 3)
            {
                Logger.WriteLog("Piece error; Did not find 3 pieces for sku {0}.", false, sku);
                return false;
            }
            return true;
        }
        private string[] GetTempPaths(string type)
        {
            return new string[]
            {
                Path.Combine(Configurator.CombinedIndividuals, String.Format("Short_{0}_{1}.pdf", type, DateTime.Now.ToString("MMddyy"))),
                Path.Combine(Configurator.CombinedIndividuals, String.Format("Long_{0}_{1}.pdf", type, DateTime.Now.ToString("MMddyy"))),
                Path.Combine(Configurator.CombinedIndividuals, String.Format("Big_{0}_{1}.pdf", type, DateTime.Now.ToString("MMddyy")))
            };
        }
        private string GetPathOfType(string type, string[] paths)
        {
            for(int i = 0; i < paths.Length; i++) if (paths[i].ToLower().Contains(type.ToLower())) return paths[i];
            return null;
        }
        private int GetDocumentIndex(string type)
        {
            int index;
            switch (type){
                case "short":
                    index = 0;
                    break;
                case "long":
                    index = 1;
                    break;
                case "big":
                    index = 2;
                    break;
                default:
                    return -1;
            }
            PuzzleDocuments[index] = new PdfDocument();
            PosterDocuments[index] = new PdfDocument();
            SleeveDocuments[index] = new PdfDocument();
            return index;
        }
        private void AddUIDs(int size)
        {
            if (!File.Exists(PuzzlePaths[size])) return;
            PdfDocument puzzleDoc = PdfReader.Open(PuzzlePaths[size]);
            PdfDocument posterDoc = PdfReader.Open(PosterPaths[size]);
            PdfDocument sleeveDoc = PdfReader.Open(SleevePaths[size]);
            string uidPrefix = (size == 0) ? "S" : (size == 1) ? "L" : "B";
            int count = 1;
            for(int i = 0; i < puzzleDoc.PageCount; i++)
            {
                //SHARED
                string uidComplete = String.Format("{0}{1}", uidPrefix, count.ToString("0000"));
                var xFont = new XFont("Verdana", 7);
                //FOR POSTER, NEED BACK PAGE, page index = COUNT+i
                var gfx = XGraphics.FromPdfPage(posterDoc.Pages[count + i], XGraphicsPdfPageOptions.Append);
                var xRect = new XRect(5, 666, 110, 0);
                gfx.DrawString(uidComplete, xFont, XBrushes.Black, xRect, XStringFormats.Default);
                //FOR PUZZLE
                gfx = XGraphics.FromPdfPage(puzzleDoc.Pages[i], XGraphicsPdfPageOptions.Append);
                xRect = new XRect(100, 20, 110, 18);
                gfx.DrawString(uidComplete, xFont, XBrushes.Black, xRect, XStringFormats.Center);
                //FOR SLEEVE
                gfx = XGraphics.FromPdfPage(sleeveDoc.Pages[i], XGraphicsPdfPageOptions.Append);
                xRect = new XRect(1685, 100, 110, 18);
                gfx.RotateAtTransform(270, new XPoint(1685, 100));
                gfx.DrawString(uidComplete, xFont, XBrushes.White, xRect, XStringFormats.Center);
                //Shared, again.
                count++;
            }
            puzzleDoc.Save(PuzzlePaths[size]);
            posterDoc.Save(PosterPaths[size]);
            sleeveDoc.Save(SleevePaths[size]);
            puzzleDoc.Close();
            posterDoc.Close();
            sleeveDoc.Close();

        }
    }
}

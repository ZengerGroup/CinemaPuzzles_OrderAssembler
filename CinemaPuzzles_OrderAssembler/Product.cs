using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaPuzzles_OrderAssembler
{
    internal class Product
    {
        public string SKU;
        public int Quantity;
        public string PuzzleSize;
        public Product(string sku, string qty)
        {
            SKU = sku;
            if (!Int32.TryParse(qty, out Quantity)) Logger.ErrorExit([String.Format("Quantity for {0] is not correctly formatted.", SKU)], 102);
            PuzzleSize = GetSize();
            if(PuzzleSize == "_") Logger.ErrorExit(["Unable to determine puzzle size. Please Check archive and resubmit."], 600);
        }
        private string GetSize()
        {
            try { return SKU.Split("_")[2]; }
            catch { return "_"; }
        }
    }
}

using PdfSharp.Fonts;
using PdfSharp.Snippets.Font;

namespace CinemaPuzzles_OrderAssembler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Logger.WriteLog("Beginning system setup.", true);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            GlobalFontSettings.FontResolver = new FailsafeFontResolver();
            if (args.Length != 2) Logger.ErrorExit(["Processing requires two arguments."], 10);
            else Logger.JobNumber = args[1];
            Logger.WriteLog("Beginning assembly", true);
            if (!File.Exists(args[0])) Logger.ErrorExit(["Unable to access report file."], 101);
            ProductMapper ProductMap = new ProductMapper();
            Batch DaysBatch = new Batch(args[0], args[1]);
            Assembler BatchAssembler = new Assembler(DaysBatch.Products, ProductMap.Map);
            Mailer ReportMailer = new Mailer(args[1]);
            ReportMailer.SendMail(DaysBatch.GetPuzzleCounts());
        }
    }
}

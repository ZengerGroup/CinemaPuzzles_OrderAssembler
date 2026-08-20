using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaPuzzles_OrderAssembler
{
    internal static class Configurator
    {
        public static string LogPath = ConfigurationManager.AppSettings["LogPath"];
        public static string Puzzles = ConfigurationManager.AppSettings["ProductionPuzzles"];
        public static string Posters = ConfigurationManager.AppSettings["ProductionPosters"];
        public static string Sleeves = ConfigurationManager.AppSettings["ProductionSleeves"];
        public static string CombinedIndividuals = ConfigurationManager.AppSettings["CombinedIndividuals"];
        public static string BatchOutput = ConfigurationManager.AppSettings["BatchOutput"];
        public static string MailAccount = ConfigurationManager.AppSettings["SenderAccount"];
        public static string MailSecret = ConfigurationManager.AppSettings["SenderSecret"];
        public static string MailRecipient = ConfigurationManager.AppSettings["EmailRecipients"];
        public static string Reports = ConfigurationManager.AppSettings["ReportDirectory"];
        public static string Travelers = ConfigurationManager.AppSettings["TravelerDirectory"];
        public static string Attachments = ConfigurationManager.AppSettings["AttachmentDirectory"];
        public static string ToMove = ConfigurationManager.AppSettings["MoveToLive"];
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Transmitter.Services
{
    public static class LoggingService
    {
        public static void LogException(Exception ex)
        {
            DeleteOldies();
            var path = Path.Combine("Logs", $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}hs.txt");
            File.WriteAllText(path, ex.ToString());
        }

        private static void DeleteOldies()
        {
            var directory = new DirectoryInfo("Logs");
            var oldies = directory.GetFiles()
                                    .Where(x => x.CreationTime.Date < DateTime.Today.AddDays(-30))
                                    .Select(x => x.FullName)
                                    .ToList();
            foreach (var oldie in oldies)
                File.Delete(oldie);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxieTrackerDownloader
{
    

    class Program
    {

        static Persistence.Repositories.TrackerDownloader _tracker;
        static Persistence.Repositories.LogWriter _log;

        static void Main(string[] args)
        {

            var logPath = string.Format(ConfigurationManager.AppSettings["LogPath"].ToString(), DateTime.Now.AddHours(-8));

            _log = new Persistence.Repositories.LogWriter(logPath);

            _log.Write("Job Started");

            _tracker = new Persistence.Repositories.TrackerDownloader();
            _tracker.APIURL = ConfigurationManager.AppSettings["AxieAPITracker"].ToString();
            _tracker.ProcessSuccessEvent += _tracker_ProcessSuccessEvent;
            _tracker.ProcessRoninReadEvent += _tracker_ProcessRoninReadEvent;
            _tracker.ProcessRoninReadErrorEvent += _tracker_ProcessRoninReadErrorEvent;

            _tracker.DownloadAsync().Wait();

            _log.Write("Job End");

        }

        private static void _tracker_ProcessRoninReadErrorEvent(string message)
        {
            _log.Write("_tracker_ProcessRoninReadErrorEvent " + message);
            Console.WriteLine(message);
        }

        private static void _tracker_ProcessRoninReadEvent(string message)
        {
            _log.Write("_tracker_ProcessRoninReadEvent " + message);
            Console.WriteLine(message);
        }

        private static void _tracker_ProcessSuccessEvent(string message)
        {
            _log.Write("_tracker_ProcessSuccessEvent " + message);
            Console.WriteLine(message);
        }
    }
}

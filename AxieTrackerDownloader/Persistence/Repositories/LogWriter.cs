using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxieTrackerDownloader.Persistence.Repositories
{
    public class LogWriter
    {
        string _logPath;
        public LogWriter(string logPath)
        {
            _logPath = logPath;
        }

        public void Write(string message)
        {
            try
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(_logPath, true))
                {
                    sw.WriteLine("{0} --> {1}", DateTime.Now, message);
                }

            }
            catch (Exception)
            {

                //throw;
            }
        }
    }
}

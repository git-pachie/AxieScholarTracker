using AxieTrackerDownloader.Persistence.DB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AxieTrackerDownloader.Persistence.Repositories
{
    public delegate void ExecuteDelegate(string message);
    public class TrackerDownloader
    {
        public string APIURL { get; set; }
        public event ExecuteDelegate ProcessSuccessEvent;
        public event ExecuteDelegate ProcessRoninReadEvent;
        public event ExecuteDelegate ProcessRoninReadErrorEvent;

        private AxieScholarDBEntities _axiedB;
        
        public TrackerDownloader()
        {
            _axiedB = new Persistence.DB.AxieScholarDBEntities();
        }

       

        public async Task DownloadAsync()
        {
            if (ProcessSuccessEvent != null)
            {
                ProcessSuccessEvent("DownloadAsync Started");
            }

            foreach (var ronin in _axiedB.ScholarProfiles.Where(x => x.IsEnabled == true))
            {
                var roninAddress = ronin.RoninAddress;

                string url = string.Format("{0}/{1}", APIURL, roninAddress);

                HttpClient client = new HttpClient();


                try
                {
                    


                    string response = await client.GetStringAsync(url);

                    if (ProcessRoninReadEvent != null)
                    {
                        ProcessRoninReadEvent("Ronin URL Request " + url);
                    }

                    var data = JsonConvert.DeserializeObject<AxieTrackerDownloader.Models.TrackerModel>(response);


                    var batchCode = Convert.ToDateTime(string.Format("{0:yyyy-MM-dd}", DateTime.Now.AddHours(-8)));

                    var record = _axiedB
                        .RoninSLPs
                        .Where(x => x.RoninID == roninAddress && x.DateBatchCode == batchCode)
                        .FirstOrDefault();

                    var lastClaimDate = ConvertFromUnixTimestamp(data.slpData.lastClaim);

                    if (record != null)
                    {

                        record.LastUpdated = DateTime.Now;
                        record.Name = ronin.ScholarName;
                        record.MMR = data.leaderboardData.elo;
                        record.WalletSLP = data.slpData.roninSlp;
                        record.InGameSLP = data.slpData.gameSlp;
                        record.TotalSLP = data.slpData.totalSlp;
                        record.LastUpdated = DateTime.Now;
                        record.LastClaimDate = lastClaimDate;
                        record.RawSLPDailyYesterday = data.databaseData.slpDailyYesterday;
                        record.RawSLPTotalYesterday = data.databaseData.slpTotalYesterday;
                    }
                    else
                    {
                        _axiedB.RoninSLPs.Add(new Persistence.DB.RoninSLP
                        {
                            DateBatchCode = batchCode
                         ,
                            RoninID = roninAddress
                         ,
                            Name = ronin.ScholarName
                         ,
                            MMR = data.leaderboardData.elo
                         ,
                            WalletSLP = data.slpData.roninSlp
                          ,
                            InGameSLP = data.slpData.gameSlp
                          ,
                            LastClaimDate = lastClaimDate
                          ,
                            NextClaimDate = DateTime.Now
                          ,
                            TotalSLP = data.slpData.totalSlp
                          ,
                            DateCreated = DateTime.Now
                          ,
                            CreatedBy = "Job"
                          ,
                            LastUpdated = DateTime.Now
                            ,
                            RawSLPDailyYesterday = data.databaseData.slpDailyYesterday
                            ,
                            RawSLPTotalYesterday = 0

                        });
                    }

                    Console.WriteLine("Done: " + roninAddress);

                }
                catch (Exception ex)
                {
                    if (ProcessRoninReadErrorEvent != null)
                    {
                        ProcessRoninReadErrorEvent(ex.Message + " __ " + url + "__" + ronin.ScholarName);
                    }

                    //Console.WriteLine(ex);
                    throw ex;
                }

            }

            _axiedB.SaveChanges();

            if(ProcessSuccessEvent != null)
            {
                ProcessSuccessEvent("DownloadAsync Success");
            }

        }


        private DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

    }
}

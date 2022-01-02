using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AxieTrackerDownloader.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

    //public class TrackerModel
    //{
    //    public bool success { get; set; }
    //    public long cache_last_updated { get; set; }
    //    public int draw_total { get; set; }
    //    public int lose_total { get; set; }
    //    public int win_total { get; set; }
    //    public int total_matches { get; set; }
    //    public int win_rate { get; set; }
    //    public int mmr { get; set; }
    //    public int rank { get; set; }
    //    public int ronin_slp { get; set; }
    //    public int total_slp { get; set; }
    //    public int raw_total { get; set; }
    //    public int in_game_slp { get; set; }
    //    public int last_claim { get; set; }
    //    public int lifetime_slp { get; set; }
    //    public string name { get; set; }
    //    public int next_claim { get; set; }
    //}



    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class SlpData
    {
        public bool slpSuccess { get; set; }
        public int gameSlp { get; set; }
        public int totalSlp { get; set; }
        public int roninSlp { get; set; }
        public int lastClaim { get; set; }
        public long updatedOn { get; set; }
    }

    public class LeaderboardData
    {
        public bool leaderboardSuccess { get; set; }
        public int elo { get; set; }
        public int rank { get; set; }
    }

    public class DatabaseData
    {
        public string roninAddress { get; set; }
        public long? slpTotalYesterday { get; set; }
        public long? slpDailyYesterday { get; set; }
        public DateTime updatedOn { get; set; }
        public bool included { get; set; }
    }

    public class TrackerModel
    {
        public SlpData slpData { get; set; }
        public LeaderboardData leaderboardData { get; set; }
        public DatabaseData databaseData { get; set; }
    }



}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Dijitle.Metra.Data
{
    public class StopTimes
    {

        public string trip_id { get; private set; } 
        public DateTime arrival_time { get; private set; } 
        public DateTime departure_time { get; private set; } 
        public string stop_id { get; private set; } 
        public int stop_sequence { get; private set; } 
        public StopType pickup_type { get; private set; } 
        public StopType drop_off_type { get; private set; } 
        public bool center_boarding { get; private set; } 
        public bool south_boarding { get; private set; } 
        public bool bikes_allowed { get; private set; } 
        public string notice { get; private set; }

        public Trips Trip { get; set; }
        public Stops Stop { get; set; }

        public enum StopType
        {
            RegularlyScheduled = 0,
            None = 1,
            MustPhoneAgency = 2,
            MustCoordinateWithDriver = 3
        }

        public StopTimes(string[] csv)
        {
            trip_id = csv[0].Trim();     
            arrival_time = GetTime(csv[1].Trim());
            departure_time = GetTime(csv[2].Trim());
            stop_id = csv[3].Trim();
            stop_sequence = Convert.ToInt32(csv[4].Trim());
            pickup_type = (StopType)Convert.ToInt32(csv[5].Trim());
            drop_off_type = (StopType)Convert.ToInt32(csv[6].Trim());
            center_boarding = (csv[7].Trim() == "1");
            south_boarding = (csv[8].Trim() == "1");
            bikes_allowed = (csv[9].Trim() == "1");
            notice = csv[10].Trim();
        }

        private DateTime GetTime(string time)
        {
            MatchCollection matches = Regex.Matches(time, @"^(?<hour>\d{2}):(?<minute>\d{2}):(?<second>\d{2})$");

            int hour = Convert.ToInt32(matches[0].Groups["hour"].Value);
            int minute = Convert.ToInt32(matches[0].Groups["minute"].Value);
            int second = Convert.ToInt32(matches[0].Groups["second"].Value);
            
            return new DateTime(1,1,1).AddHours(hour).AddMinutes(minute).AddSeconds(second);
        }
        
        public override string ToString()
        {
            return stop_id;
        }
    }
}


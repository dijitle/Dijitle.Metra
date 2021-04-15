using System;

namespace Dijitle.Metra.Data
{
    public class StopTimes
    {

        public string trip_id { get; private set; } 
        public string arrival_time { get; private set; } 
        public string departure_time { get; private set; } 
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
            arrival_time = csv[1].Trim().PadLeft(8, '0');
            departure_time = csv[2].Trim().PadLeft(8, '0');
            stop_id = csv[3].Trim();
            stop_sequence = Convert.ToInt32(csv[4].Trim());
            pickup_type = (StopType)Convert.ToInt32(csv[5].Trim());
            drop_off_type = (StopType)Convert.ToInt32(csv[6].Trim());

            if(csv.Length > 7)
            {
                center_boarding = (csv[7].Trim() == "1");
                south_boarding = (csv[8].Trim() == "1");
                bikes_allowed = (csv[9].Trim() == "1");
                notice = csv[10].Trim();
            }
        }
        
        public override string ToString()
        {
            return stop_id;
        }
    }
}


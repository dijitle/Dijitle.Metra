using System;

namespace Dijitle.Metra.Data
{

    public class Speeds
    {
        public string log_entry { get; set; }
        public int TIMES_LOGGED { get; set; }
        public string direction { get; set; }
        public string tripId { get; set; }
        public int sequence { get; set; }
        public string AVG_TIMESTAMP_AT_SEQUENCE { get; set; }
        public double total_distance_from_origin { get; set; }
        public double AVG_ELAPSED_TIME_FROM_PREV_SEQ { get; set; }
        public double DIST_TRAVELLED_FROM_PREV_SEQ { get; set; }
        public double AVG_SPEED_FROM_PREV_SEQ { get; set; }
    }

}

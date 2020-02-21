using System;
using System.Collections.Generic;

namespace Dijitle.Metra.Data
{
    public class Dateish
    {
        public DateTime low { get; set; }
        public int high { get; set; }
        public bool unsigned { get; set; }
    }

    public class ActivePeriod
    {
        public Dateish start { get; set; }
        public Dateish end { get; set; }
    }

    public class AlertTrip
    {
        public string trip_id { get; set; }
        public string route_id { get; set; }
        public string direction_id { get; set; }
        public string start_time { get; set; }
        public string start_date { get; set; }
        public int schedule_relationship { get; set; }
    }

    public class InformedEntity
    {
        public string agency_id { get; set; }
        public string route_id { get; set; }
        public string route_type { get; set; }
        public AlertTrip trip { get; set; }
        public object stop_id { get; set; }
    }

    public class Translation
    {
        public string text { get; set; }
        public string language { get; set; }
    }

    public class Url
    {
        public List<Translation> translation { get; set; }
    }

    public class HeaderText
    {
        public List<Translation> translation { get; set; }
    }

    public class DescriptionText
    {
        public List<Translation> translation { get; set; }
    }

    public class AlertsAlert
    {
        public List<ActivePeriod> active_period { get; set; }
        public List<InformedEntity> informed_entity { get; set; }
        public int cause { get; set; }
        public int effect { get; set; }
        public Url url { get; set; }
        public HeaderText header_text { get; set; }
        public DescriptionText description_text { get; set; }
    }

    public class Alerts
    {
        public string id { get; set; }
        public bool is_deleted { get; set; }
        public object trip_update { get; set; }
        public object vehicle { get; set; }
        public AlertsAlert alert { get; set; }
    }
}

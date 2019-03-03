using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Dijitle.Metra.Data
{
    public class Shapes
    {
        public string shape_id { get; private set; }
        public decimal shape_pt_lat { get; private set; }
        public decimal shape_pt_lon { get; private set; }
        public int shape_pt_sequence { get; private set; }

        public Shapes(string[] csv)
        {
            shape_id = csv[0].Trim();
            shape_pt_lat = Convert.ToDecimal(csv[1].Trim());
            shape_pt_lon = Convert.ToDecimal(csv[2].Trim());
            shape_pt_sequence = Convert.ToInt32(csv[3].Trim());
        }

        public override string ToString()
        {
            return shape_pt_sequence.ToString();
        }
    }
}

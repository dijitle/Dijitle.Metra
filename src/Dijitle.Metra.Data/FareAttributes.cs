using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace Dijitle.Metra.Data
{
    public class FareAttributes
    {
        public int fare_id { get; private set; }
        public decimal price { get; private set; }
        public string currency_type { get; private set; }
        public PaymentMethod payment_method { get; set; }
        public Transfers transfers { get; private set; }
        public int transfer_duration { get; private set; }

        public enum PaymentMethod
        {
            OnBoard = 0,
            PreBoard = 1
        }

        public enum Transfers
        {
            None = 0,
            Once = 1,
            Twice = 2,
            Unlimited = -1
        }

        public FareAttributes(string[] csv)
        {
            fare_id = Convert.ToInt32(csv[0].Trim());
            price = Convert.ToDecimal(csv[1].Trim());
            currency_type = csv[2].Trim();
            payment_method = (PaymentMethod)Convert.ToInt32(csv[3].Trim());
            transfers = (Transfers)Convert.ToInt32(csv[4].Trim());
            transfer_duration = Convert.ToInt32(csv[5].Trim());

        }
        
        public override string ToString()
        {
            return price.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace Dijitle.Metra.Data
{
    public class FareAttributes
    {
        public string fare_id { get; private set; }
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

        public FareAttributes(Dictionary<string, string> dictData)
        {
            fare_id = dictData["fare_id"];
            price = Convert.ToDecimal(dictData["price"]);
            currency_type = dictData["currency_type"];
            payment_method = (PaymentMethod)Convert.ToInt32(dictData["payment_method"]);

            int.TryParse(dictData["transfers"], out var trans);
            transfers = (Transfers)trans;

            if (dictData.ContainsKey("transfer_duration"))
            {
                transfer_duration = Convert.ToInt32(dictData["transfer_duration"]);
            }

        }
        
        public override string ToString()
        {
            return price.ToString();
        }
    }
}

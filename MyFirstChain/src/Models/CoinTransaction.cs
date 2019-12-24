using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstChain.src.Models
{
    public class CoinTransaction
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public int Amount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstChain.src.Models
{
    public class Block
    {

        public Block(int currentChainAmount, string prevHash = "0", int proff = 1)
        {
            this.TransactionList = new List<CoinTransaction>();
            this.Index = currentChainAmount;
            this.TimeSpame = DateTime.Now.ToUniversalTime().ToString();
            this.Proff = proff;
            this.PreviousHash = prevHash;
            this.Data = "";
        }

        public IEnumerable<CoinTransaction> TransactionList { get; set; }
        private string _index;
        private string _timeStame;
        private string _proff;
        private string _prevHash;
        private string _data;

        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public int Index
        {
            get { return int.Parse(_index); }
            set { _index = value.ToString(); }
        }
        public string TimeSpame
        {
            get { return (_timeStame); }
            set
            {
                var temp = value;
                _timeStame = temp.ToString();
            }
        }
        public int Proff
        {
            get { return int.Parse(_proff); }
            set { _proff = value.ToString(); }
        }
        public string PreviousHash
        {
            get { return _prevHash; }
            set { _prevHash = value; }
        }
    }
}

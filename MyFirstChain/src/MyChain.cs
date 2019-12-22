using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstChain.src
{
    public interface IChain
    {
        IEnumerable<Block> BlockChain { get; set; }
        Block CreateBlock(string prevHash = "0", int proff = 1);
        Block PrivousBlock();
        int ProffOfWork(int prevProff);
        string HashBlock(Block newBlock);
        bool IsChainValid(IEnumerable<Block> chain);
    }
    public class MyChain : IChain
    {
        public IEnumerable<Block> BlockChain { get; set; }
        public MyChain()
        {
            if (BlockChain==null)
            {
                BlockChain = new List<Block>() {
                new Block(this.BlockChain)
            };
            }
         
        }

        public Block CreateBlock(string prevHash = "0", int proff = 1)
        {
            var newBlock = new Block(BlockChain, prevHash, proff);
            BlockChain = BlockChain.Append(newBlock);
            var count = BlockChain.Count();
            return newBlock;
        }
        public Block PrivousBlock() => this.BlockChain.ToList().SingleOrDefault(s => s.Index == this.BlockChain.ToList().Count() - 1);

        public int ProffOfWork(int prevProff)
        {
            var newProff = 1;
            var checkProff = false;
            using (var hashOpration = SHA256.Create())
            {
                byte[] encodeBuffer, hash;
                string hashHexString;
                string itemStr;
                while (!checkProff)
                {

                    encodeBuffer = Encoding.ASCII.GetBytes((
                          (Math.Sqrt(newProff) * 2)
                       - (Math.Sqrt(prevProff) * 2)
                       ).ToString());
                    hash = hashOpration.ComputeHash(encodeBuffer);

                    hashHexString = "";
                    for (int i = 0; i < 3; i++)
                    {
                        byte item = hash[i];
                        switch (item.ToString().Length)
                        {
                            case 1:
                                itemStr = $"00{item}";
                                break;
                            case 2:
                                itemStr = $"0{item}";
                                break;
                            case 3:
                                itemStr = $"{item}";
                                break;
                            default:
                                itemStr = "000";
                                break;
                        }
                        hashHexString += itemStr;
                    }
                    if (hashHexString.Substring(0, 4).Equals("0000"))
                        checkProff = true;
                    else
                        newProff++;
                }

            }
            return newProff;

        }

        public string HashBlock(Block newBlock)
        {
            var jsonChain = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(newBlock));
            byte[] hash;
            using (var hashOpration = SHA256.Create())
            {
                hash = hashOpration.ComputeHash(jsonChain);

            }
            var hashStr = "";
            string itemStr;
            foreach (var item in hash)
            {
                switch (item.ToString().Length)
                {
                    case 1:
                        itemStr = $"00{item}";
                        break;
                    case 2:
                        itemStr = $"0{item}";
                        break;
                    case 3:
                        itemStr = $"{item}";
                        break;
                    default:
                        itemStr = "000";
                        break;
                }
                hashStr += itemStr;
            }



            return hashStr;
        }

        public bool IsChainValid(IEnumerable<Block> chain)
        {
            var prevBlock = chain.ElementAt(0);

            for (int i = 1; i < chain.Count(); i++)
            {
                var block = chain.ElementAt(i);
                if (block.PreviousHash != HashBlock(prevBlock))
                    return false;
                using (var hashOpration = SHA256.Create())
                {
                    var encodeBuffer = Encoding.ASCII.GetBytes((
                           (Math.Sqrt(block.Proff) * 2)
                        - (Math.Sqrt(prevBlock.Proff) * 2)
                        ).ToString());
                    var hash = hashOpration.ComputeHash(encodeBuffer);

                    if (!hash.ToString().Substring(0, 4).Equals("0000"))
                        return false;
                }
                prevBlock = block;

            }

            return true;
        }
    }

    public class Block
    {

        public Block(IEnumerable<Block> currentChain, string prevHash = "0", int proff = 1)
        {
            if (currentChain != null)
                this.Index = currentChain.Count() ;
            else
                this.Index = 0;
            this.TimeSpame = DateTime.Now.ToUniversalTime();
            this.Proff = proff;
            this.PreviousHash = prevHash;
            this.Data = "";
        }


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
        public DateTime TimeSpame
        {
            get { return DateTime.Parse(_timeStame); }
            set
            {
                var temp = value.ToUniversalTime();
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

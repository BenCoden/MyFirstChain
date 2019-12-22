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
            if (BlockChain == null)
            {
                BlockChain = new List<Block>()
                {
                        new Block(0)
                 };
            }
        }

        public Block CreateBlock(string prevHash = "0", int proff = 1)
        {
            var newBlock = new Block(BlockChain.Count(), prevHash, proff);
            if (IsChainValid(BlockChain.Append(newBlock)))
            {
                BlockChain = BlockChain.Append(newBlock);
            }
            else
            {
                return null;
            }
            
            return newBlock;
        }
        public Block PrivousBlock() => this.BlockChain.SingleOrDefault(s => s.Index == this.BlockChain.Count() - 1);
        public int ProffOfWork(int prevProff)
        {
            var newProff = 1;
            using (var hashOpration = CreateHashing())
            {
                byte[] encodeBuffer, hash;
                string hashHexString;
                var checkProff = false;

                while (!checkProff)
                {
                    encodeBuffer = EncodedProff(prevProff, newProff);
                    hash = hashOpration.ComputeHash(encodeBuffer);
                    hashHexString = ByteArrayToString(hash);

                    if (IsProffValid(hashHexString))
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
            using (var hashOpration = CreateHashing())
            {
                hash = hashOpration.ComputeHash(jsonChain);
            }
            var hashStr = ByteArrayToString(hash);
            return hashStr;
        }

        public bool IsChainValid(IEnumerable<Block> chain)
        {
            var prevBlock = chain.ElementAt(0);
            using (var hashOpration = CreateHashing())
            {
                Block block;
                byte[] encodeBuffer, hash;
                string hashHexString;
                for (int i = 1; i < chain.Count(); i++)
                {
                    block = chain.ElementAt(i);
                    if (block.PreviousHash != HashBlock(prevBlock))
                        return false;

                    encodeBuffer = EncodedProff(prevBlock.Proff, block.Proff);
                    hash = hashOpration.ComputeHash(encodeBuffer);
                    hashHexString = ByteArrayToString(hash);

                    if (!IsProffValid(hashHexString))
                        return false;

                    prevBlock = block;
                }
            }

            return true;
        }
        #region Private methods

        private string ByteArrayToString(byte[] arg)
        {
            var result = "";
            foreach (var item in arg)
            {
                string itemStr;
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
                result += itemStr;
            }
            return result;
        }

        private byte[] EncodedProff(int prevProff, int newProff)
        {
            byte[] result = Encoding.ASCII.GetBytes((
                          (Math.Sqrt(newProff) * 2)
                       - (Math.Sqrt(prevProff) * 2)
                       ).ToString());
            return result;
        }

        private bool IsProffValid(string arg)
        {
            bool result = arg.Substring(0, 6).Equals("000000");
            return result;
        }

        private SHA512 CreateHashing()
        {
            return SHA512.Create();
        }


        #endregion
    }

    public  class Block
    {

        public Block(int currentChainAmount, string prevHash = "0", int proff = 1)
        {

            this.Index = currentChainAmount;
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

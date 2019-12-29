using HttpUtils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using MyFirstChain.src.Models;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstChain.src
{
    public interface IChain
    {
        public int AddTransaction(string sender, string receiver, int amount);

        IEnumerable<Block> BlockChain { get; }
        IEnumerable<string> Nodes { get; }

        Block CreateBlock(string prevHash = "0", int proff = 1);

        Block PrivousBlock();

        int ProffOfWork(int prevProff);

        string HashBlock(Block newBlock);

        bool IsChainValid(IEnumerable<Block> chain);

        IEnumerable<string> AddNode(IEnumerable<string> arg);

        void ReplaceChain();
    }

    public class MyChain : IChain
    {
        private readonly IChainUtils _chainUtils;
        public IEnumerable<string> UrlNodes { get; private set; }
        public IEnumerable<CoinTransaction> UnconfirmedTransactionList { get; private set; }
        public IEnumerable<string> Nodes { get; private set; }
        public IEnumerable<Block> BlockChain { get; private set; }

        public MyChain(IChainUtils chainUtils)
        {
            _chainUtils = chainUtils;
            if (BlockChain == null)
            {
                BlockChain = new List<Block>()
                {
                        new Block(0)
                 };
                UnconfirmedTransactionList = new List<CoinTransaction>();
                Nodes = new List<string>();
            }
        }

        #region Chain Work

        public Block CreateBlock(string prevHash = "0", int proff = 1)
        {
            var newBlock = new Block(BlockChain.Count(), prevHash, proff)
            {
                TransactionList = this.UnconfirmedTransactionList
            };
            if (IsChainValid(BlockChain.Append(newBlock)))
                BlockChain = BlockChain.Append(newBlock);
            else
                return null;

            this.UnconfirmedTransactionList = new List<CoinTransaction>();
            return newBlock;
        }

        public Block PrivousBlock() => this.BlockChain.SingleOrDefault(s => s.Index == this.BlockChain.Count() - 1);

        public int ProffOfWork(int prevProff)
        {
            var newProff = 1;
            using (var hashOpration = _chainUtils.CreateHashing())
            {
                byte[] encodeBuffer, hash;
                string hashHexString;
                var checkProff = false;

                while (!checkProff)
                {
                    encodeBuffer = _chainUtils.EncodedProff(prevProff, newProff);
                    hash = hashOpration.ComputeHash(encodeBuffer);
                    hashHexString = _chainUtils.ByteArrayToString(hash);

                    if (_chainUtils.IsProffValid(hashHexString))
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
            using (var hashOpration = _chainUtils.CreateHashing())
            {
                hash = hashOpration.ComputeHash(jsonChain);
            }
            var hashStr = _chainUtils.ByteArrayToString(hash);
            return hashStr;
        }

        public bool IsChainValid(IEnumerable<Block> chain)
        {
            if (chain.Count() <= 0)
                return false;

            var prevBlock = chain.ElementAt(0);
            using (var hashOpration = _chainUtils.CreateHashing())
            {
                Block block;
                byte[] encodeBuffer, hash;
                string hashHexString;
                for (int i = 1; i < chain.Count(); i++)
                {
                    block = chain.ElementAt(i);
                    if (block.PreviousHash != HashBlock(prevBlock))
                        return false;

                    encodeBuffer = _chainUtils.EncodedProff(prevBlock.Proff, block.Proff);
                    hash = hashOpration.ComputeHash(encodeBuffer);
                    hashHexString = _chainUtils.ByteArrayToString(hash);

                    if (!_chainUtils.IsProffValid(hashHexString))
                        return false;

                    prevBlock = block;
                }
            }

            return true;
        }

        #endregion Chain Work

        public int AddTransaction(string sender, string receiver, int amount)
        {
            UnconfirmedTransactionList = UnconfirmedTransactionList.Append(new CoinTransaction()
            {
                Amount = amount,
                Sender = sender,
                Receiver = receiver
            });
            //Index of next block
            return BlockChain.Count();
        }

        public IEnumerable<string> AddNode(IEnumerable<string> arg)
        {
            var newNodes = new List<string>();
            foreach (var item in arg)
            {
                if (!Nodes.Contains(item))
                {
                    newNodes.Add(item);
                    Nodes = Nodes.Append(item);
                }
            }
            return newNodes;
        }

        public void ReplaceChain()
        {
            IEnumerable<Block> longestChain = new List<Block>();
            IEnumerable<Block> orphanBlocks = new List<Block>();
            int maxLength = BlockChain.Count();
            IEnumerable<RequestChain> requestChainList = new List<RequestChain>()
            {
                new RequestChain()
                {
                    length=maxLength,
                    chain=BlockChain
                }
            };
            foreach (var network in Nodes)
            {
                var request = new RestClient($"{network}/GetChain");
                var requestChain = JsonConvert.DeserializeObject<RequestChain>(request.MakeRequest());
                if (!IsChainValid(requestChain.chain))
                    return;
                if (requestChain.length == maxLength)
                {
                    if (!requestChainList.Contains(requestChain))
                        requestChainList = requestChainList.Append(requestChain);
                }
                else if (requestChain.length > maxLength)
                {
                    requestChainList = new List<RequestChain>();
                    requestChainList = requestChainList.Append(requestChain);

                    foreach (var orphan in longestChain.Where(w => w.Index > (maxLength-1)))
                        if (!orphanBlocks.Contains(orphan))
                            orphanBlocks.Append(orphan);

                    longestChain = requestChain.chain;
                    maxLength = requestChain.length;
                }
            }

            if (requestChainList.Count() > 1)
            {//compare all block and get oldest chain
                var possableLongestChain = longestChain;
                var tempRequestArray = requestChainList.ToArray();
                var baseChain = tempRequestArray[0].chain.OrderBy(o => o.Index).ToArray();

                for (int i = 1; i < tempRequestArray.Count(); i++)
                {
                    var nThChainArray = tempRequestArray[i].chain.OrderBy(o => o.Index).ToArray();
                    for (int ii = 0; ii < baseChain.Count(); ii++)
                    {
                        if (!baseChain[ii].TimeSpame.Equals(nThChainArray[ii].TimeSpame))
                        {
                            if (DateTime.Parse(baseChain[ii].TimeSpame) > DateTime.Parse(nThChainArray[ii].TimeSpame))
                            {
                                foreach (var orphan in baseChain.Where(w => w.Index >= ii))
                                    if (!orphanBlocks.Contains(orphan))
                                        orphanBlocks = orphanBlocks.Append(orphan);

                                baseChain = nThChainArray;
                                possableLongestChain = nThChainArray;
                                break;
                            }
                            else
                            {
                                foreach (var orphan in nThChainArray.Where(w => w.Index >= ii))
                                    if (!orphanBlocks.Contains(orphan))
                                        orphanBlocks = orphanBlocks.Append(orphan);
                            }
                        }
                    }
                }

                longestChain = possableLongestChain;
            }

            //add transaction baack
            foreach (var item in orphanBlocks)
            {
                foreach (var transaction in item.TransactionList)
                {
                    AddTransaction(transaction.Sender, transaction.Receiver, transaction.Amount);
                }
            }

            if (longestChain.Count() > 0 && IsChainValid(longestChain))
                BlockChain = longestChain;
        }
    }
}
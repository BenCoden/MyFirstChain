using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyFirstChain.src;
using MyFirstChain.src.Models;

namespace MyFirstChain.Controllers
{
    public class BlockChainController : ControllerBase
    {
        private IChain _myChain;

        public BlockChainController(IChain chain)
        {
            _myChain = chain;
        }

        [Route("MineBlock"), HttpGet()]
        public IActionResult Index()
        {
            var prevBlock = _myChain.PrivousBlock();
            var proff = _myChain.ProffOfWork(prevBlock.Proff);
            var prevHash = _myChain.HashBlock(prevBlock);
            var block = _myChain.CreateBlock(prevHash, proff);
            if (block != null)
                return Accepted(new
                {
                    message = "Winner, Winner Chick Dinner!",
                    index = block.Index,
                    Transaction = block.TransactionList,
                    timeStamp = block.TimeSpame,
                    proff = block.Proff,
                    prevHash = block.PreviousHash
                });

            return BadRequest();
        }

        [Route("GetChain"), HttpGet()]
        public IActionResult GetChain()
        {
            var response = new
            {
                chain = _myChain.BlockChain,
                length = _myChain.BlockChain.Count()
            };

            return Ok(response);
        }

        [Route("IsChainValid"), HttpGet()]
        public IActionResult IsChainValid()
        {
            return Ok(_myChain.IsChainValid(_myChain.BlockChain));
        }

        [Route("AddTransaction"), HttpPost]
        public IActionResult AddTransaction([FromBody]CoinTransaction arg)
        {
            var result = _myChain.AddTransaction(arg.Sender, arg.Receiver, arg.Amount);
            return Ok(new
            {
                message = "Transaction Added to pool",
                transaction = arg,
                blockIndex = result
            });
        }

        [Route("AddNode"), HttpPost()]
        public IActionResult AddNode([FromBody]NodeReuqest arg)
        {
            var oldNode = _myChain.Nodes;
            var newNodes = _myChain.AddNode(arg.NewNode);
            return Ok(new
            {
                addNodes = newNodes,
                oldNodes = oldNode
            });
        }

        [Route("SyncChain"), HttpGet()]
        public IActionResult SyncChain()
        {
            _myChain.ReplaceChain();
            return Ok(_myChain.BlockChain);
        }
    }
}
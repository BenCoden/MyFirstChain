using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyFirstChain.src;

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

            return Accepted(new
            {
                message = "Winner, Winner Chick Dinner!",
                index = block.Index,
                timeStamp = block.TimeSpame,
                proff = block.Proff,
                prevHash = block.PreviousHash
            });
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
    }
}
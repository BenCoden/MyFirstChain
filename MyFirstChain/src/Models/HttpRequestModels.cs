using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstChain.src.Models
{
    public class RequestChain
    {
        public IEnumerable<Block> chain { get; set; }
        public int length { get; set; }
    }



    public class NodeReuqest
    {
        public string NewNode { get; set; }
    }
}

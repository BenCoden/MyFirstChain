using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstChain.src
{public interface IChainUtils
    {
        string ByteArrayToString(byte[] arg);
        byte[] EncodedProff(int prevProff, int newProff);
        bool IsProffValid(string arg);
        SHA256 CreateHashing();
    }
    public class ChainUtils : IChainUtils
    {
        public string ByteArrayToString(byte[] arg)
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

        public byte[] EncodedProff(int prevProff, int newProff)
        {
            byte[] result = Encoding.ASCII.GetBytes((
                          (Math.Sqrt(newProff) * 2)
                       - (Math.Sqrt(prevProff) * 2)
                       ).ToString());
            return result;
        }

        public bool IsProffValid(string arg)
        {
            bool result = arg.Substring(0, 6).Equals("000000");
            return result;
        }

        public SHA256 CreateHashing()
        {
            return SHA256.Create();
        }

    }
}

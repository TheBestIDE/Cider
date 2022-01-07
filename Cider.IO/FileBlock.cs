using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.IO
{
    public class FileBlock
    {
        public string HashCode { get; set; }

        public byte[] Data { get; set; }

        public FileBlock (string hash, byte[] data)
        {
            HashCode = hash;
            Data = data;
        }
    }
}

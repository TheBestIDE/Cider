﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cider.IO
{
    /// <summary>
    /// 分块的文件
    /// </summary>
    public class BlockedFile
    {
        public string FileName { get; set; }

        public string? Path { get; set; }

        public IList<string> BlockHashList { get; set; }

        public IList<int>? DifferentBlockPositionList { get; set; }

        public FileBlock[]? DifferentFileBlocks { get; set; }

        public BlockedFile()
        {
            FileName = "";
            BlockHashList = new List<string>();
        }
    }
}

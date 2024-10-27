using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcInt
{
    internal class ResultModel
    {
        public string BinaryResultContent { get; set; } = "";
        public string HexadecimalResultContent { get; set; } = "";

        public string PreviousResultContent { get; set; } = "";
        public string ResultContent { get; set; } = "";
    }
}

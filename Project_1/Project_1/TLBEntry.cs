using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_1
{
    class TLBEntry
    {
        #region Fields



        #endregion

        #region Constructors



        #endregion

        #region Properties

        public bool IsValid { get; set; }

        public int Tag { get; set; }

        public uint DataField { get; set; }

        public int LruCycleNumber { get; set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"Tag: 0x{Tag:X} / Data: 0x{DataField:X} / LRU: {LruCycleNumber}";
        }

        #endregion
    }
}

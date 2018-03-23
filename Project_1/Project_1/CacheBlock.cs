using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_1
{
    class CacheBlock
    {
        #region Fields



        #endregion

        #region Constructors



        #endregion

        #region Properties

        public bool IsValid { get; set; } = true;

        public int Tag { get; set; } = 0;

        public int LruCycleNumber { get; set; } = 0;

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"Tag: 0x{Tag:X} / LRU: {LruCycleNumber}";
        }

        #endregion
    }
}

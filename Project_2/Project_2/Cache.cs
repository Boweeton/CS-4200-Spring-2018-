using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Project_2
{
    /// <summary>
    /// 
    /// </summary>
    public class Cache
    {
        #region Fields

        readonly int nAssociativityWay;
        readonly int nBlocks;
        readonly int nBlockIndexBits;
        readonly int nSetIndexBits;
        readonly int nSets;
        readonly int nShiftBits;
        readonly int nSetMask;
        readonly CacheEntry[] aCacheEntries;

        #endregion

        #region Constructors

        /// <summary>
        /// Default contructor
        /// </summary>
        public Cache()
        {
        }

        /// <summary>
        /// Standard Cache contructor.
        /// </summary>
        /// <param name="CacheSize">The total size of the cache in bytes.</param>
        /// <param name="BlockSize">The size of each block within the cache sets.</param>
        /// <param name="Asscociativity">The set associativity of the cache sets.</param>
        public Cache(int CacheSize, int BlockSize, int Asscociativity)
        {
            // Find block bits
            nBlockIndexBits = QuickLog2(BlockSize);

            // Check for invalid block size
            if ((1 << nBlockIndexBits) != BlockSize)
            {
                throw new System.ArgumentException($"The Block Size {BlockSize} must be a power of 2.");
            }

            // Check for even divisibility between CacheSize and BlockSize
            if (CacheSize % BlockSize != 0)
            {
                throw new System.ArgumentException($"Cache size {CacheSize} and Block size {BlockSize} need to be evenly divisible.");
            }

            // Set Blocks
            nBlocks = CacheSize / BlockSize;

            // If new Associativity is exactly 0, set it to nBlocks before continuing
            if (Asscociativity == 0)
            {
                Asscociativity = nBlocks;
            }

            // Check for blocks not being divisible by associativity
            if (nBlocks % Asscociativity != 0)
            {
                throw new System.ArgumentException($"{nBlocks} blocks is not evenly divisible by way-{Asscociativity}.");
            }

            // Set Sets
            nSets = nBlocks / Asscociativity;

            // Check for sets not being power of 2
            if (nSets * Asscociativity != nBlocks)
            {
                throw new System.ArgumentException($"Sets {nSets} must be a power of 2.");
            }

            // Set up Cache now that checks are completed
            nSetIndexBits = QuickLog2(nSets);
            nSetMask = (1 << nSetIndexBits) - 1;
            nShiftBits = nSetIndexBits + nBlockIndexBits;
            nAssociativityWay = Asscociativity;
            aCacheEntries = new CacheEntry[nBlocks];
        }

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Gets data out of the cache.
        /// </summary>
        /// <param name="uAddress">The address of the data.</param>
        /// <param name="uCycle">The current cycle.</param>
        /// <returns></returns>
        public bool Get(uint uAddress, uint uCycle)
        {
            bool bFound = false;
            int nSet = (int)(uAddress >> nBlockIndexBits) & nSetMask;
            uint uTag = uAddress >> nShiftBits;
            int nStartBlock;
            int nEndBlock;

            if (nAssociativityWay == nBlocks)
            {
                // If the cache is fully associative
                nStartBlock = 0;
                nEndBlock = nBlocks - 1;
            }
            else
            {
                nStartBlock = nSet * nAssociativityWay;
                nEndBlock = (nStartBlock + nAssociativityWay) - 1;
            }

            // Cycle through the blocks to find the match
            uint uMinLru = 0xffffffff;
            int nMinLruIndex = 0;
            for (int i = nStartBlock; !bFound && i <= nEndBlock; i++)
            {
                if (aCacheEntries[i].uTag == uTag)
                {
                    bFound = true;
                    aCacheEntries[i].uLru = uCycle;
                }
                else if (uMinLru > aCacheEntries[i].uLru)
                {
                    uMinLru = aCacheEntries[i].uLru;
                    nMinLruIndex = i;
                }
            }

            if (!bFound)
            {
                aCacheEntries[nMinLruIndex].uTag = uTag;
                aCacheEntries[nMinLruIndex].uLru = uCycle;
            }

            return bFound;
        }

        /// <summary>
        /// Prints ot contents of cache into a .txt file.
        /// </summary>
        /// <param name="strFileName">Given file name.</param>
        public void Print(string strFileName)
        {
            StreamWriter sw = new StreamWriter(strFileName);

            // Print General Cache Info
            sw.WriteLine($"Number of Blocks: {nBlocks}");
            sw.WriteLine($"Number of Block Index Bits: {nBlockIndexBits}");
            sw.WriteLine($"Number of Set Index Bits: {nSetIndexBits}");
            sw.WriteLine($"Number of Sets: {nSets}");
            sw.WriteLine($"Number of Tag Bits: {32 - nShiftBits}");
            sw.WriteLine($"The Set Mask: {nSetMask:X4}");

            // Print out contents of the Cache Entries
            if (nAssociativityWay == nBlocks)
            {
                // For fully associative caches
                sw.WriteLine($"Block\tTag\tLRU");
                for (int i = 0; i < nBlocks; i++)
                {
                    sw.WriteLine($"{i}\t0x{aCacheEntries[i].uTag:X}\t{aCacheEntries[i].uLru}");
                }
            }
            else
            {
                sw.Write("Set\t");
                for (int i = 0; i < nAssociativityWay; i++)
                {
                    sw.Write("Block\tTag\tLRU\t");
                }
                sw.WriteLine();
                for (int i = 0; i < nSets; i++)
                {
                    bool bPrint = false;
                    for (int j = 0; !bPrint && j < nAssociativityWay; j++)
                    {
                        bPrint = (aCacheEntries[(i * nAssociativityWay) + j].uLru != 0);
                    }

                    if (bPrint)
                    {
                        sw.Write($"{i}\t");
                        for (int j = 0; j < nAssociativityWay; j++)
                        {
                            int nIndex = (i * nAssociativityWay) + j;
                            sw.Write($"{nIndex}\t0x{aCacheEntries[nIndex].uTag:X}\t{aCacheEntries[nIndex].uLru}\t");
                        }
                        sw.WriteLine();
                    }
                }
            }
            sw.Flush();
            sw.Close();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculates the nearest up flush-power of 2 Log2 of the input.
        /// </summary>
        /// <param name="n">The input number.</param>
        /// <returns></returns>
        static int QuickLog2(int n)
        {
            int nResult = 0;
            for (; (n & 1) == 0; nResult++, n >>= 1)
            {
            }

            return nResult;
        }

        #endregion

        #endregion

        #region LocalStructs

        /// <summary>
        /// Struct to hold Cache entry information.
        /// </summary>
        struct CacheEntry
        {
            public uint uTag;
            public uint uLru;
        }

        #endregion
    }
}
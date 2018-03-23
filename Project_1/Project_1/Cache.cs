using System.Collections.Generic;
using System.IO;
using System.Text;
using static System.Math;

namespace Project_1
{
    class Cache
    {
        #region Fields



        #endregion

        #region Constructors


        public Cache(int newCacheSize, int newBlockSize, int newAssociativity)
        {
            // Construction Calculations
            BlockCount = newCacheSize / newBlockSize;
            SetOffsetWidth = QuickLog2(newAssociativity);
            BlockOffsetWidth = QuickLog2(BlockCount);

            // Create the list of blocks
            Blocks = new List<CacheBlock>();
            for (int i = 0; i < BlockCount; i++)
            {
                Blocks.Add(new CacheBlock());
            }
        }

        #endregion

        #region Properties

        public List<CacheBlock> Blocks { get; set; }

        public int Associativity { get; set; }

        public int BlockCount { get; set; }

        public int SetOffsetWidth { get; set; }

        public int BlockOffsetWidth { get; set; }

        #endregion

        #region Methods


        #region PublicMethods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uAddress"></param>
        /// <param name="uCycle"></param>
        /// <returns></returns>
        public bool Get(uint uAddress, uint uCycle)
        {
            // Calculate which set to look in
            int localSetBase = (int)ExtractBits(uAddress, 16, SetOffsetWidth);

            // Loop through the set to find the data


            return false;
        }

        /// <summary>
        /// Prints the contents of the Cache to the given .txt file. (Creates the .txt file if it doesn't exist.)
        /// </summary>
        /// <param name="fileName">The given file name to print to or create.</param>
        public void Print(string fileName)
        {
            // Create the file (relativly pathed)
            StreamWriter sw = File.CreateText(fileName);

            // If the TLB is fully associative
            if (Associativity == 0)
            {
                // Print header
                sw.Write("Block\t");
                sw.Write("Tag\t");
                sw.Write("LRU\t");
                sw.Write("Data");
                sw.WriteLine();

                for (int i = 0; i < Blocks.Count; i++)
                {
                    CacheBlock block = Blocks[i];

                    if (block.IsValid)
                    {
                        // Block #
                        sw.Write($"{i}\t");

                        // Tag HEX
                        sw.Write($"0x{block.Tag:X}\t");

                        // LRU #
                        sw.Write($"{block.LruCycleNumber}\t");
                    }
                }
            }
            else
            {
                // Print header
                sw.Write("Set\t|\t");
                for (int i = 0; i < Associativity; i++)
                {
                    sw.Write("Valid\t");
                    sw.Write("Block\t");
                    sw.Write("Tag\t");
                    sw.Write("LRU\t");
                    sw.Write("Data\t|\t");
                }

                sw.WriteLine();

                // Loop through each set
                for (int i = 0; i < Blocks.Count; i += Associativity)
                {
                    bool setHasData = false;

                    // Check to see if the current set has data
                    for (int j = i; j < Associativity; j++)
                    {
                        setHasData = Blocks[j].IsValid;
                    }

                    // Print the set data if there is indeed data in it
                    if (true)
                    {
                        // Print set #
                        int setNumber = (i / Associativity) + 1;
                        sw.Write($"{setNumber}\t|\t");
                        for (int j = i; j < Associativity; j++)
                        {
                            // Print isValid
                            sw.Write($"{Blocks[j].IsValid}\t");

                            // Print block #
                            sw.Write($"{j}\t");

                            // Print tag info
                            sw.Write($"{Blocks[j].Tag}\t");

                            // Print LRU
                            sw.Write($"{Blocks[j].LruCycleNumber}\t");
                        }

                        sw.WriteLine();
                    }
                }
            }

            sw.Flush();
        }

        #endregion

        #region PrivateMethods

        /// <summary>
        /// Calculates the uint value of just the subset of bits requestd by the params.
        /// </summary>
        /// <param name="baseUint">The given uint from which the subset will be extracted.</param>
        /// <param name="offsetGoingLeft">The bits to the left of the desired subset that are not needed.</param>
        /// <param name="bitFieldWidth">The width of the bit subset desired.</param>
        /// <returns></returns>
        uint ExtractBits(uint baseUint, int offsetGoingLeft, int bitFieldWidth)
        {
            // Local Declarations
            uint product = baseUint;

            // Chop out everything we don't want
            product = product << (32 - (bitFieldWidth + offsetGoingLeft));
            product = product >> (32 - bitFieldWidth);

            return product;
        }

        /// <summary>
        /// Finds the log2 of an input, rounded up to the next power of 2.
        /// </summary>
        /// <param name="n">The input number to be logged.</param>
        /// <returns></returns>
        int QuickLog2(int n)
        {
            int nResult = 0;
            // ReSharper disable once EnforceForStatementBraces
            for (; (n & 1) == 0; nResult++, n >>= 1) ;
            return nResult;
        }

        #endregion


        #endregion
    }
}
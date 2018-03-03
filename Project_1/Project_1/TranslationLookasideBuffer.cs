using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_1
{
    class TranslationLookasideBuffer
    {
        #region Fields



        #endregion

        #region Constructors

        /// <summary>
        /// Constructor to fully set up the TLB with the given params.
        /// </summary>
        /// <param name="newEntryCount">Number of entries in the TLB in total.</param>
        /// <param name="newAssociativity">Associativity of the TLB (number of entries in a given block).</param>
        /// <param name="newTlbTagBits">Number of bits in the "Tab" portion of the entries in the TLB.</param>
        public TranslationLookasideBuffer(int newEntryCount, int newAssociativity, int newTlbTagBits)
        {
            // Set the properties to the params
            Associativity = newAssociativity;
            TlbTagBits = newTlbTagBits;

            // Construct the list of entries
            Entries = new List<TLBEntry>();
            for (int i = 0; i < newEntryCount; i++)
            {
                Entries.Add(new TLBEntry());
            }
        }

        #endregion

        #region Properties

        public List<TLBEntry> Entries { get; set; }

        public int Associativity { get; set; }

        public int TlbTagBits { get; set; }

        #endregion

        #region Methods


        #region PublicMethods

        public uint GetData()
        {
            return 0;
        }

        /// <summary>
        /// Sets data into the TLB. 
        /// </summary>
        /// <param name="uData">The data that's being stored.</param>
        /// <param name="uAddress">The Address to be decomposed to get info (based on the TLB Associativity and Number of Tag Bits).</param>
        /// <param name="uCycle">The current cycle.</param>
        public void SetData(uint uData, uint uAddress, uint uCycle)
        {
            // Local Declarations
            int setBitsWidth = Associativity; // Todo: find actual calculation here
            int blockBitsWidth = (int)Math.Log(Entries.Count, 2); // Todo: find actual calculation here
            int tagWidth = 32 - (blockBitsWidth + setBitsWidth);
            uint localTag = ExtractBits(uAddress, (setBitsWidth + blockBitsWidth), tagWidth);
            bool hasLoggedCurrentEntry = false;

            // Find the tag if present, if so: update LruCycle
            foreach (TLBEntry entry in Entries)
            {
                if (entry.Tag == localTag)
                {
                    // Log the entry
                    entry.LruCycleNumber = (int)uCycle;

                    hasLoggedCurrentEntry = true;
                }
            }

            // If the tag is not present, find an empty entry, if so: dump the data into the entry and update LruCycle
            if (!hasLoggedCurrentEntry)
            {
                foreach (TLBEntry entry in Entries)
                {
                    if (entry.IsValid == false)
                    {
                        // Log the entry
                        entry.IsValid = true;
                        entry.Tag = (int)localTag;
                        entry.DataField = uData;
                        entry.LruCycleNumber = (int)uCycle;

                        hasLoggedCurrentEntry = true;
                    }
                }
            }

            // If the Tag isn't present, and no entries are invalid: find the oldest entry, dump the data into the entry and update LruCycle
            if (!hasLoggedCurrentEntry)
            {
                int oldestLruCycle = int.MaxValue;

                // Find the oldest
                foreach (TLBEntry entry in Entries)
                {
                    if (entry.LruCycleNumber < oldestLruCycle && entry.LruCycleNumber != 0)
                    {
                        oldestLruCycle = entry.LruCycleNumber;
                    }
                }

                // Find that oldest one and overwrite it
                foreach (TLBEntry entry in Entries)
                {
                    if (entry.LruCycleNumber == oldestLruCycle)
                    {
                        // Log the entry
                        entry.Tag = (int)localTag;
                        entry.DataField = uData;
                        entry.LruCycleNumber = (int)uCycle;
                    }
                }
            }
        }

        /// <summary>
        /// Prints the contents of the TLB to the given .txt file. (Creates the .txt file if it doesn't exist.
        /// </summary>
        /// <param name="fileName">The given file name to print to or create.</param>
        public void Print(string fileName)
        {
            // Indent constants
            const int OffsetAfterBlock = 10;
            const int OffsetAfterTag = 15;
            const int OffsetAfterLru = 10;

            // Create the file (relativly pathed)
            StreamWriter sw = File.CreateText(fileName);

            // Print header
            sw.Write($"{"Block",-OffsetAfterBlock}");
            sw.Write($"{"Tag",-OffsetAfterTag}");
            sw.Write($"{"LRU",-OffsetAfterLru}");
            sw.Write("Data");
            sw.WriteLine();

            for (int i = 0; i < Entries.Count; i++)
            {
                TLBEntry entry = Entries[i];
                // Block #
                sw.Write($"{i, -OffsetAfterBlock}");

                // Tag HEX
                sw.Write($"0x{entry.Tag,-OffsetAfterTag:X}");

                // LRU #
                sw.Write($"{entry.LruCycleNumber,-OffsetAfterLru}");

                // Data Hex
                sw.Write($"0x{entry.DataField:X}");
                sw.WriteLine();
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

        #endregion


        #endregion
    }
}
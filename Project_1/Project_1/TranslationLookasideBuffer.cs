using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

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

        /// <summary>
        /// Retrieves data from the specified address after decompisition. (Returns 0xffffffff if it's a miss.)
        /// </summary>
        /// <param name="uAddress">The Address to be decomposed to get info (based on the TLB Associativity and Number of Tag Bits).</param>
        /// <param name="uCycle">The current cycle.</param>
        /// <returns></returns>
        public uint GetData(uint uAddress, uint uCycle)
        {
            // Local Declarations
            int setBitsWidth = Associativity;
            int blockBitsWidth = TlbTagBits;
            int localSetBegining = (int)ExtractBits(uAddress, blockBitsWidth, setBitsWidth);
            int tagWidth = 32 - (blockBitsWidth + setBitsWidth);
            uint localTag = ExtractBits(uAddress, (setBitsWidth + blockBitsWidth), tagWidth);

            // Look through the desired set
            for (int i = localSetBegining; i < Associativity; i++)
            {
                if (Entries[i].Tag == localTag)
                {
                    Entries[i].LruCycleNumber = (int)uCycle;
                    return Entries[i].DataField;
                }
            }

            return 0xffffffff;
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
            int setBitsWidth = Associativity;
            int blockBitsWidth = TlbTagBits;
            int tagWidth = 32 - (blockBitsWidth + setBitsWidth);
            uint localTag = ExtractBits(uAddress, (setBitsWidth + blockBitsWidth), tagWidth);
            bool hasLoggedCurrentEntry = false;
            int localSet = (int)ExtractBits(uAddress, 16, 7);

            // Find the tag if present, if so: update LruCycle
            foreach (TLBEntry entry in Entries)
            {
                if (entry.Tag == localTag && !hasLoggedCurrentEntry)
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
                    if (entry.IsValid == false && !hasLoggedCurrentEntry)
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
                    if (entry.LruCycleNumber == oldestLruCycle && !hasLoggedCurrentEntry)
                    {
                        // Log the entry
                        entry.Tag = (int)localTag;
                        entry.DataField = uData;
                        entry.LruCycleNumber = (int)uCycle;

                        hasLoggedCurrentEntry = true;
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

                for (int i = 0; i < Entries.Count; i++)
                {
                    TLBEntry entry = Entries[i];

                    if (entry.IsValid)
                    {
                        // Block #
                        sw.Write($"{i}\t");

                        // Tag HEX
                        sw.Write($"0x{entry.Tag:X}\t");

                        // LRU #
                        sw.Write($"{entry.LruCycleNumber}\t");

                        // Data Hex
                        sw.Write($"0x{entry.DataField:X4}");
                        sw.WriteLine();
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
                for (int i = 0; i < Entries.Count; i += Associativity)
                {
                    bool setHasData = false;

                    // Check to see if the current set has data
                    for (int j = i; j < Associativity; j++)
                    {
                        setHasData = Entries[j].IsValid;
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
                            sw.Write($"{Entries[j].IsValid}\t");

                            // Print block #
                            sw.Write($"{j}\t");

                            // Print tag info
                            sw.Write($"{Entries[j].Tag}\t");

                            // Print LRU
                            sw.Write($"{Entries[j].LruCycleNumber}\t");

                            // Print data in HEX
                            sw.Write($"0x{Entries[j].DataField:X}\t|\t");
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

        #endregion


        #endregion
    }
}
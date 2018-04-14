using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2
{
    class Program
    {
        static void Main()
        {
            uint uCycle = 1;

            //L1 cache is 32k 64B, 4 way
            Cache L1Cache = new Cache(32768, 64, 4);
            L1Cache.Get(0xABCDEF02, uCycle++);
            L1Cache.Get(0xABCEEF12, uCycle++);
            L1Cache.Get(0xABCFEF22, uCycle++);
            L1Cache.Get(0xABC0EF32, uCycle++);
            L1Cache.Get(0xABC1EF32, uCycle++);
            L1Cache.Print("L1Cache.txt");
            System.Diagnostics.Process.Start("notepad.exe", "L1Cache.txt");

            //L2 cache is 256K, 64B, 16-way
            Cache L2Cache = new Cache(1048576, 64, 16);
            L2Cache.Get(0xABCDEF02, uCycle++);
            L2Cache.Get(0xABCEEF02, uCycle++);
            L2Cache.Get(0xABCFEF02, uCycle++);
            L2Cache.Get(0xABD0EF02, uCycle++);
            L2Cache.Get(0xABD1EF02, uCycle++);
            L2Cache.Get(0xABD2EF02, uCycle++);
            L2Cache.Get(0xABD3EF02, uCycle++);
            L2Cache.Get(0xABD4EF02, uCycle++);
            L2Cache.Get(0xABD5EF02, uCycle++);
            L2Cache.Get(0xABD6EF02, uCycle++);
            L2Cache.Get(0xABD7EF02, uCycle++);
            L2Cache.Get(0xABD8EF02, uCycle++);
            L2Cache.Get(0xABD9EF02, uCycle++);
            L2Cache.Get(0xABDAEF02, uCycle++);
            L2Cache.Get(0xABDBEF02, uCycle++);
            L2Cache.Get(0xABDCEF02, uCycle++);
            L2Cache.Get(0xABDDEF02, uCycle++);
            L2Cache.Print("L2Cache.txt");
            System.Diagnostics.Process.Start("notepad.exe", "L2Cache.txt");

        }
    }
}

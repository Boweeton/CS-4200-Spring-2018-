using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_1
{
    class Program
    {
        static void Main(string[] args)
        {
            uint uCycle = 1;

            // Micro TLB is 10 entries, fully associative, 16-bit data value
            TranslationLookasideBuffer microTlb = new TranslationLookasideBuffer(10, 0, 16);
            microTlb.SetData(0x10, 0xABCDEF12, uCycle++);
            microTlb.SetData(0x20, 0xBBCDEF12, uCycle++);
            microTlb.SetData(0x30, 0xCBCDEF12, uCycle++);
            microTlb.SetData(0x40, 0xDBCDEF12, uCycle++);
            microTlb.SetData(0x50, 0xEBCDEF12, uCycle++);
            microTlb.SetData(0x60, 0xFBCDEF12, uCycle++);
            microTlb.Print("MicroTLB1.txt");

            // The following line fires up Notepad to save the time of doing it manually
            System.Diagnostics.Process.Start("notepad.exe", "MicroTLB1.txt");

            microTlb.SetData(0x70, 0x0BCDEF12, uCycle++);
            microTlb.SetData(0x80, 0x1BCDEF12, uCycle++);
            microTlb.SetData(0x90, 0x2BCDEF12, uCycle++);
            microTlb.SetData(0xA0, 0x3BCDEF12, uCycle++);
            microTlb.SetData(0xB0, 0x4BCDEF12, uCycle++);
            microTlb.Print("MicroTLB2.txt");
            System.Diagnostics.Process.Start("notepad.exe", "MicroTLB2.txt");

            //L2 TLB is 512 entries, 4-way, 16-bit data
            TranslationLookasideBuffer l2Tlb = new TranslationLookasideBuffer(512, 4, 16);
            l2Tlb.SetData(0x10, 0xABCDEF12, uCycle++);
            l2Tlb.SetData(0x20, 0xBBCDEF12, uCycle++);
            l2Tlb.SetData(0x30, 0xCBCDEF12, uCycle++);
            l2Tlb.SetData(0x40, 0xDBCDEF12, uCycle++);
            l2Tlb.SetData(0x50, 0xEBCDEF12, uCycle++);
            l2Tlb.SetData(0x60, 0xFBCDEF12, uCycle++);
            l2Tlb.SetData(0x70, 0x0BCDEF12, uCycle++);
            l2Tlb.SetData(0x80, 0x1BCDEF12, uCycle++);
            l2Tlb.SetData(0x90, 0x2BCDEF12, uCycle++);
            l2Tlb.SetData(0xA0, 0x3BCDEF12, uCycle++);
            l2Tlb.SetData(0xB0, 0x4BCDEF12, uCycle++);
            l2Tlb.Print("L2TLB.txt");
            System.Diagnostics.Process.Start("notepad.exe", "L2TLB.txt");


        }
    }
}

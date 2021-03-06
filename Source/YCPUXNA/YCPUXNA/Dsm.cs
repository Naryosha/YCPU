﻿using System.IO;
using Ypsilon.Emulation.Processor;

namespace YCPUXNA
{
    internal class Dsm
    {
        public bool TryDisassemble(string[] args)
        {
            string[] disassembly;

            if (args.Length <= 1)
                return false;

            string inPath = args[1];
            string outPath = inPath + ".disasm";

            if (!File.Exists(inPath))
                return false;

            using (BinaryReader reader = new BinaryReader(File.Open(inPath, FileMode.Open)))
            {
                disassembly = Disassemble(reader);
            }
            File.WriteAllLines(outPath, disassembly);
            return true;
        }

        private string[] Disassemble(BinaryReader reader)
        {
            YCPU ycpu = new YCPU();
            byte[] data = new byte[reader.BaseStream.Length];
            reader.BaseStream.Read(data, 0, data.Length);
            ycpu.BUS.FillROM(data);

            string[] disassembled;
            disassembled = ycpu.Disassemble(0x0000, 32000, false);

            return disassembled;
        }
    }
}

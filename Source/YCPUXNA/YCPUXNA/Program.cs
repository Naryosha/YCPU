﻿using System;
using Ypsilon;

namespace YCPUXNA
{
    internal static class Program
    {
        private const string c_ErrNoArguments = "YCPUXNA: No input specified. Select an option:\n" +
            "    1. Assemble default 'testconsole.asm' file\n" + /* 2. Disassemble default 'testconsole.asm.bin' file.\n" + */
            "    2. Run emulator\n    3. Run assembly tests\n    4. Exit";

        // default entry point
        private static void Main(string[] args)
        {
            StdConsole.ShowConsoleWindow();
            StdConsole.Clear();

            if (args == null || args.Length == 0)
            {
                StdConsole.StdOutWriteLine(c_ErrNoArguments);
                bool waitForKey = true;
                while (waitForKey)
                {
                    ConsoleKeyInfo cki = StdConsole.StdInReadKey();
                    switch (cki.KeyChar)
                    {
                        case '1':
#if DEBUG
                            args = new string[] {"-asm", "../../Examples/testconsole.asm" };
#else
                            args = new[] { "-asm", "../Examples/testconsole.asm" };
#endif
                            waitForKey = false;
                            break;
                        /*case '2':
                        args = new string[] { "-disasm", "../Examples/testconsole.asm.bin" };
                        waitForKey = false;
                        break;*/
                        case '2':
#if DEBUG
                            args = new string[] {"-emu", "../../Examples/testconsole.asm.bin"};
#else
                            args = new[] { "-emu", "../Examples/testconsole.asm.bin" };
#endif
                            waitForKey = false;
                            break;
                        case '3':
                            args = new[] {"-test"};
                            waitForKey = false;
                            break;
                        case '4':
                            StdConsole.HideConsoleWindow();
                            Environment.Exit(1);
                            break;
                    }
                }
            }

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-asm": // run assembler
                        Asm asm = new Asm();
                        asm.AssembleFromArgs(args);
                        StdConsole.StdInReadKey();
                        break;
                    case "-disasm": // run disassembler
                        Dsm disasm = new Dsm();
                        disasm.TryDisassemble(args);
                        StdConsole.StdInReadKey();
                        break;
                    case "-emu": // run emulator!
                        StdConsole.StdOutWriteLine("Starting emulator... ");
                        Emu e = new Emu();
                        e.SetArgs(args);
                        StdConsole.HideConsoleWindow();
                        e.Run();
                        break;
                    case "-test": // run assembly tests
                        StdConsole.StdOutWriteLine(Tests.Run());
                        StdConsole.StdInReadKey();
                        break;
                    default:
                        // do nothing;
                        break;
                }
            }
        }
    }
}

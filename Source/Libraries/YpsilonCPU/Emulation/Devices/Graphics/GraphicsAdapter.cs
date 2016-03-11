﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ypsilon.Emulation.Hardware;

namespace Ypsilon.Emulation.Devices.Graphics
{
    public class GraphicsAdapter : ADevice
    {
        protected override ushort DeviceType
        {
            get { return DeviceTypeGraphicsAdapter; }
        }
        protected override ushort ManufacturerID
        {
            get { return 0x0000; }
        }
        protected override ushort DeviceID
        {
            get { return 0x0000; }
        }
        protected override ushort DeviceRevision
        {
            get { return 0x0000; }
        }

        public GraphicsAdapter(Hardware.YBUS bus)
            : base(bus)
        {
            m_Bank = new MemoryBankLEM();
        }

        protected override void Initialize()
        {
            SetMode_None();
        }

        public override void Dispose()
        {
            m_Bank = null;
        }

        public override ISegmentProvider GetMemoryBank(ushort bank_index)
        {
            return m_Bank;
        }

        protected override ushort ReceiveMessage(ushort param_0, ushort param_1)
        {
            switch (param_0)
            {
                case 0x0000: // SET_MODE
                    SetMode(param_1);
                    break;
                default:
                    return MSG_ERROR;
            }
            return MSG_ACK;
        }

        public override void Display(int busIndex, List<ITexture> textures, IDisplayProvider renderer)
        {
            switch (m_GraphicsMode)
            {
                case GraphicsMode.None:
                    // do nothing;
                    return;
                case GraphicsMode.LEM1802:
                    Update_LEM();
                    ITexture texture = renderer.RenderLEM(m_Bank, m_LEM_CHRRAM, m_LEM_PALRAM);
                    if (texture != null)
                    {
                        texture.DeviceBusIndex = busIndex;
                        textures.Add(texture);
                    }
                    return;
            }
        }

        // Internal Variables
        GraphicsMode m_GraphicsMode = GraphicsMode.None;

        ISegmentProvider m_Bank;

        // Internal Routines
        private void SetMode(ushort i)
        {
            switch (i)
            {
                case 0x0000:
                    SetMode_None();
                    break;
                case 0x0001:
                    SetMode_LEM();
                    break;
            }
        }
        private void SetMode_None()
        {
            m_GraphicsMode = GraphicsMode.None;
        }

        private void SetMode_LEM()
        {
            if (m_GraphicsMode != GraphicsMode.LEM1802)
            {
                byte[] chrram_default = new byte[512];
                System.Buffer.BlockCopy(c_DefaultCharset, 0, chrram_default, 0, 512);
                for (int i = 0; i < 512; i += 1)
                    m_Bank[0x0800 + i] = chrram_default[i];

                byte[] palram_default = new byte[32];
                System.Buffer.BlockCopy(c_DefaultPalette, 0, palram_default, 0, 32);
                for (int i = 0; i < 32; i += 1)
                    m_Bank[0x0C00 + i] = palram_default[i];

                m_GraphicsMode = GraphicsMode.LEM1802;
            }
        }

        private void Update_LEM()
        {
            MemoryBankLEM lem = (MemoryBankLEM)m_Bank;
            if (lem.SCRRAM_Delta)
            {

            }

            if (lem.CHRRAM_Delta)
            {
                Update_LEM_CHRRAM();
                lem.CHRRAM_Delta = false;
            }

            if (lem.PALRAM_Delta)
            {
                Update_LEM_PALRAM();
                lem.PALRAM_Delta = false;
            }
        }

        uint[] m_LEM_CHRRAM = new uint[0x80];
        uint[] m_LEM_PALRAM = new uint[0x10];

        private void Update_LEM_CHRRAM()
        {
            // Assume CHRRAM format is Color (ARGB8888)
            // Each character is 4x8 pixels at 1bit depth, 4 bytes total.
            // byte 0, bit 0-3: 3210
            // byte 0, bit 4-7: 7654
            // byte 1, bit 0-3: 3210
            // byte 1, bit 4-7: 7654
            // ... same for bytes 2 and 3.
            Buffer.BlockCopy(c_DefaultCharset, 0, m_LEM_CHRRAM, 0, 512);
        }

        private void Update_LEM_PALRAM()
        {
            // Assume PALRAM format is Color (ARGB8888)
            for (int i = 0; i < 0x10; i += 1)
            {
                ushort color = (ushort)(m_Bank[0x0C00 + i * 2] + (m_Bank[0x0C00 + i * 2 + 1] << 8));
                m_LEM_PALRAM[i] = (uint)(0xFF000000) | ((uint)(color & 0x0F00) << 12) | ((uint)(color & 0x00F0) << 8) | ((uint)(color & 0x000F) << 4);
            }
        }

        enum GraphicsMode
        {
            None,
            LEM1802
        }

        private static byte[] c_DefaultPalette = new byte[] {
            0x00, 0x00, 0xDA, 0x0F, 0x90, 0x0F, 0x50, 0x08, 0x21, 0x03, 0xD8, 0x01, 0x90, 0x01, 0x42, 0x04, 
            0xEF, 0x06, 0x8F, 0x00, 0x6A, 0x00, 0x34, 0x02, 0x5F, 0x08, 0x0D, 0x01, 0xFF, 0x0F, 0x99, 0x09 };

        private static byte[] c_DefaultCharset = new byte[] {
            0x22, 0xE2, 0x00, 0x00, 0x22, 0xF2, 0x00, 0x00, 0x00, 0xF0, 0x22, 0x22, 0x22, 0xE2, 0x22, 0x22,
            0x00, 0xF0, 0x00, 0x00, 0x22, 0xF2, 0x22, 0x22, 0x22, 0x2E, 0x2E, 0x22, 0x55, 0xD5, 0x55, 0x55,
            0x55, 0x1D, 0x0F, 0x00, 0x00, 0x1F, 0x5D, 0x55, 0x55, 0x0D, 0x0F, 0x00, 0x00, 0x0F, 0x5D, 0x55,
            0x55, 0x1D, 0x5D, 0x55, 0x00, 0x0F, 0x0F, 0x00, 0x55, 0x0D, 0x5D, 0x55, 0x22, 0x0F, 0x0F, 0x00,
            0x55, 0xF5, 0x00, 0x00, 0x00, 0x0F, 0x2F, 0x22, 0x00, 0xF0, 0x55, 0x55, 0x55, 0xF5, 0x00, 0x00,
            0x22, 0x2E, 0x0E, 0x00, 0x00, 0x2E, 0x2E, 0x22, 0x00, 0xF0, 0x55, 0x55, 0x55, 0xF5, 0x55, 0x55,
            0x22, 0x2F, 0x2F, 0x22, 0x22, 0x32, 0x00, 0x00, 0x00, 0xE0, 0x22, 0x22, 0xFF, 0xFF, 0xFF, 0xFF,
            0x00, 0x00, 0xFF, 0xFF, 0x33, 0x33, 0x33, 0x33, 0xCC, 0xCC, 0xCC, 0xCC, 0xFF, 0xFF, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x22, 0x22, 0x02, 0x02, 0x55, 0x00, 0x00, 0x00, 0x50, 0x57, 0x57, 0x00,
            0x72, 0x21, 0x74, 0x02, 0x45, 0x22, 0x12, 0x05, 0x52, 0x25, 0x7D, 0x0C, 0x24, 0x00, 0x00, 0x00,
            0x24, 0x11, 0x21, 0x04, 0x21, 0x44, 0x24, 0x01, 0x50, 0x72, 0x52, 0x00, 0x20, 0x72, 0x22, 0x00,
            0x00, 0x00, 0x20, 0x01, 0x00, 0x70, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x44, 0x22, 0x12, 0x01,
            0x52, 0x55, 0x55, 0x02, 0x32, 0x22, 0x22, 0x07, 0x52, 0x24, 0x12, 0x07, 0x52, 0x24, 0x54, 0x02,
            0x55, 0x75, 0x44, 0x04, 0x17, 0x43, 0x54, 0x02, 0x52, 0x31, 0x55, 0x02, 0x47, 0x24, 0x12, 0x01,
            0x52, 0x25, 0x55, 0x02, 0x52, 0x65, 0x54, 0x02, 0x00, 0x02, 0x20, 0x00, 0x00, 0x02, 0x20, 0x01,
            0x48, 0x12, 0x42, 0x08, 0x00, 0x07, 0x07, 0x00, 0x21, 0x84, 0x24, 0x01, 0x52, 0x24, 0x02, 0x02,
            0x52, 0x75, 0x17, 0x06, 0x52, 0x75, 0x55, 0x05, 0x53, 0x35, 0x55, 0x03, 0x52, 0x11, 0x51, 0x02,
            0x53, 0x55, 0x55, 0x03, 0x17, 0x31, 0x11, 0x07, 0x17, 0x31, 0x11, 0x01, 0x52, 0x71, 0x55, 0x02,
            0x55, 0x75, 0x55, 0x05, 0x27, 0x22, 0x22, 0x07, 0x44, 0x44, 0x54, 0x02, 0x55, 0x33, 0x55, 0x05,
            0x11, 0x11, 0x11, 0x07, 0x75, 0x57, 0x55, 0x05, 0x53, 0x55, 0x55, 0x05, 0x52, 0x55, 0x55, 0x02,
            0x53, 0x35, 0x11, 0x01, 0x52, 0x55, 0x55, 0x42, 0x53, 0x35, 0x55, 0x05, 0x52, 0x21, 0x54, 0x02,
            0x27, 0x22, 0x22, 0x02, 0x55, 0x55, 0x55, 0x07, 0x55, 0x55, 0x25, 0x02, 0x55, 0x55, 0x77, 0x05,
            0x55, 0x25, 0x55, 0x05, 0x55, 0x25, 0x22, 0x02, 0x47, 0x24, 0x11, 0x07, 0x26, 0x22, 0x22, 0x06,
            0x11, 0x22, 0x42, 0x04, 0x46, 0x44, 0x44, 0x06, 0x52, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x70,
            0x42, 0x00, 0x00, 0x00, 0x00, 0x43, 0x56, 0x06, 0x11, 0x53, 0x55, 0x03, 0x00, 0x52, 0x51, 0x02,
            0x44, 0x56, 0x55, 0x06, 0x00, 0x52, 0x17, 0x06, 0x24, 0x72, 0x22, 0x02, 0x00, 0x56, 0x46, 0x03,
            0x11, 0x53, 0x55, 0x05, 0x02, 0x23, 0x22, 0x07, 0x04, 0x44, 0x54, 0x02, 0x11, 0x55, 0x53, 0x05,
            0x23, 0x22, 0x22, 0x07, 0x00, 0x75, 0x57, 0x05, 0x00, 0x53, 0x55, 0x05, 0x00, 0x52, 0x55, 0x02,
            0x00, 0x53, 0x13, 0x01, 0x00, 0x56, 0x46, 0x04, 0x00, 0x53, 0x11, 0x01, 0x00, 0x16, 0x42, 0x03,
            0x20, 0x27, 0x22, 0x04, 0x00, 0x55, 0x55, 0x06, 0x00, 0x55, 0x25, 0x02, 0x00, 0x55, 0x77, 0x05,
            0x00, 0x55, 0x52, 0x05, 0x00, 0x55, 0x46, 0x03, 0x00, 0x47, 0x12, 0x07, 0x24, 0x12, 0x22, 0x04,
            0x22, 0x02, 0x22, 0x02, 0x21, 0x42, 0x22, 0x01, 0x5A, 0x00, 0x00, 0x00, 0x00, 0x22, 0x55, 0x07 };
    }
}

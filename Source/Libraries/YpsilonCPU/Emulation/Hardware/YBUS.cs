﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Ypsilon.Emulation.Hardware
{
    public class YBUS
    {
        private IDisplayProvider m_DisplayProvider;
        private IInputProvider m_InputProvider;

        private List<ADevice> m_Devices;

        public YCPU CPU
        {
            get;
            private set;
        }

        public YBUS(YCPU cpu)
        {
            CPU = cpu;
            m_Devices = new List<ADevice>();
            
        }

        public void Reset()
        {
            while(m_Devices.Count > 0)
            {
                m_Devices[0].Dispose();
                m_Devices.RemoveAt(0);
            }
        }

        public void Update()
        {
            for (int i = 0; i < m_Devices.Count; i += 1)
                m_Devices[i].Update(m_InputProvider);
        }

        public void Display(List<ITexture> textures)
        {
            for (int i = 0; i < m_Devices.Count; i++)
                m_Devices[i].Display(i, textures, m_DisplayProvider);
        }

        internal void SetProviders(IDisplayProvider display, IInputProvider input)
        {
            m_DisplayProvider = display;
            m_InputProvider = input;
        }

        public ushort DevicesConnected
        {
            get { return (ushort)m_Devices.Count; }
        }

        public ushort[] QueryDevice(ushort deviceIndex)
        {
            if (deviceIndex <= 0 || deviceIndex > m_Devices.Count)
            {
                ushort[] info = new ushort[0x04];
                info[0] = 0x0000;
                info[1] = 0x0000;
                info[2] = 0x0000;
                info[3] = 0x0000;
                return info;
            }
            else
            {
                return m_Devices[deviceIndex - 1].Bus_DeviceQuery();
            }
        }

        public void AddDevice(ADevice device)
        {
            m_Devices.Add(device);
        }

        public ushort SendDeviceMessage(ushort deviceIndex, ushort param_0, ushort param_1)
        {
            if (deviceIndex <= 0 || deviceIndex > m_Devices.Count)
                return ADevice.MSG_NO_DEVICE;

            return m_Devices[deviceIndex - 1].Bus_SendMessage(param_0, param_1);
        }

        private List<ushort> m_DevicesRaisingIRQ = new List<ushort>();

        internal void Device_RaiseIRQ(ADevice device)
        {
            int device_index = (int)m_Devices.IndexOf(device);

            if (device_index == -1)
            {
                // should never occur
                return; 
            }
            else
            {
                if (!m_DevicesRaisingIRQ.Contains((ushort)device_index))
                    m_DevicesRaisingIRQ.Add((ushort)device_index);
            }
        }

        public void AcknowledgeIRQ(ushort deviceIndex)
        {
            deviceIndex -= 1;
            if (m_DevicesRaisingIRQ.Contains(deviceIndex))
            {
                m_Devices[deviceIndex].IRQAcknowledged();
                m_DevicesRaisingIRQ.Remove(deviceIndex);
            }
        }

        public bool IRQ
        {
            get { return m_DevicesRaisingIRQ.Count > 0; }
        }

        public ushort II
        {
            get
            {
                if (m_DevicesRaisingIRQ.Count == 0)
                    return 0xFFFF;
                return (ushort)(m_DevicesRaisingIRQ.Min() + 1);
            }
        }

        /// <summary>
        /// Returns the byte array of random access memory attached to the BUS.
        /// </summary>
        public byte[] GetMemoryReference()
        {
            deviceIndex -= 1;
            if (deviceIndex >= m_Devices.Count)
                return null;
            return m_Devices[deviceIndex].GetMemoryBank((ushort)(bank_index & 0x00FF));
        }

        /// <summary>
        /// Returns  the byte array of memory attached to the device.
        /// </summary>
        public byte[] GetDeviceMemoryReference(ushort deviceIndex)
        {
            if (deviceIndex >= m_Devices.Count)
                return null;
            return m_Devices[deviceIndex].GetMemory();
        }
    }
}

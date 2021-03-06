﻿using System;
using System.Collections.Generic;
using Ypsilon.Core.Input;
using Ypsilon.Core.Windows;
using Ypsilon.Emulation;

namespace Ypsilon.Providers {
    /// <summary>
    /// This is a front end to Ypsilon.Core.Input.InputManager that provides messages to
    /// </summary>
    public class InputProvider : IInputProvider {
        private const ushort AltDown = 0x2000;

        private const ushort CtrlDown = 0x1000;
        private const ushort EventDown = 0x0200;
        private const ushort EventPress = 0x0300;
        // YPSILON STUFF

        private const ushort EventUp = 0x0100;
        private const ushort ShiftDown = 0x4000;
        private const ushort TranslatedKey = 0x8000;
        private readonly List<InputEvent> m_EventsThisFrame;

        // BASE STUFF
        private readonly InputManager m_InputManager;

        public InputProvider(InputManager input) {
            m_InputManager = input;
            m_EventsThisFrame = new List<InputEvent>();
            m_InputManager.Update(0, 0);
        }

        public bool TryGetKeyboardEvent(bool translateASCII, out ushort eventCode) {
            eventCode = 0;
            for (int i = 0; i < m_EventsThisFrame.Count; i++) {
                if (m_EventsThisFrame[i].Handled || !(m_EventsThisFrame[i] is InputEventKeyboard))
                    continue;
                InputEventKeyboard e = (InputEventKeyboard)m_EventsThisFrame[i];
                m_EventsThisFrame.RemoveAt(i);
                bool translated = false;
                ushort bitsKeycode = (byte)e.KeyCode;
                ushort bitsKeyChar = (byte)e.KeyChar;
                if (translateASCII && (bitsKeyChar != 0)) {
                    translated = true;
                    bitsKeycode = bitsKeyChar;
                }
                ushort bitsEvent;
                switch (e.EventType) {
                    case KeyboardEvent.Up:
                        bitsEvent = EventUp;
                        break;
                    case KeyboardEvent.Down:
                        bitsEvent = EventDown;
                        break;
                    case KeyboardEvent.Press:
                        bitsEvent = EventPress;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                eventCode = (ushort)(bitsKeycode | bitsEvent | (e.Shift ? ShiftDown : 0) | (e.Alt ? AltDown : 0) | (e.Control ? CtrlDown : 0) | (translated ? TranslatedKey : 0));
                return true;
            }
            return false;
        }

        public bool HandleKeyboardEvent(KeyboardEvent type, WinKeys key, bool shift, bool alt, bool ctrl) {
            foreach (InputEvent e in m_EventsThisFrame) {
                if (e.Handled || !(e is InputEventKeyboard))
                    continue;
                InputEventKeyboard ek = (InputEventKeyboard)e;
                if (ek.EventType != type || ek.KeyCode != key || ek.Shift != shift || ek.Alt != alt || ek.Control != ctrl)
                    continue;
                e.Handled = true;
                return true;
            }
            return false;
        }

        public void Update(float totalSeconds, float frameSeconds) {
            m_EventsThisFrame.Clear();
            bool shift = m_InputManager.IsShiftDown;
            bool ctrl = m_InputManager.IsCtrlDown;
            bool alt = m_InputManager.IsAltDown;
            foreach (InputEventKeyboard e in m_InputManager.GetKeyboardEvents()) {
                m_EventsThisFrame.Add(e);
            }
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;

namespace SerialTest
{
    public class InputQueue
    {
        private readonly SerialPort _serialPort;
        private readonly ConcurrentQueue<int> _inputs;
        readonly object _mutex = new object();
        private int _messageCount = 0;

        public InputQueue(string serialInterface)
        {
            _inputs = new ConcurrentQueue<int>();
            _serialPort = new SerialPort(serialInterface);
        }

        //Called on Startup
        public void StartListening()
        {
            _serialPort.DataReceived += SerialDataReceived;
            _serialPort.Open();
        }

        private void SerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            for (string input = _serialPort.ReadLine(); input != ""; input = _serialPort.ReadLine())
            {
                input = input.Trim();
                Int16 number;
                if (Int16.TryParse(input, out number))
                {
                    _inputs.Enqueue(number);
                    lock (_mutex)
                    {
                        _messageCount++;
                    }
                }
            }
        }

        //Should be called on Stop
        public void StopListening()
        {
            _serialPort.DataReceived -= SerialDataReceived;
            _serialPort.Close();
        }

        //Called once every Tick
        public void ProcessData(out Dictionary<Enums.ButtonEnum, bool> buttons,
            out Dictionary<Enums.RotaryEnum, int> rotary)
        {
            buttons = new Dictionary<Enums.ButtonEnum, bool>();
            rotary = new Dictionary<Enums.RotaryEnum, int>();
            InitButtons(buttons);
            InitRotary(rotary);


            int currentMessageCount = _messageCount;
            if (currentMessageCount <= 0) return;
            for (int i = 0; i < currentMessageCount; i++)
            {
                int data;
                _inputs.TryDequeue(out data);
                ProcessValue(data, buttons, rotary);
            }

            lock (_mutex)
            {
                _messageCount -= currentMessageCount;
            }
        }

        private void InitRotary(Dictionary<Enums.RotaryEnum, int> rotary)
        {
            rotary.Add(Enums.RotaryEnum.Rotary1, 0);
            rotary.Add(Enums.RotaryEnum.Rotary2, 0);
        }

        private void InitButtons(Dictionary<Enums.ButtonEnum, bool> buttons)
        {
            buttons.Add(Enums.ButtonEnum.Button1, false);
            buttons.Add(Enums.ButtonEnum.Button2, false);
            buttons.Add(Enums.ButtonEnum.Button3, false);
            buttons.Add(Enums.ButtonEnum.Button4, false);
            buttons.Add(Enums.ButtonEnum.Rotary1, false);
            buttons.Add(Enums.ButtonEnum.Rotary2, false);
        }

        public void SendData(string data)
        {
            _serialPort.WriteLine(data);
        }

        private void ProcessValue(int data, Dictionary<Enums.ButtonEnum, bool> buttons,
            Dictionary<Enums.RotaryEnum, int> rotary)
        {
            if ((data & 1) != 0)
            {
                buttons[Enums.ButtonEnum.Button1] = true;
            }

            if ((data & 2) != 0)
            {
                buttons[Enums.ButtonEnum.Button2] = true;
            }

            if ((data & 4) != 0)
            {
                buttons[Enums.ButtonEnum.Button3] = true;
            }

            if ((data & 8) != 0)
            {
                buttons[Enums.ButtonEnum.Button4] = true;
            }

            if ((data & 16) != 0)
            {
                buttons[Enums.ButtonEnum.Rotary1] = true;
            }

            if ((data & 32) != 0)
            {
                rotary[Enums.RotaryEnum.Rotary1] += 1;
            }

            if ((data & 64) != 0)
            {
                rotary[Enums.RotaryEnum.Rotary1] -= 1;
            }

            if ((data & 128) != 0)
            {
                buttons[Enums.ButtonEnum.Rotary2] = true;
            }

            if ((data & 256) != 0)
            {
                rotary[Enums.RotaryEnum.Rotary2] += 1;
            }

            if ((data & 512) != 0)
            {
                rotary[Enums.RotaryEnum.Rotary2] -= 1;
            }
        }
    }
}
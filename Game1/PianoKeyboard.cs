using Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kursach2018
{
  
    public class PianoKeyboard
    {

        //   public List<key> keys;
        public PianoKeyboard()
        {

            devices = new List<DeviceSignal>();
            Device = new DeviceSignal(0);
            devices.Add(Device);

        }
        public List<DeviceSignal> devices;
        /// <summary>
        /// базовый класс кнопка есть сигнал черный? 
        /// </summary>
        public class key
        {
            public bool isblack;
            public int signal;
            public key(bool state, int sign)
            {
                isblack = state;
                signal = sign;
            }
        }

        public DeviceSignal Device;
        public class DeviceSignal

        {

            public void RemoveSignal()
            {
                if (q != 0) q = 0;
            }
            public int q = 0;
            public InputDevice inputDevice;
            public int InstaledDevicesCount = InputDevice.InstalledDevices.Count();
            public DeviceSignal(int id)
            {

                if (InputDevice.InstalledDevices.Count != 0)
                {
                    inputDevice = InputDevice.InstalledDevices[id];
                    inputDevice.Open();
                    inputDevice.StartReceiving(null);
                }



                //inputDevice.Open();
                // inputDevice.StartReceiving(null);


            }

            public int GetSignal()
            {


                inputDevice.NoteOn += new InputDevice.NoteOnHandler(NoteOn);
                void NoteOn(NoteOnMessage msg)
                {
                    q = (int)msg.Pitch;
                }

                return q;
            }  // Note events will be received in another thread
               // Pitch[] pitches = { Pitch.C4, Keys.D2, Keys.W, Keys.D3, Keys.E, Keys.R, Keys.D5, Keys.T, Keys.D6, Keys.Y, Keys.D7, Keys.U, Keys.I, Keys.D9, Keys.O, Keys.D0, Keys.P };


            // Console.ReadKey();  // This thread waits for a keypress

        }
        /// <summary>
        /// сигнал с клавиши идет когда нажал и когда отпустил. решение при поступлении сигнала от клавиши к К
        /// прибавляется её сигнал , если K-сигнал равно 0 значит клавиша нажата если к-сигнал равно сигналу значит клавиша отпущена и к обнуляется
        /// </summary>
        int k = 0;
        /// <summary>
        /// возвраащает false если сигнал с клавиши не совпадает с забитым и true если совпадает
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyDown(int key)
        {
            int n = Device.GetSignal();
            k += n;         
            int key1 = n;
            if (key1 == key && n <= k * 32) return true;
            else
            {
                k = 0;
                return false;
            }


        }

        //public bool KeyUp(int key)




    }
}

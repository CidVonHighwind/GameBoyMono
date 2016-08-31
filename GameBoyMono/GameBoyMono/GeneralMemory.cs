using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyMono
{
    public class GeneralMemory
    {
        public byte[] _generalMemory = new byte[65536];

        public byte this[int index]
        {
            get
            {
                if (index == 0xFF42) { }
                if (0xE000 <= index && index <= 0xFDFF)
                {
                    return _generalMemory[index - 0x2000];
                }
                return _generalMemory[index];
            }
            set
            {
                if (Game1.gbCPU.gameStarted && index < 0x7FFF)
                {
                    Game1.gbCPU.bugFound = true;
                    return;
                }

                _generalMemory[index] = value;

                // shadow memory
                if (0xE000 <= index && index <= 0xFDFF)
                {
                    _generalMemory[index - 0x2000] = value;
                }

                // timer
                if (index == 0xFF04)
                    _generalMemory[index] = 0x00;
                if (index == 0xFF07) { }

                // input
                if (index == 0xFF00)
                { }

                if (index == 0xFF42 && value == 3)
                {
                }
                if (index == 0xFF42) { }
                // ToDo, need to take 160 microseconds
                if (index == 0xFF46)
                {
                    // DMA Transfer
                    for (int i = 0; i <= 0x9F; i++)
                    {
                        _generalMemory[0xFE00 + i] = _generalMemory[(value << 8) + i];
                    }
                }

            }
        }

        /// <summary>
        /// set a value without anyone noticing
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetByte(ushort index, byte value)
        {
            _generalMemory[index] = value;
        }
    }
}

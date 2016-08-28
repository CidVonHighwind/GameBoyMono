using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyMono
{
    public class GeneralMemory
    {
        byte[] _generalMemory = new byte[65536];

        public byte this[int index]
        {
            get { return _generalMemory[index]; }
            set
            {
                _generalMemory[index] = value;

                // ToDo, need to take 160 microseconds
                if (index == 0xFF46)
                {
                    // DMA Transfer
                    for (int i = 0; i <= 0x9F; i++)
                    {
                        _generalMemory[0xFE00 + i] = _generalMemory[value + i];
                    }
                }
                if(Game1.gbCPU.gameStarted && index <= 0x7FFF)
                {
                    Game1.gbCPU.bugFound = true;
                }
            }
        }
    }
}

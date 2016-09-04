using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyMono
{
    public class Cartridge
    {
        public byte[] ROM;
        byte type;
        byte selectedROMBank = 1;
        byte selectedRAMBank;

        bool enableRAM;

        public byte this[int index]
        {
            get
            {
                if (index < 0x4000)
                {
                    return ROM[index];
                }
                // ROM Bank 01-..
                else if (index < 0x8000)
                {
                    return ROM[(selectedROMBank - 1) * 0x4000 + index];
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                // enable RAM
                if(index < 0x2000)
                {
                    enableRAM = (value & 0x0F) == 0x0A;
                }
                // select ROM Bank
                else if(index < 0x4000)
                {
                    selectedROMBank = (byte)(value & 0x1F);

                    if (selectedROMBank == 0x00 || selectedROMBank == 0x20 || selectedROMBank == 0x40 || selectedROMBank == 0x60)
                        selectedROMBank++;
                }
                // RAM Bank Number - or - Upper Bits of ROM Bank Number
                else if (index < 0x6000)
                {

                }
                // ROM/RAM Mode Select
                else if(index < 0x8000)
                {

                }
            }
        }
    }
}

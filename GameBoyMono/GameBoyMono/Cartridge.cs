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
        public byte[] RAM;
        byte type;
        byte selectedROMBank = 1;
        byte selectedRAMBank;

        bool ROMRAMMode;

        bool enableRAM;
        
        public string romName;
        public byte cartridgeType, romSize, ramSize, destinationCode;

        public void Init()
        {
            // load cartrige info
            cartridgeType = ROM[0x0147];
            romSize = ROM[0x0148];
            ramSize = ROM[0x0149];
            destinationCode = ROM[0x014A];

            // 0143 - CGB Flag
            // 80h - Game supports CGB functions, but works on old gameboys also.
            // C0h - Game works on CGB only (physically the same as 80h).

            // load the name
            for (int i = 0x0134; i < 0x0144; i++)
            {
                if (ROM[i] != 0)
                    romName += (char)ROM[i];
                else
                    break;
            }

            if (ramSize == 0)
                RAM = null;
            else if (ramSize == 1)
                RAM = new byte[2048];
            else if (ramSize == 2)
                RAM = new byte[8192];
            else if (ramSize == 3)
                RAM = new byte[8192 * 4];
        }

        public byte this[int index]
        {
            get
            {
                // ROM Bank 00
                if (index < 0x4000)
                {
                    return ROM[index];
                }
                // ROM Bank 01-..
                else if (index < 0x8000)
                {
                    return ROM[(selectedROMBank) * 0x4000 + (index - 0x4000)];
                }
                // External Expansion Working RAM
                else if (0xA000 <= index && index < 0xC000)
                {
                    if (RAM != null && (index - 0xA000) * selectedRAMBank < RAM.Length)
                        return RAM[(index - 0xA000) * selectedRAMBank];
                    else
                        return 0;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                // enable RAM
                if (index < 0x2000)
                {
                    enableRAM = (value & 0x0F) == 0x0A;
                }
                // select ROM Bank
                else if (index < 0x4000)
                {
                    selectedROMBank = (byte)(value & 0x1F);

                    if (selectedROMBank == 0x00 || selectedROMBank == 0x20 || selectedROMBank == 0x40 || selectedROMBank == 0x60)
                        selectedROMBank++;
                }
                // RAM Bank Number - or - Upper Bits of ROM Bank Number
                else if (index < 0x6000)
                {
                    selectedRAMBank = (byte)(value & 0x03);
                }
                // ROM/RAM Mode Select
                else if (index < 0x8000)
                {
                    ROMRAMMode = (value & 0x01) == 0x01;
                }
                // External Expansion Working RAM
                else if (0xA000 <= index && index < 0xC000)
                {
                    if (RAM != null && (index - 0xA000) * selectedRAMBank < RAM.Length)
                        RAM[(index - 0xA000) * selectedRAMBank] = value;
                }
            }
        }
    }
}

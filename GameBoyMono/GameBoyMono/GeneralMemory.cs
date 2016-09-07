using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyMono
{
    public class GeneralMemory
    {
        public byte[] memory = new byte[65536]; // 0x10000

        public byte this[int index]
        {
            get
            {
                // DMG Rom
                if (Game1.gbCPU.romMounted && index < 0x100)
                {
                    return DMGRom.romDump[index];
                }
                else if (Game1.gbCPU.romMounted && index == 0x100)
                {
                    Game1.gbCPU.romMounted = false;
                }

                // Cartridge
                if (index < 0x8000)
                {
                    return Game1.gbCPU.cartridge[index];
                }

                // shadow ram
                if (0xE000 <= index && index <= 0xFDFF)
                    return memory[index - 0x2000];

                // update joypad input
                if (index == 0xFF00)
                {
                    memory[0xFF00] &= 0x30;

                    // right, left, up, down
                    if ((memory[0xFF00] & 0x10) == 0x00)
                    {
                        memory[0xFF00] |= (byte)(
                            (InputHandler.KeyDown(Keys.Right) ? 0x00 : 0x01) | (InputHandler.KeyDown(Keys.Left) ? 0x00 : 0x02) |
                            (InputHandler.KeyDown(Keys.Up) ? 0x00 : 0x04) | (InputHandler.KeyDown(Keys.Down) ? 0x00 : 0x08));
                    }
                    // a, b, select, start
                    if ((memory[0xFF00] & 0x20) == 0x00)
                    {
                        memory[0xFF00] |= (byte)(
                            (InputHandler.KeyDown(Keys.A) ? 0x00 : 0x01) | (InputHandler.KeyDown(Keys.S) ? 0x00 : 0x02) |
                            (InputHandler.KeyDown(Keys.Back) ? 0x00 : 0x04) | (InputHandler.KeyDown(Keys.Enter) ? 0x00 : 0x08));
                    }
                }

                return memory[index];
            }
            set
            {
                // ROM
                if (index < 0x8000)
                {
                    Game1.gbCPU.cartridge[index] = value;
                }
                // 8KB Video RAM (VRAM)
                else if (index < 0xA000)
                {
                    memory[index] = value;

                    if (index < 0x97FF)
                        Game1.gbRenderer.updateTileset = true;
                }
                // External Expanision Working RAM
                else if (index < 0xC000)
                {
                    //memory[index] = value;
                }
                // Unit Working RAM
                else if (index < 0xE000)
                {
                    memory[index] = value;
                }
                // shadow memory
                else if (index < 0xFE00)
                {
                    memory[index - 0x2000] = value;
                }
                // sprite attribute table (OAM)
                else if (index < 0xFEA0)
                {
                    memory[index] = value;
                }
                // NOT USABLE
                else if (index < 0xFF00)
                {

                }
                // I/O Ports
                else if (index < 0xFF80)
                {
                    // reset div
                    if (index == 0xFF04)
                    {
                        memory[0xFF04] = 0;
                    }
                    // TAC
                    else if (index == 0xFF07)
                    {
                        // TIMA = TMA
                        if ((memory[index] & 0x03) != (value & 0x03))
                            memory[index] = memory[0xFF06];

                        memory[index] = (byte)(value & 0x07);
                        return;
                    }
                    // FF41 bit 2: match
                    //  Executing a write instruction for the match flag resets that flag but does not change the mode flag.

                    // LCD
                    else if (index == 0xFF44)
                    {
                        /*
                            When the value of bit 7 of the LCDC register is 1, writing 1 to this again does not change the value of register LY.
                            Writing a value of 0 to bit 7 of the LCDC register when its value is 1 stops the LCD controller,
                            and the value of register LY immediately becomes 0.  (Note:  Values should not be written to the register during screen display.)
                         */
                        memory[0xFF44] = 0;
                    }
                    // ToDo, need to take 160 microseconds
                    else if (index == 0xFF46)
                    {
                        memory[index] = value;

                        // DMA Transfer
                        if (0x80 <= value && value <= 0xDF)
                        {
                            for (int i = 0; i <= 0x9F; i++)
                                memory[0xFE00 + i] = memory[(value << 8) + i];
                        }
                        else { }
                    }
                    else
                    {
                        memory[index] = value;
                    }
                }
                // High RAM (HRAM)
                else if(index < 0xFFFF)
                {
                    memory[index] = value;
                }
                // Interrupt Enable Register
                else if(index <= 0xFFFF)
                {
                    memory[index] = value;
                }
                // Nothing
                else
                {

                }
            }
        }
    }
}

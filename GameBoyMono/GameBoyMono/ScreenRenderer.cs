using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyMono
{
    class ScreenRenderer
    {
        Texture2D sprTileData0, sprTileData1;

        public void Load(ContentManager Content)
        {
            LoadTexture(out sprTileData0, 0);
            LoadTexture(out sprTileData1, 1);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // draw the tiledata
            int scale = 1;
            spriteBatch.Draw(sprTileData0, new Rectangle(0, 264, sprTileData0.Width * scale, sprTileData0.Height * scale), Color.White);
            spriteBatch.Draw(sprTileData1, new Rectangle(sprTileData0.Width * scale + 8, 264, sprTileData1.Width * scale, sprTileData1.Height * scale), Color.White);
            
            int drawPosX = Game1.gbCPU.generalMemory[0xFF43];
            int drawPosY = Game1.gbCPU.generalMemory[0xFF42];

            // FF40 - LCDC - LCD Control(R / W)
            // Bit 7 - LCD Display Enable               (0 = Off, 1 = On)
            // Bit 6 - Window Tile Map Display Select   (0 = 9800 - 9BFF, 1 = 9C00 - 9FFF)
            // Bit 5 - Window Display Enable            (0 = Off, 1 = On)
            // Bit 4 - BG & Window Tile Data Select     (0 = 8800 - 97FF, 1 = 8000 - 8FFF)
            // Bit 3 - BG Tile Map Display Select       (0 = 9800 - 9BFF, 1 = 9C00 - 9FFF)
            // Bit 2 - OBJ(Sprite) Size                 (0 = 8x8, 1 = 8x16)
            // Bit 1 - OBJ(Sprite) Display Enable       (0 = Off, 1 = On)
            // Bit 0 - BG Display(for CGB see below)    (0 = Off, 1 = On)
            byte LCDC = Game1.gbCPU.generalMemory[0xFF40];
            bool LCDC_Bit4 = (LCDC & 0x10) == 0x10; // true=first tile bank, false=second tile bank
            bool LCDC_Bit3 = (LCDC & 0x08) == 0x08; // 0=9800-9BFF, 1=9C00-9FFF
            //LCDC_Bit4 = true;

            int startAddress = LCDC_Bit3 ? 0x9C00 : 0x9800;
            int endAddress = LCDC_Bit3 ? 0x9FFF : 0x9BFF;

            // LCDC Bit 3 - BG Tile Map Display Select     (0=9800-9BFF, 1=9C00-9FFF)
            for (int i = startAddress; i <= endAddress; i++)
            {
                int data = (sbyte)Game1.gbCPU.generalMemory[i] + 128;
                if (LCDC_Bit4)
                    data = Game1.gbCPU.generalMemory[i];

                int posX = (i - startAddress) % 32;
                int posY = (i - startAddress) / 32;
                spriteBatch.Draw(LCDC_Bit4 ? sprTileData0 : sprTileData1, new Rectangle(posX * 8, posY * 8, 8, 8), new Rectangle((data % 16) * 8, (data / 16) * 8, 8, 8), Color.White);
            }

            // Window
            // Window Position
            byte WY = Game1.gbCPU.generalMemory[0xFF4A];
            byte WX = Game1.gbCPU.generalMemory[0xFF4B];
            bool LCDC_Bit6 = (LCDC & 0x40) == 0x40; // Window Tile Map Display Select (0=9800-9BFF, 1=9C00-9FFF)
            //LCDC_Bit6 = true;
            startAddress = LCDC_Bit6 ? 0x9C00 : 0x9800;
            endAddress = LCDC_Bit6 ? 0x9FFF : 0x9BFF;

            for (int i = startAddress; i <= endAddress; i++)
            {
                int data = (sbyte)Game1.gbCPU.generalMemory[i] + 128;
                if (LCDC_Bit4)
                    data = Game1.gbCPU.generalMemory[i];

                int posX = (i - startAddress) % 32;
                int posY = (i - startAddress) / 32;
                spriteBatch.Draw(LCDC_Bit4 ? sprTileData0 : sprTileData1, new Rectangle(WX + posX * 8, WY + posY * 8 - 7, 8, 8), 
                    new Rectangle((data % 16) * 8, (data / 16) * 8, 8, 8), Color.White);
            }


            bool LCDC_Bit2 = (LCDC & 0x04) == 0x04; // obj size (0=8x8, 1=8x16)
            //LCDC_Bit2 = true;

            // draw the sprites
            for (int i = 0xFE00; i < 0xFE9F; i += 4)
            {
                // Byte0-2:
                byte posY = Game1.gbCPU.generalMemory[i];
                byte posX = Game1.gbCPU.generalMemory[i + 1];
                byte tileNumber = Game1.gbCPU.generalMemory[i + 2];
                byte attributes = Game1.gbCPU.generalMemory[i + 3];
                // Byte3:
                // Bit7   OBJ-to-BG Priority (0=OBJ Above BG, 1=OBJ Behind BG color 1-3)
                // (Used for both BG and Window. BG color 0 is always behind OBJ)
                // Bit6   Y flip          (0=Normal, 1=Vertically mirrored)
                // Bit5   X flip          (0=Normal, 1=Horizontally mirrored)
                // Bit4   Palette number  **Non CGB Mode Only** (0=OBP0, 1=OBP1)
                // Bit3   Tile VRAM-Bank  **CGB Mode Only**     (0=Bank 0, 1=Bank 1)
                // Bit2-0 Palette number  **CGB Mode Only**     (OBP0-7)

                SpriteEffects sprEffect = SpriteEffects.None;
                if ((attributes & 0x40) == 0x40)
                    sprEffect = SpriteEffects.FlipVertically;
                if ((attributes & 0x20) == 0x20)
                    sprEffect = SpriteEffects.FlipHorizontally;

                if (LCDC_Bit2)
                    tileNumber = (byte)(tileNumber & 0xFE);

                // draw the tile
                spriteBatch.Draw(sprTileData0, new Rectangle(posX - 8, posY - 16, 8, 8), new Rectangle((tileNumber % 16) * 8, (tileNumber / 16) * 8, 8, 8), 
                    Color.White, 0, Vector2.Zero, sprEffect, 0);
                
                // draw the second part of the sprite if in 8x16 mode
                if (LCDC_Bit2)
                    spriteBatch.Draw(sprTileData0, new Rectangle(posX - 8, posY - 16 + 8, 8, 8), new Rectangle(((tileNumber + 1) % 16) * 8, ((tileNumber + 1) / 16) * 8, 8, 8),
                        Color.White, 0, Vector2.Zero, sprEffect, 0);
            }

            // draw the screenPosition
            spriteBatch.Draw(Game1.sprWhite, new Rectangle(drawPosX, drawPosY, 160, 144), Color.Green * 0.25f);
        }

        void LoadTexture(out Texture2D sprTexture, int bank)
        {
            // bank 0 countains 192 tiles and two background maps
            // bank 1 contains another 192 tiles

            // tile data:   8000-8FFF   (Background/Window
            //              8800-97FF
            // they overlap
            int tilesX = 16;
            int tilesY = 16;

            // max 25x25 sprites
            sprTexture = new Texture2D(Game1.graphics.GraphicsDevice, tilesX * 8, tilesY * 8);

            Color[] colorData = new Color[sprTexture.Width * sprTexture.Height];

            // tile
            for (int iy = 0; iy < tilesY; iy++)
            {
                for (int ix = 0; ix < tilesX; ix++)
                {
                    // 
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            int i = ix + iy * tilesX;
                            int startAddress = bank == 0 ? 0x8000 : 0x8800;

                            // get the color data
                            int data = ((Game1.gbCPU.generalMemory[startAddress + (i * 16) + (y * 2)] >> (7 - x)) & 0x01) |
                                (((Game1.gbCPU.generalMemory[startAddress + (i * 16) + (y * 2) + 1] << 1) >> (7 - x)) & 0x02);

                            // convert the number to a color
                            float color = (3 - data) / 3f;
                            colorData[((y + (iy * 8)) * sprTexture.Width) + (ix * 8) + x] = new Color(color, color, color);
                        }
                    }
                }
            }

            sprTexture.SetData(colorData);
        }
    }
}

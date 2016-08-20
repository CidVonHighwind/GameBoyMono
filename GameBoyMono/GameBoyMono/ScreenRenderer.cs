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

            int b0 = 0x8000;
            int b1 = 0x8FFF;
            int b2 = (b1 - b0 + 1) / 16;

            int b10 = 0x8800;
            int b11 = 0x97FF;
            int b12 = (b11 - b10 + 1) / 16;

            int b3 = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int scale = 1;
            spriteBatch.Draw(sprTileData0, new Rectangle(0, 264, sprTileData0.Width * scale, sprTileData0.Height * scale), Color.White);
            spriteBatch.Draw(sprTileData1, new Rectangle(sprTileData0.Width * scale + 8, 264, sprTileData1.Width * scale, sprTileData1.Height * scale), Color.White);

            // background 0x9BFF - 0x9800
            // 32x32 tiles (8x8)

            int drawPosX = Game1.gbCPU.generalMemory[0xFF43];
            int drawPosY = Game1.gbCPU.generalMemory[0xFF42];

            byte LCDC = Game1.gbCPU.generalMemory[0xFF40];
            bool LCDC_Bit4 = (LCDC & 0x10) == 0x10; // true=first tile bank, false=second tile bank
            //LCDC_Bit4 = true;

            // LCDC Bit 3 - BG Tile Map Display Select     (0=9800-9BFF, 1=9C00-9FFF)
            for (int i = 0x9800; i <= 0x9BFF; i++)
            {
                int data = (sbyte)Game1.gbCPU.generalMemory[i] + 128;
                if (LCDC_Bit4)
                    data = Game1.gbCPU.generalMemory[i];
                
                int posX = (i - 0x9800) % 32;
                int posY = (i - 0x9800) / 32;
                spriteBatch.Draw(LCDC_Bit4 ? sprTileData0 : sprTileData1, new Rectangle(posX * 8, posY * 8, 8, 8), new Rectangle((data % 16) * 8, (data / 16) * 8, 8, 8), Color.White);
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

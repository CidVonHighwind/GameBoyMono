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
        public RenderTarget2D gbRenderTarget;

        public bool debugMode = false;

        Effect gbShader;

        Vector4[] colors = new Vector4[4];

        public void Load(ContentManager Content)
        {
            LoadTexture(ref sprTileData0, 0);
            LoadTexture(ref sprTileData1, 1);

            gbShader = Content.Load<Effect>("gbShader");

            colors[0] = new Vector4(1f, 1f, 1, 1);
            colors[1] = new Vector4(0, 0.66f, 0.66f, 1);
            colors[2] = new Vector4(0, 0.33f, 0.33f, 1);
            colors[3] = new Vector4(0, 0, 0, 1);

            gbShader.Parameters["color1"].SetValue(colors[0]);
            gbShader.Parameters["color2"].SetValue(colors[1]);
            gbShader.Parameters["color3"].SetValue(colors[2]);
            gbShader.Parameters["color4"].SetValue(colors[3]);

            gbRenderTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, 160, 144);
        }

        public void Update()
        {
            LoadTexture(ref sprTileData0, 0);
            LoadTexture(ref sprTileData1, 1);
        }

        /*
            FF47 - BGP - BG Palette Data (R/W) - Non CGB Mode Only
            This register assigns gray shades to the color numbers of the BG and Window tiles.
            
              Bit 7-6 - Shade for Color Number 3
              Bit 5-4 - Shade for Color Number 2
              Bit 3-2 - Shade for Color Number 1
              Bit 1-0 - Shade for Color Number 0
            
            The four possible gray shades are:
            
              0  White
              1  Light gray
              2  Dark gray
              3  Black

            FF48 - OBP0 - Object Palette 0 Data (R/W): for sprite palett 0
            FF49 - OBP1 - Object Palette 1 Data (R/W): for sprite palett 1
        */

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!debugMode)
                Game1.graphics.GraphicsDevice.SetRenderTarget(gbRenderTarget);

            Game1.graphics.GraphicsDevice.Clear(Color.Pink);
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, gbShader);

            // draw the tiledata
            int scale = 1;

            // set background colors
            gbShader.Parameters["color1"].SetValue(colors[(Game1.gbCPU.generalMemory[0xFF47] >> 0) & 0x03]);
            gbShader.Parameters["color2"].SetValue(colors[(Game1.gbCPU.generalMemory[0xFF47] >> 2) & 0x03]);
            gbShader.Parameters["color3"].SetValue(colors[(Game1.gbCPU.generalMemory[0xFF47] >> 4) & 0x03]);
            gbShader.Parameters["color4"].SetValue(colors[(Game1.gbCPU.generalMemory[0xFF47] >> 6) & 0x03]);

            if (debugMode)
            {
                spriteBatch.Draw(sprTileData0, new Rectangle(0, 264, sprTileData0.Width * scale, sprTileData0.Height * scale), Color.White);
                spriteBatch.Draw(sprTileData1, new Rectangle(sprTileData0.Width * scale + 8, 264, sprTileData1.Width * scale, sprTileData1.Height * scale), Color.White);
            }

            // LCD Display Enable
            if ((Game1.gbCPU.generalMemory.memory[0xFF40] & 0x80) == 0x00)
            {
                //spriteBatch.End();
                //return;
            }

            int drawPosX = Game1.gbCPU.generalMemory.memory[0xFF43];
            int drawPosY = Game1.gbCPU.generalMemory.memory[0xFF42];

            // FF40 - LCDC - LCD Control(R / W)
            // Bit 7 - LCD Display Enable               (0 = Off, 1 = On)
            // Bit 6 - Window Tile Map Display Select   (0 = 9800 - 9BFF, 1 = 9C00 - 9FFF)
            // Bit 5 - Window Display Enable            (0 = Off, 1 = On)
            // Bit 4 - BG & Window Tile Data Select     (0 = 8800 - 97FF, 1 = 8000 - 8FFF)
            // Bit 3 - BG Tile Map Display Select       (0 = 9800 - 9BFF, 1 = 9C00 - 9FFF)
            // Bit 2 - OBJ(Sprite) Size                 (0 = 8x8, 1 = 8x16)
            // Bit 1 - OBJ(Sprite) Display Enable       (0 = Off, 1 = On)
            // Bit 0 - BG Display(for CGB see below)    (0 = Off, 1 = On)
            byte LCDC = Game1.gbCPU.generalMemory.memory[0xFF40];
            bool LCDC_Bit5 = (LCDC & 0x20) == 0x20;
            bool LCDC_Bit4 = (LCDC & 0x10) == 0x10; // true=first tile bank, false=second tile bank
            bool LCDC_Bit3 = (LCDC & 0x08) == 0x08; // 0=9800-9BFF, 1=9C00-9FFF

            byte shade = Game1.gbCPU.generalMemory[0xFF47];

            int int1 = (Game1.gbCPU.generalMemory[0xFF47] >> 6) & 0x03;
            int int2 = (Game1.gbCPU.generalMemory[0xFF47] >> 4) & 0x03;
            int int3 = (Game1.gbCPU.generalMemory[0xFF47] >> 2) & 0x03;
            int int4 = (Game1.gbCPU.generalMemory[0xFF47] >> 0) & 0x03;
            
            int startAddress = LCDC_Bit3 ? 0x9C00 : 0x9800;
            int endAddress = LCDC_Bit3 ? 0x9FFF : 0x9BFF;

            // LCDC Bit 3 - BG Tile Map Display Select     (0=9800-9BFF, 1=9C00-9FFF)
            // draw the background
            for (int i = startAddress; i <= endAddress; i++)
            {
                int data = (sbyte)Game1.gbCPU.generalMemory.memory[i] + 128;
                if (LCDC_Bit4)
                    data = Game1.gbCPU.generalMemory.memory[i];

                int posX = ((i - startAddress) % 32) * 8 - drawPosX;
                int posY = ((i - startAddress) / 32) * 8 - drawPosY;

                if (posX + 8 < 0)
                    posX += 256;
                if (posY + 8 < 0)
                    posY += 256;

                spriteBatch.Draw(LCDC_Bit4 ? sprTileData0 : sprTileData1, new Rectangle(posX * scale, posY * scale, 8 * scale, 8 * scale),
                    new Rectangle((data % 16) * 8, (data / 16) * 8, 8, 8), Color.White);
            }

            // Window
            if (LCDC_Bit5)
            {
                // Window Position
                byte WY = Game1.gbCPU.generalMemory.memory[0xFF4A];
                byte WX = Game1.gbCPU.generalMemory.memory[0xFF4B];
                bool LCDC_Bit6 = (LCDC & 0x40) == 0x40; // Window Tile Map Display Select (0=9800-9BFF, 1=9C00-9FFF)
                                                        //LCDC_Bit6 = true;
                startAddress = LCDC_Bit6 ? 0x9C00 : 0x9800;
                endAddress = LCDC_Bit6 ? 0x9FFF : 0x9BFF;

                for (int i = startAddress; i <= endAddress; i++)
                {
                    int data = (sbyte)Game1.gbCPU.generalMemory.memory[i] + 128;
                    if (LCDC_Bit4)
                        data = Game1.gbCPU.generalMemory.memory[i];

                    int posX = (i - startAddress) % 32;
                    int posY = (i - startAddress) / 32;
                    spriteBatch.Draw(LCDC_Bit4 ? sprTileData0 : sprTileData1, new Rectangle(WX + posX * 8, WY + posY * 8 - 7, 8, 8),
                        new Rectangle((data % 16) * 8, (data / 16) * 8, 8, 8), Color.White);
                }
            }

            bool LCDC_Bit2 = (LCDC & 0x04) == 0x04; // obj size (0=8x8, 1=8x16)

            gbShader.Parameters["color1"].SetValue(new Vector4(0, 0, 0, 0));
            // draw the objects
            for (int i = 0xFE00; i < 0xFE9F; i += 4)
            {
                // Byte0-2:
                int posY = (Game1.gbCPU.generalMemory.memory[i] - 16) * scale;
                int posX = (Game1.gbCPU.generalMemory.memory[i + 1] - 8) * scale;
                byte tileNumber = Game1.gbCPU.generalMemory.memory[i + 2];
                byte attributes = Game1.gbCPU.generalMemory.memory[i + 3];
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
                spriteBatch.Draw(sprTileData0, new Rectangle(posX, posY, 8 * scale, 8 * scale), new Rectangle((tileNumber % 16) * 8, (tileNumber / 16) * 8, 8, 8),
                    Color.White, 0, Vector2.Zero, sprEffect, 0);

                // draw the second part of the sprite if in 8x16 mode
                if (LCDC_Bit2)
                    spriteBatch.Draw(sprTileData0, new Rectangle(posX - 8, posY - 16 + 8, 8 * scale, 8 * scale),
                        new Rectangle(((tileNumber + 1) % 16) * 8, ((tileNumber + 1) / 16) * 8, 8, 8), Color.White, 0, Vector2.Zero, sprEffect, 0);
            }

            spriteBatch.End();

            Game1.graphics.GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin();

            // draw the screenPosition
            //spriteBatch.Draw(Game1.sprWhite, new Rectangle(0, 0, 160, 144), Color.Green * 0.25f);

            spriteBatch.End();
        }

        Color[] colorData;
        void LoadTexture(ref Texture2D sprTexture, int bank)
        {
            // bank 0 countains 192 tiles and two background maps
            // bank 1 contains another 192 tiles

            // tile data:   8000-8FFF   (Background/Window
            //              8800-97FF
            // they overlap
            int tilesX = 16;
            int tilesY = 16;

            // max 25x25 sprites
            if (sprTexture == null)
                sprTexture = new Texture2D(Game1.graphics.GraphicsDevice, tilesX * 8, tilesY * 8);

            colorData = new Color[sprTexture.Width * sprTexture.Height];

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
                            int data = ((Game1.gbCPU.generalMemory.memory[startAddress + (i * 16) + (y * 2)] >> (7 - x)) & 0x01) |
                                (((Game1.gbCPU.generalMemory.memory[startAddress + (i * 16) + (y * 2) + 1] << 1) >> (7 - x)) & 0x02);

                            // convert the number to a color
                            float color = (3 - data) / 3f;

                            Color cl = Color.Black;

                            if (data == 3)
                                cl = new Color(0.0f, 1f, 1f);
                            if (data == 2)
                                cl = new Color(0.2f, 1f, 1f);
                            if (data == 1)
                                cl = new Color(0.4f, 1f, 1f);
                            if (data == 0)
                                cl = new Color(0.6f, 1f, 1f);

                            cl = new Color(color, color, color, 1);

                            colorData[((y + (iy * 8)) * sprTexture.Width) + (ix * 8) + x] = cl;
                        }
                    }
                }
            }

            sprTexture.SetData(colorData);
        }
    }
}

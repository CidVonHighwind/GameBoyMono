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
    public class ScreenRenderer
    {
        public Texture2D sprTileData, sprOutput;
        public RenderTarget2D gbRenderTarget;

        Effect gbShader;

        Vector4[] bgColors = new Vector4[4];
        Vector4[] objColors = new Vector4[4];

        public Color[] colorData = new Color[160 * 144];

        public bool debugMode = false;

        public void Load(ContentManager Content)
        {
            LoadTexture(ref sprTileData);

            gbShader = Content.Load<Effect>("gbShader");

            bgColors[0] = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            bgColors[1] = new Vector4(0.0f, 0.6f, 0.6f, 1.0f);
            bgColors[2] = new Vector4(0.0f, 0.3f, 0.3f, 1.0f);
            bgColors[3] = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

            objColors[0] = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            objColors[1] = new Vector4(0.8f, 0.5f, 0.5f, 1.0f);
            objColors[2] = new Vector4(0.5f, 0.2f, 0.2f, 1.0f);
            objColors[3] = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

            gbShader.Parameters["color1"].SetValue(bgColors[0]);
            gbShader.Parameters["color2"].SetValue(bgColors[1]);
            gbShader.Parameters["color3"].SetValue(bgColors[2]);
            gbShader.Parameters["color4"].SetValue(bgColors[3]);

            sprOutput = new Texture2D(Game1.graphics.GraphicsDevice, 160, 144);
            gbRenderTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, 160, 144);
        }

        public void Update()
        {
            LoadTexture(ref sprTileData);
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

        public void Draw()
        {
            if (!debugMode)
                Game1.graphics.GraphicsDevice.SetRenderTarget(gbRenderTarget);

            Game1.graphics.GraphicsDevice.Clear(Color.Pink);
            Game1.spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, gbShader);

            // draw the tiledata
            int scale = 1;

            // set background colors
            gbShader.Parameters["color1"].SetValue(bgColors[(Game1.gbCPU.generalMemory[0xFF47] >> 0) & 0x03]);
            gbShader.Parameters["color2"].SetValue(bgColors[(Game1.gbCPU.generalMemory[0xFF47] >> 2) & 0x03]);
            gbShader.Parameters["color3"].SetValue(bgColors[(Game1.gbCPU.generalMemory[0xFF47] >> 4) & 0x03]);
            gbShader.Parameters["color4"].SetValue(bgColors[(Game1.gbCPU.generalMemory[0xFF47] >> 6) & 0x03]);

            if (debugMode)
                Game1.spriteBatch.Draw(sprTileData, new Rectangle(0, 264, sprTileData.Width * scale, sprTileData.Height * scale), Color.White);

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

            int startAddress = LCDC_Bit3 ? 0x9C00 : 0x9800;

            // LCDC Bit 3 - BG Tile Map Display Select     (0=9800-9BFF, 1=9C00-9FFF)
            // draw the background
            for (int i = startAddress; i < startAddress + 1024; i++)
            {
                int data = (sbyte)Game1.gbCPU.generalMemory.memory[i] + 128 + 0x800;
                if (LCDC_Bit4)
                    data = Game1.gbCPU.generalMemory.memory[i];

                int posX = ((i - startAddress) % 32) * 8 - drawPosX;
                int posY = ((i - startAddress) / 32) * 8 - drawPosY;

                if (posX + 8 < 0)
                    posX += 256;
                if (posY + 8 < 0)
                    posY += 256;

                Game1.spriteBatch.Draw(sprTileData, new Rectangle(posX * scale, posY * scale, 8 * scale, 8 * scale),
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

                for (int i = startAddress; i < startAddress + 1024; i++)
                {
                    int data = (sbyte)Game1.gbCPU.generalMemory.memory[i] + 128 + 0x800;
                    if (LCDC_Bit4)
                        data = Game1.gbCPU.generalMemory.memory[i];

                    int posX = (i - startAddress) % 32;
                    int posY = (i - startAddress) / 32;
                    Game1.spriteBatch.Draw(sprTileData, new Rectangle(WX + posX * 8, WY + posY * 8 - 7, 8, 8),
                        new Rectangle((data % 16) * 8, (data / 16) * 8, 8, 8), Color.White);
                }
            }

            bool LCDC_Bit2 = (LCDC & 0x04) == 0x04; // obj size (0=8x8, 1=8x16)


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
                    sprEffect |= SpriteEffects.FlipVertically;
                if ((attributes & 0x20) == 0x20)
                    sprEffect |= SpriteEffects.FlipHorizontally;

                // set object colors
                gbShader.Parameters["color1"].SetValue(new Vector4(0, 0, 0, 0));
                gbShader.Parameters["color2"].SetValue(objColors[(Game1.gbCPU.generalMemory[0xFF48 + ((attributes >> 4) & 0x01)] >> 2) & 0x03]);
                gbShader.Parameters["color3"].SetValue(objColors[(Game1.gbCPU.generalMemory[0xFF48 + ((attributes >> 4) & 0x01)] >> 4) & 0x03]);
                gbShader.Parameters["color4"].SetValue(objColors[(Game1.gbCPU.generalMemory[0xFF48 + ((attributes >> 4) & 0x01)] >> 6) & 0x03]);

                if (LCDC_Bit2)
                    tileNumber = (byte)(tileNumber & 0xFE);

                // draw the tile
                Game1.spriteBatch.Draw(sprTileData, new Rectangle(posX, posY, 8 * scale, 8 * scale), new Rectangle((tileNumber % 16) * 8, (tileNumber / 16) * 8, 8, 8),
                    Color.White, 0, Vector2.Zero, sprEffect, 0);

                // draw the second part of the sprite if in 8x16 mode
                if (LCDC_Bit2)
                    Game1.spriteBatch.Draw(sprTileData, new Rectangle(posX, posY + 8, 8 * scale, 8 * scale),
                        new Rectangle(((tileNumber + 1) % 16) * 8, ((tileNumber + 1) / 16) * 8, 8, 8), Color.White, 0, Vector2.Zero, sprEffect, 0);
            }

            Game1.spriteBatch.End();

            Game1.graphics.GraphicsDevice.SetRenderTarget(null);

            Game1.spriteBatch.Begin();

            // draw the screenPosition
            //spriteBatch.Draw(Game1.sprWhite, new Rectangle(0, 0, 160, 144), Color.Green * 0.25f);

            Game1.spriteBatch.End();
        }

        public void RenderLine(int scanLine)
        {
            if (0 > scanLine || scanLine > 143)
                return;

            for (int i = 0; i < 160; i++)
            {
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
                bool LCDC_Bit2 = (LCDC & 0x04) == 0x04;

                int startAddress = LCDC_Bit3 ? 0x9C00 : 0x9800;

                int start = Game1.gbCPU.generalMemory.memory[startAddress + (((i + drawPosX) % 256) / 8) + ((((scanLine + drawPosY) % 256) / 8) * 32)];
                if (!LCDC_Bit4)
                    start = (sbyte)start + 256;

                colorData[scanLine * 160 + i] = tilesetData[(start % 16) * 8 + (start / 16) * (16 * 8 * 8) + ((i + drawPosX) % 8) + ((scanLine + drawPosY) % 8) * (16 * 8)];

                // Window
                if (LCDC_Bit5)
                {
                    // Window Position
                    byte WY = Game1.gbCPU.generalMemory.memory[0xFF4A];
                    byte WX = Game1.gbCPU.generalMemory.memory[0xFF4B];
                    bool LCDC_Bit6 = (LCDC & 0x40) == 0x40;

                    if (i < (WX - 7) || scanLine < WY)
                    { }
                    else
                    {
                        // Window Tile Map Display Select (0=9800-9BFF, 1=9C00-9FFF)                                
                        startAddress = LCDC_Bit6 ? 0x9C00 : 0x9800;

                        start = Game1.gbCPU.generalMemory.memory[startAddress + (((i + WX - 7) % 256) / 8) + ((((scanLine + WY) % 256) / 8) * 32)];
                        if (!LCDC_Bit4)
                            start = (sbyte)start + 256;

                        colorData[scanLine * 160 + i] = tilesetData[(start % 16) * 8 + (start / 16) * (16 * 8 * 8) + ((i + WX - 7) % 8) + ((scanLine + WY) % 8) * (16 * 8)];
                    }
                }


                // draw the objects
                for (int j = 0xFE00; j < 0xFE9F; j += 4)
                {
                    // Byte0-2:
                    int posY = (Game1.gbCPU.generalMemory.memory[j] - 16);
                    int posX = (Game1.gbCPU.generalMemory.memory[j + 1] - 8);

                    if (posX > i || i >= posX + 8 || posY > scanLine || scanLine >= posY + (LCDC_Bit2 ? 16 : 8))
                        continue;

                    byte tileNumber = Game1.gbCPU.generalMemory.memory[j + 2];
                    byte attributes = Game1.gbCPU.generalMemory.memory[j + 3];
                    // Byte3:
                    // Bit7   OBJ-to-BG Priority (0=OBJ Above BG, 1=OBJ Behind BG color 1-3)
                    // (Used for both BG and Window. BG color 0 is always behind OBJ)
                    // Bit6   Y flip          (0=Normal, 1=Vertically mirrored)
                    // Bit5   X flip          (0=Normal, 1=Horizontally mirrored)
                    // Bit4   Palette number  **Non CGB Mode Only** (0=OBP0, 1=OBP1)
                    // Bit3   Tile VRAM-Bank  **CGB Mode Only**     (0=Bank 0, 1=Bank 1)
                    // Bit2-0 Palette number  **CGB Mode Only**     (OBP0-7)

                    bool flipX = (attributes & 0x20) == 0x20;
                    bool flipY = (attributes & 0x40) == 0x40;

                    if (LCDC_Bit2)
                        tileNumber = (byte)(tileNumber & 0xFE);
                    if ((scanLine - posY >= 8 && !flipY) || (scanLine - posY < 8 && flipY))
                        tileNumber++;

                    int spriteX = i - posX;
                    int spriteY = (scanLine - posY) % 8;

                    // X flip
                    if (flipX)
                        spriteX = 7 - spriteX;
                    // Y flip
                    if (flipY)
                        spriteY = 7 - spriteY;

                    Color cl = tilesetData[(tileNumber % 16) * 8 + (tileNumber / 16) * (16 * 8 * 8) + spriteX + spriteY * (16 * 8)];

                    if (cl != Color.White)
                        colorData[scanLine * 160 + i] = tilesetData[(tileNumber % 16) * 8 + (tileNumber / 16) * (16 * 8 * 8) + spriteX + spriteY * (16 * 8)];
                }
            }
        }

        Color[] tilesetData;
        void LoadTexture(ref Texture2D sprTexture)
        {
            // bank 0 countains 192 tiles and two background maps
            // bank 1 contains another 192 tiles

            // tile data:   8000-8FFF   (Background/Window
            //              8800-97FF
            // they overlap
            int tilesX = 16;
            int tilesY = 24;

            if (sprTexture == null)
                sprTexture = new Texture2D(Game1.graphics.GraphicsDevice, tilesX * 8, tilesY * 8);

            tilesetData = new Color[sprTexture.Width * sprTexture.Height];

            // tile
            for (int iy = 0; iy < tilesY; iy++)
                for (int ix = 0; ix < tilesX; ix++)
                    for (int y = 0; y < 8; y++)
                        for (int x = 0; x < 8; x++)
                        {
                            int i = ix + iy * tilesX;
                            int startAddress = 0x8000;

                            // get the color data
                            int data = ((Game1.gbCPU.generalMemory.memory[startAddress + (i * 16) + (y * 2)] >> (7 - x)) & 0x01) |
                                (((Game1.gbCPU.generalMemory.memory[startAddress + (i * 16) + (y * 2) + 1] << 1) >> (7 - x)) & 0x02);

                            // convert the number to a color
                            float color = (3 - data) / 3f;

                            Color cl = new Color(color, color, color, 1);

                            tilesetData[((y + (iy * 8)) * sprTexture.Width) + (ix * 8) + x] = cl;
                        }

            sprTexture.SetData(tilesetData);
        }
    }
}

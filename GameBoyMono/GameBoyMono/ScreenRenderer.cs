using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace GameBoyMono
{
    public class ScreenRenderer
    {
        public Texture2D sprTileData, sprBackground, sprObjects;

        public Color[] colorData1 = new Color[160 * 144];
        public Color[] colorData2 = new Color[160 * 144];

        public bool debugMode = false, updateTileset;

        public Point debugFramePos, debugTilePos, debugBackgroundPos, debugWindowPos, debugObjectPos;

        public Texture2D sprReplacementTiles;
        Color[] sprReplacementTilesData;

        SearchTree replaceTileTree = new SearchTree();

        public byte bgp, obj0, obj1;

        // draw the tiledata
        int scale = 2;

        public void Load(ContentManager Content)
        {
            LoadTexture(ref sprTileData);

            LoadTileData("Content/sprites/tileDataAlien");

            sprBackground = new Texture2D(Game1.graphics.GraphicsDevice, 160, 144);
            sprObjects = new Texture2D(Game1.graphics.GraphicsDevice, 160, 144);

            sprReplacementTiles = Content.Load<Texture2D>("sprites/textureDumpAlien");
            sprReplacementTilesData = new Color[sprReplacementTiles.Width * sprReplacementTiles.Height];
            sprReplacementTiles.GetData(sprReplacementTilesData);

            debugFramePos = new Point(5, 5);
            debugBackgroundPos = new Point(160 * scale + 10, 5);
            debugWindowPos = new Point(debugBackgroundPos.X + (256 * scale) + 5, 5);
            debugObjectPos = new Point(debugBackgroundPos.X, (256 * scale) + 10);
            debugTilePos = new Point(5, 144 * scale + 10);
        }

        public void Update()
        {
            if (updateTileset)
            {
                updateTileset = false;
                LoadTexture(ref sprTileData);
            }

            if (InputHandler.KeyPressed(Microsoft.Xna.Framework.Input.Keys.T))
                SaveTileData("textureDump.png");

            scale = debugMode ? 2 : 1;
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
            // draw background + objects
            spriteBatch.Draw(sprBackground, new Rectangle(debugFramePos.X, debugFramePos.Y, sprBackground.Width * scale, sprBackground.Height * scale), Color.White);
            spriteBatch.Draw(sprObjects, new Rectangle(debugFramePos.X, debugFramePos.Y, sprBackground.Width * scale, sprBackground.Height * scale), Color.White);

            spriteBatch.Draw(Game1.sprWhite, new Rectangle(debugTilePos.X, debugTilePos.Y, sprTileData.Width * scale, sprTileData.Height * scale), Color.White);

            if (sprTileData != null)
                spriteBatch.Draw(sprTileData, new Rectangle(debugTilePos.X, debugTilePos.Y, sprTileData.Width * scale, sprTileData.Height * scale), Color.White);

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

            spriteBatch.Draw(Game1.sprWhite, new Rectangle(debugBackgroundPos.X, debugBackgroundPos.Y, 256 * scale, 256 * scale), Color.White);

            // LCDC Bit 3 - BG Tile Map Display Select     (0=9800-9BFF, 1=9C00-9FFF)
            // draw the background
            for (int i = startAddress; i < startAddress + 1024; i++)
            {
                int data = LCDC_Bit4 ? Game1.gbCPU.generalMemory.memory[i] : (sbyte)Game1.gbCPU.generalMemory.memory[i] + 256;

                int posX = ((i - startAddress) % 32) * 8;
                int posY = ((i - startAddress) / 32) * 8;

                if (sprTileData != null)
                    spriteBatch.Draw(sprTileData, new Rectangle(debugBackgroundPos.X + posX * scale, debugBackgroundPos.Y + posY * scale, 8 * scale, 8 * scale),
                        new Rectangle((data % 16) * 8, (data / 16) * 8, 8, 8), Color.White);
            }


            // Window
            spriteBatch.Draw(Game1.sprWhite, new Rectangle(debugWindowPos.X, debugWindowPos.Y, 256 * scale, 256 * scale), Color.White);

            if (LCDC_Bit5)
            {
                // Window Position
                bool LCDC_Bit6 = (LCDC & 0x40) == 0x40; // Window Tile Map Display Select (0=9800-9BFF, 1=9C00-9FFF)
                                                        //LCDC_Bit6 = true;
                startAddress = LCDC_Bit6 ? 0x9C00 : 0x9800;

                for (int i = startAddress; i < startAddress + 1024; i++)
                {
                    int data = (sbyte)Game1.gbCPU.generalMemory.memory[i] + 256;
                    if (LCDC_Bit4)
                        data = Game1.gbCPU.generalMemory.memory[i];

                    int posX = ((i - startAddress) % 32) * 8;
                    int posY = ((i - startAddress) / 32) * 8;

                    spriteBatch.Draw(sprTileData, new Rectangle(debugWindowPos.X + posX * scale, debugWindowPos.Y + posY * scale, 8 * scale, 8 * scale),
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

                posX = debugObjectPos.X + (i - 0xFE00) / 4 * 10 * scale;
                posY = debugObjectPos.Y;

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

                // FF48 - OBP0 - Object Palette 0 Data (R/W)
                // FF49 - OBP1 - Object Palette 1 Data (R/W)

                SpriteEffects sprEffect = SpriteEffects.None;
                if ((attributes & 0x40) == 0x40)
                    sprEffect |= SpriteEffects.FlipVertically;
                if ((attributes & 0x20) == 0x20)
                    sprEffect |= SpriteEffects.FlipHorizontally;

                if (LCDC_Bit2)
                    tileNumber = (byte)(tileNumber & 0xFE);

                // draw the tile
                if (sprTileData != null)
                    spriteBatch.Draw(sprTileData, new Rectangle(posX, posY, 8 * scale, 8 * scale),
                        new Rectangle((tileNumber % 16) * 8, (tileNumber / 16) * 8, 8, 8), Color.White, 0, Vector2.Zero, sprEffect, 0);

                // draw the second part of the sprite if in 8x16 mode
                if (LCDC_Bit2)
                    spriteBatch.Draw(sprTileData, new Rectangle(posX, posY + 8 * scale, 8 * scale, 8 * scale),
                        new Rectangle(((tileNumber + 1) % 16) * 8, ((tileNumber + 1) / 16) * 8, 8, 8), Color.White, 0, Vector2.Zero, sprEffect, 0);
            }
        }

        public void RenderLine(int scanLine)
        {
            if (0 > scanLine || scanLine > 143)
                return;

            int bgPosX = Game1.gbCPU.generalMemory.memory[0xFF43];
            int bgPosY = Game1.gbCPU.generalMemory.memory[0xFF42];

            // FF40 - LCDC - LCD Control(R / W)
            byte LCDC = Game1.gbCPU.generalMemory.memory[0xFF40];

            // Bit 7 - LCD Display Enable               (0 = Off, 1 = On)
            // Bit 6 - Window Tile Map Display Select   (0 = 9800 - 9BFF, 1 = 9C00 - 9FFF)
            bool windowTileMap = (LCDC & 0x40) == 0x40;
            // Bit 5 - Window Display Enable            (0 = Off, 1 = On)
            bool drawWindow = (LCDC & 0x20) == 0x20;
            // Bit 4 - BG & Window Tile Data Select     (0 = 8800 - 97FF, 1 = 8000 - 8FFF)
            bool LCDC_Bit4 = (LCDC & 0x10) == 0x10;
            // Bit 3 - BG Tile Map Display Select       (0 = 9800 - 9BFF, 1 = 9C00 - 9FFF)
            bool LCDC_Bit3 = (LCDC & 0x08) == 0x08;
            // Bit 2 - OBJ(Sprite) Size                 (0 = 8x8, 1 = 8x16)
            bool objSize = (LCDC & 0x04) == 0x04;
            // Bit 1 - OBJ(Sprite) Display Enable       (0 = Off, 1 = On)
            bool drawOBJ = (LCDC & 0x02) == 0x02;
            // Bit 0 - BG Display(for CGB see below)    (0 = Off, 1 = On)
            bool drawBG = (LCDC & 0x01) == 0x01;

            int startAddress = LCDC_Bit3 ? 0x9C00 : 0x9800;

            for (int i = 0; i < 160; i++)
            {
                colorData1[scanLine * 160 + i] = Color.Transparent;
                colorData2[scanLine * 160 + i] = Color.Transparent;

                // draw background
                if (drawBG)
                {
                    int start = Game1.gbCPU.generalMemory.memory[startAddress + (((i + bgPosX) % 256) / 8) + ((((scanLine + bgPosY) % 256) / 8) * 32)];
                    if (!LCDC_Bit4)
                        start = (sbyte)start + 256;

                    colorData1[scanLine * 160 + i] = tilesetData[(start % 16) * 8 + (start / 16) * (16 * 8 * 8) + ((i + bgPosX) % 8) + ((scanLine + bgPosY) % 8) * (16 * 8)];
                }

                // draw window
                if (drawWindow)
                {
                    // Window Position 
                    byte WY = Game1.gbCPU.generalMemory.memory[0xFF4A];
                    byte WX = Game1.gbCPU.generalMemory.memory[0xFF4B];

                    if (i >= (WX - 7) && scanLine >= WY)
                    {
                        // current tile
                        int start = Game1.gbCPU.generalMemory.memory[(windowTileMap ? 0x9C00 : 0x9800) + ((i - WX + 7) / 8) + (((scanLine - WY) / 8) * 32)];
                        if (!LCDC_Bit4)
                            start = (sbyte)start + 256;

                        colorData1[scanLine * 160 + i] = tilesetData[(start % 16) * 8 + (start / 16) * (16 * 8 * 8) + ((i - WX + 7) % 8) + ((scanLine - WY) % 8) * (16 * 8)];
                    }
                }

                if (colorData1[scanLine * 160 + i] == Color.Transparent)
                    colorData1[scanLine * 160 + i] = Color.White;

                // draw the objects
                if (true)
                {
                    for (int j = 0xFE00; j < 0xFE9F; j += 4)
                    {
                        // Byte0-2:
                        int posY = (Game1.gbCPU.generalMemory.memory[j] - 16);
                        int posX = (Game1.gbCPU.generalMemory.memory[j + 1] - 8);

                        if (posX > i || i >= posX + 8 || posY > scanLine || scanLine >= posY + (objSize ? 16 : 8))
                            continue;
                        else
                        {

                        }

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

                        bool inBackground = (attributes & 0x80) == 0x80;
                        bool flipX = (attributes & 0x20) == 0x20;
                        bool flipY = (attributes & 0x40) == 0x40;
                        bool palette = (attributes & 0x10) == 0x10;

                        if (inBackground && colorData1[scanLine * 160 + i] != Color.White)
                            continue;
                        if (objSize)
                            tileNumber = (byte)(tileNumber & 0xFE);
                        if ((scanLine - posY >= 8 && !flipY) || (scanLine - posY < 8 && flipY && objSize))
                            tileNumber++;

                        int spriteX = i - posX;
                        int spriteY = (scanLine - posY) % 8;

                        // X flip
                        if (flipX)
                            spriteX = 7 - spriteX;
                        // Y flip
                        if (flipY)
                            spriteY = 7 - spriteY;

                        byte b = tilesetByteData[(tileNumber % 16) * 8 + (tileNumber / 16) * (16 * 8 * 8) + spriteY * (16 * 8) + spriteX];

                        int colorNumber = (Game1.gbCPU.generalMemory[0xFF48 + (palette ? 1 : 0)] >> ((b * 2))) & 0x03;

                        float shade = (3 - colorNumber) / 3f;
                        Color color = new Color(shade, shade, shade, 1);

                        if (b == 0)
                            color = Color.Transparent;

                        // if sprite is not white (transparent)
                        //if (b > 0)
                        //    colorData2[scanLine * 160 + i] = color;

                        Color pixel = tilesetData[(tileNumber % 16) * 8 + (tileNumber / 16) * (16 * 8 * 8) + spriteY * (16 * 8) + spriteX];

                        if (obj1 != Game1.gbCPU.generalMemory.memory[0xFF49])
                        {
                            int dumpColor = (obj1 >> ((b * 2))) & 0x03;

                            //colorNumber = 3 - colorNumber;
                            //dumpColor = 3 - dumpColor;

                            int change = dumpColor - colorNumber;
                            int addChange = (int)((change / 3d) * 255);

                            pixel = new Color(MathHelper.Clamp(pixel.R + addChange, 0, 255),
                                MathHelper.Clamp(pixel.G + addChange, 0, 255), MathHelper.Clamp(pixel.B + addChange, 0, 255), pixel.A);
                        }

                        //pixel = color;

                        if (pixel != Color.Transparent)
                            colorData2[scanLine * 160 + i] = pixel;
                    }
                }
                else
                {

                }
            }

            // finished new screen
            if (scanLine == 143)
            {
                sprBackground.SetData(colorData1);
                sprObjects.SetData(colorData2);
            }
        }

        Color[] tilesetData;//, originalTilesetData;
        byte[] tilesetByteData;
        void LoadTexture(ref Texture2D sprTexture)
        {
            // bank 0 countains 192 tiles and two background maps
            // bank 1 contains another 192 tiles

            // tile data:   8000-8FFF   Background/Window
            //              8800-97FF
            // they overlap
            int tilesX = 16;
            int tilesY = 24;

            if (sprTexture == null)
                sprTexture = new Texture2D(Game1.graphics.GraphicsDevice, tilesX * 8, tilesY * 8);

            tilesetData = new Color[sprTexture.Width * sprTexture.Height];
            tilesetByteData = new byte[sprTexture.Width * sprTexture.Height];

            int[] tileData = new int[8 * 8];

            // tiles
            for (int iy = 0; iy < tilesY; iy++)
                for (int ix = 0; ix < tilesX; ix++)
                {
                    int currentTile = ix + iy * tilesX;

                    byte[] byteData = new byte[16];

                    // singel tile
                    for (int y = 0; y < 8; y++)
                    {
                        byte byteOne = Game1.gbCPU.generalMemory.memory[0x8000 + (currentTile * 16) + (y * 2)];
                        byte byteTwo = Game1.gbCPU.generalMemory.memory[0x8000 + (currentTile * 16) + (y * 2) + 1];

                        byteData[y * 2 + 0] = byteOne;
                        byteData[y * 2 + 1] = byteTwo;

                        for (int x = 0; x < 8; x++)
                        {
                            // get the color data
                            byte data = (byte)(((byteOne >> (7 - x)) & 0x01) | ((((byteTwo << 1) >> (7 - x)) & 0x02)));

                            tilesetByteData[((y + (iy * 8)) * sprTexture.Width) + (ix * 8) + x] = data;

                            tileData[x + y * 8] = data;

                            // convert the number to a color
                            float color = (3 - data) / 3f;

                            Color cl = data == 0 ? Color.Transparent : new Color(color, color, color, 1);

                            tilesetData[((y + (iy * 8)) * sprTexture.Width) + (ix * 8) + x] = cl;
                            //originalTilesetData[((y + (iy * 8)) * sprTexture.Width) + (ix * 8) + x] = cl;
                        }
                    }

                    // search for a replacement
                    int tileNumber = replaceTileTree.Search(byteData);

                    if (tileNumber >= 0)
                    {
                        // load the replacement textures
                        int tilePosX = tileNumber % tilesX;
                        int tilePosY = tileNumber / tilesX;

                        for (int y = 0; y < 8; y++)
                            for (int x = 0; x < 8; x++)
                            {
                                tilesetData[((y + (iy * 8)) * sprTexture.Width) + (ix * 8) + x] =
                                    sprReplacementTilesData[((y + (tilePosY * 8)) * sprTexture.Width) + (tilePosX * 8) + x];
                            }
                    }
                }

            sprTexture.SetData(tilesetData);
        }

        void SaveTileData(string path)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open("tileData", FileMode.Create)))
            {
                // write palette data
                writer.Write(Game1.gbCPU.generalMemory.memory[0xFF47]);
                writer.Write(Game1.gbCPU.generalMemory.memory[0xFF48]);
                writer.Write(Game1.gbCPU.generalMemory.memory[0xFF49]);

                for (int i = 0; i < 0x9800 - 0x8000; i++)
                {
                    writer.Write(Game1.gbCPU.generalMemory.memory[0x8000 + i]);
                }
            }

            using (Stream writer = File.Create(path))
            {
                sprTileData.SaveAsPng(writer, sprTileData.Width, sprTileData.Height);
            }
        }

        void LoadTileData(string path)
        {
            string strPath = path;
            if (File.Exists(strPath))
                using (BinaryReader writer = new BinaryReader(File.Open(strPath, FileMode.Open)))
                {
                    bgp = writer.ReadByte();
                    obj0 = writer.ReadByte();
                    obj1 = writer.ReadByte();

                    byte[] tile = new byte[16];
                    // load all tiles
                    for (int j = 0; j < 384; j++)
                    {
                        // read one tile
                        for (int i = 0; i < 16; i++)
                            tile[i] = writer.ReadByte();

                        replaceTileTree.AddItem(tile, j);
                    }
                }
        }
    }
}

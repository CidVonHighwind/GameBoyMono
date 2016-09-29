using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace GameBoyMonoSpriteEditor
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D sprWhite;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            sprWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            sprWhite.SetData(new Color[] { Color.White });
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void SaveTileData()
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open("tileData", FileMode.Create)))
            {
            }

            using (Stream writer = File.Create("textureDump.png"))
            {

            }
        }

        void LoadTileData()
        {
            string strPath = "Content/sprites/tileData";
            if (File.Exists(strPath))
                using (BinaryReader writer = new BinaryReader(File.Open(strPath, FileMode.Open)))
                {
                    byte[] tile = new byte[16];
                    // load all tiles
                    for (int j = 0; j < 384; j++)
                    {
                        // read one tile
                        for (int i = 0; i < 16; i++)
                            tile[i] = writer.ReadByte();
                        
                    }
                }
        }
    }
}

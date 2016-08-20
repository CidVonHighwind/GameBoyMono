using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace GameBoyMono
{
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static string[] parameter;

        string romName;
        byte cartridgeType;
        byte romSize;
        byte ramSize;
        byte destinationCode;

        public static GameBoyCPU gbCPU = new GameBoyCPU();
        public static Texture2D sprWhite;

        ScreenRenderer gbRenderer = new ScreenRenderer();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            Components.Add(new InputHandler(this));

            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //LoadRom(parameter[0]);
            LoadRamDump(parameter[0]);

            gbCPU.Start();

            sprWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            sprWhite.SetData(new Color[] { Color.White });

            gbRenderer.Load(Content);
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);

            gbRenderer.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void LoadRom(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader romReader = new BinaryReader(fileStream))
                {
                    // load the first junk into the general memory
                    for (int i = 0; i < 0x03FF; i++)
                    {
                        gbCPU.generalMemory[i] = romReader.ReadByte();

                        // name
                        if (0x0134 <= i && i <= 0x0143 && gbCPU.generalMemory[i] != 0)
                            romName += (char)gbCPU.generalMemory[i];
                        // 0143 - CGB Flag
                        // 80h - Game supports CGB functions, but works on old gameboys also.
                        // C0h - Game works on CGB only (physically the same as 80h).
                    }
                }
            }

            cartridgeType = gbCPU.generalMemory[0x0147];
            romSize = gbCPU.generalMemory[0x0148];
            ramSize = gbCPU.generalMemory[0x0149];
            destinationCode = gbCPU.generalMemory[0x014A];
        }

        void LoadRamDump(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader romReader = new BinaryReader(fileStream))
                {
                    // load the first junk into the general memory
                    int i = 0;
                    while (i < romReader.BaseStream.Length)
                    {
                        gbCPU.generalMemory[i++] = romReader.ReadByte();
                    }
                }
            }
        }
    }
}

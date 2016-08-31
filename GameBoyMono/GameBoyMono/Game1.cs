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

        SpriteFont font0;
        Effect gbShader;

        public static string[] parameter;

        string romName;
        byte cartridgeType;
        byte romSize;
        byte ramSize;
        byte destinationCode;

        public static GameBoyCPU gbCPU = new GameBoyCPU();
        public static Texture2D sprWhite, sprMemory;

        ScreenRenderer gbRenderer = new ScreenRenderer();

        string logString;

        int stepCount;

        float renderScale;
        Rectangle renderRectangle;
        RenderTarget2D shaderRenderTarget;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 1008;
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
            sprMemory = new Texture2D(graphics.GraphicsDevice, 512, 1024);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadRom(parameter[0]);
            //LoadRamDump(parameter[0]);

            gbCPU.Start();

            //gbTimer = new Thread(GameBoyTimer);
            //gbTimer.Start();

            font0 = Content.Load<SpriteFont>("font0");
            gbShader = Content.Load<Effect>("gbShader1");

            sprWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            sprWhite.SetData(new Color[] { Color.White });

            gbRenderer.Load(Content);

            UpdateScale();
        }

        public void UpdateScale()
        {
            float widthScale = graphics.PreferredBackBufferWidth / 160f;
            float heightScale = graphics.PreferredBackBufferHeight / 144f;

            renderScale = MathHelper.Min(widthScale, heightScale);

            int width = (int)(renderScale * 160);
            int height = (int)(renderScale * 144);

            renderRectangle = new Rectangle(graphics.PreferredBackBufferWidth / 2 - width / 2,
                graphics.PreferredBackBufferHeight / 2 - height / 2, width, height);

            shaderRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, 160 * (int)renderScale, 144 * (int)renderScale);
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            // update the cpu
            gbCPU.Update(gameTime);

            //if (InputHandler.KeyPressed(Keys.Space) || (InputHandler.KeyDown(Keys.Space) && stepCount == 5))
            //{
            //    gbCPU.CPUCycle();

            //    UpdateSprMemory();

            //    if (!InputHandler.KeyPressed(Keys.Space))
            //        stepCount = 0;
            //}

            if (InputHandler.KeyDown(Keys.Space))
                stepCount++;
            else
                stepCount = -20;

            // update the renderer
            gbRenderer.Update();

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            gbRenderer.RenderScreen(spriteBatch);
            //gbRenderer.Draw(spriteBatch);


            gbShader.Parameters["spriteWidth"].SetValue(shaderRenderTarget.Width);
            gbShader.Parameters["spriteHeight"].SetValue(shaderRenderTarget.Height);
            gbShader.Parameters["scale"].SetValue((int)renderScale);

            graphics.GraphicsDevice.SetRenderTarget(shaderRenderTarget);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, gbShader);

            spriteBatch.Draw(gbRenderer.gbRenderTarget, new Rectangle(0, 0, shaderRenderTarget.Width, shaderRenderTarget.Height), new Color(142, 150, 114));

            spriteBatch.End();
            graphics.GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);

            spriteBatch.Draw(shaderRenderTarget, renderRectangle, Color.White);


            string strDebugger = "AF: 0x" + string.Format("{0:X}", gbCPU.reg_AF) + "\n" +
                                    "BC: 0x" + string.Format("{0:X}", gbCPU.reg_BC) + "\n" +
                                    "DE: 0x" + string.Format("{0:X}", gbCPU.reg_DE) + "\n" +
                                    "HL: 0x" + string.Format("{0:X}", gbCPU.reg_HL) + "\n" +
                                    "SP: 0x" + string.Format("{0:X}", gbCPU.reg_SP) + "\n" +
                                    "PC: 0x" + string.Format("{0:X}", gbCPU.reg_PC) + "\n\n" +
                                    "LY: " + gbCPU.LY + "\n" +
                                    "LYC: " + gbCPU.LYC + "\n" +
                                    "Stat: " + string.Format("{0:X}", gbCPU.Stat) + "\n";
            // debugger
            //spriteBatch.DrawString(font0, strDebugger, new Vector2(0, 1024 - font0.MeasureString(strDebugger).Y), Color.White);

            //spriteBatch.Draw(sprMemory, new Vector2(300, 0), Color.White);

            //spriteBatch.DrawString(font0, "" + gbCPU.reg_PC, new Vector2(0, 0), Color.Red);

            //for (int y = 0; y < 16; y++)
            //{
            //    for (int x = 0; x < 16; x++)
            //    {
            //        System.Action action = gbCPU.ops[x + y * 16];
            //        if (action != null)
            //            spriteBatch.DrawString(font0, action.Method.Name + "\n" + gbCPU.opLength[x+y*16], 
            //                new Vector2(30 + x * 83 + 41 - (int)(font0.MeasureString(action.Method.Name).X / 2), 20 + y * 40), Color.White);
            //    }
            //}

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void UpdateSprMemory()
        {
            Color[] clMemory = new Color[1024 * 512];
            for (int i = 0; i <= 0xFFFF; i++)
            {
                clMemory[i * 8 + 0] = ((gbCPU.generalMemory._generalMemory[i] & 0x80) == 0x80) ? Color.Black : Color.White;
                clMemory[i * 8 + 1] = ((gbCPU.generalMemory._generalMemory[i] & 0x40) == 0x40) ? Color.Black : Color.White;
                clMemory[i * 8 + 2] = ((gbCPU.generalMemory._generalMemory[i] & 0x20) == 0x20) ? Color.Black : Color.White;
                clMemory[i * 8 + 3] = ((gbCPU.generalMemory._generalMemory[i] & 0x10) == 0x10) ? Color.Black : Color.White;
                clMemory[i * 8 + 4] = ((gbCPU.generalMemory._generalMemory[i] & 0x08) == 0x08) ? Color.Black : Color.White;
                clMemory[i * 8 + 5] = ((gbCPU.generalMemory._generalMemory[i] & 0x04) == 0x04) ? Color.Black : Color.White;
                clMemory[i * 8 + 6] = ((gbCPU.generalMemory._generalMemory[i] & 0x02) == 0x02) ? Color.Black : Color.White;
                clMemory[i * 8 + 7] = ((gbCPU.generalMemory._generalMemory[i] & 0x01) == 0x01) ? Color.Black : Color.White;
            }

            sprMemory.SetData(clMemory);
        }

        void LoadRom(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader romReader = new BinaryReader(fileStream))
                {
                    // load the first junk into the general memory
                    for (int i = 0; i < 0x8000; i++)
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

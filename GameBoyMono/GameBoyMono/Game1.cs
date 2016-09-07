using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace GameBoyMono
{
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;

        public static Texture2D sprWhite, sprMemory;
        SpriteFont font0;
        Effect gbShader;

        public static GameBoyCPU gbCPU = new GameBoyCPU();
        public static ScreenRenderer gbRenderer = new ScreenRenderer();

        RenderTarget2D shaderRenderTarget;
        Rectangle renderRectangle;

        public static string[] parameter;

        string romName;
        float renderScale;
        byte cartridgeType, romSize, ramSize, destinationCode;
        bool debugMode = true;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 1008;// 1024;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = false;
        }
        protected override void Initialize()
        {
            Components.Add(new InputHandler(this));

            base.Initialize();
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font0 = Content.Load<SpriteFont>("font0");
            gbShader = Content.Load<Effect>("gbShader1");

            sprMemory = new Texture2D(graphics.GraphicsDevice, 512, 1024);

            sprWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            sprWhite.SetData(new Color[] { Color.White });

            UpdateScale();

            gbRenderer.Load(Content);

            LoadRom(parameter[0]);

            gbCPU.Start();
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
            gbRenderer.debugMode = debugMode;

            // update the cpu
            gbCPU.Update(gameTime);

            //if (InputHandler.KeyPressed(Keys.Space) || (InputHandler.KeyDown(Keys.Space) && stepCount == 5))
            //{
            //    //gbCPU.CPUCycle();

            //    UpdateSprMemory();

            //    if (!InputHandler.KeyPressed(Keys.Space))
            //        stepCount = 0;
            //}
            //if (InputHandler.KeyDown(Keys.Space))
            //    stepCount++;
            //else
            //    stepCount = -20;

            if (debugMode)
                UpdateSprMemory();

            if (InputHandler.KeyPressed(Keys.F1))
                debugMode = !debugMode;

            // update the renderer
            gbRenderer.Update();

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (!debugMode)
            {
                gbShader.Parameters["spriteWidth"].SetValue(shaderRenderTarget.Width);
                gbShader.Parameters["spriteHeight"].SetValue(shaderRenderTarget.Height);
                gbShader.Parameters["scale"].SetValue((int)renderScale);

                graphics.GraphicsDevice.SetRenderTarget(shaderRenderTarget);
                graphics.GraphicsDevice.Clear(Color.White);
                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, gbShader);

                spriteBatch.Draw(gbRenderer.gbRenderTarget, new Rectangle(0, 0, shaderRenderTarget.Width, shaderRenderTarget.Height), new Color(142, 150, 114));

                spriteBatch.End();
                graphics.GraphicsDevice.SetRenderTarget(null);
            }
            else
            {
                gbRenderer.Draw();
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);

            if (!debugMode)
            {
                // draw the scaled up output
                spriteBatch.Draw(shaderRenderTarget, renderRectangle, Color.White);
            }
            else
            {
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
                spriteBatch.DrawString(font0, strDebugger, new Vector2(0, 1024 - font0.MeasureString(strDebugger).Y), Color.White);

                //spriteBatch.Draw(sprMemory, new Vector2(300, 0), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        Color[] clMemory;
        void UpdateSprMemory()
        {
            clMemory = new Color[1024 * 512];

            Color color1 = Color.Black;
            Color color2 = new Color(50, 110, 160);

            for (int i = 0; i <= 0xFFFF; i++)
            {
                if (i == 0x100)
                    color2 = new Color(70, 130, 180);
                else if (i == 0x150)
                    color2 = new Color(90, 150, 200);
                else if (i == 0x8000)
                    color2 = Color.Green;
                else if (i == 0x9800)
                    color2 = Color.Blue;
                else if (i == 0x9C00)
                    color2 = Color.Green;
                else if (i == 0xA000)
                    color2 = Color.Blue;
                else if (i == 0xC000)
                    color2 = Color.Green;
                else if (i == 0xE000)
                    color2 = Color.IndianRed;
                else if (i == 0xFE00)
                    color2 = Color.Green;
                else if (i == 0xFEA0)
                    color2 = Color.IndianRed;
                else if (i == 0xFF00)
                    color2 = Color.ForestGreen;
                else if (i == 0xFF80)
                    color2 = Color.Blue;
                else if (i == 0xFFFE)
                    color2 = Color.Green;
                else if (i == 0xFFFF)
                    color2 = Color.Blue;

                clMemory[i * 8 + 0] = ((gbCPU.generalMemory.memory[i] & 0x80) == 0x80) ? color1 : color2;
                clMemory[i * 8 + 1] = ((gbCPU.generalMemory.memory[i] & 0x40) == 0x40) ? color1 : color2;
                clMemory[i * 8 + 2] = ((gbCPU.generalMemory.memory[i] & 0x20) == 0x20) ? color1 : color2;
                clMemory[i * 8 + 3] = ((gbCPU.generalMemory.memory[i] & 0x10) == 0x10) ? color1 : color2;
                clMemory[i * 8 + 4] = ((gbCPU.generalMemory.memory[i] & 0x08) == 0x08) ? color1 : color2;
                clMemory[i * 8 + 5] = ((gbCPU.generalMemory.memory[i] & 0x04) == 0x04) ? color1 : color2;
                clMemory[i * 8 + 6] = ((gbCPU.generalMemory.memory[i] & 0x02) == 0x02) ? color1 : color2;
                clMemory[i * 8 + 7] = ((gbCPU.generalMemory.memory[i] & 0x01) == 0x01) ? color1 : color2;
            }

            sprMemory.SetData(clMemory);
        }

        void LoadRom(string path)
        {
            gbCPU.cartridge.ROM = File.ReadAllBytes(path);

            // 0143 - CGB Flag
            // 80h - Game supports CGB functions, but works on old gameboys also.
            // C0h - Game works on CGB only (physically the same as 80h).

            // load the name
            for (int i = 0x0134; i < 0x0144; i++)
            {
                if (gbCPU.generalMemory.memory[i] != 0)
                    romName += (char)gbCPU.generalMemory.memory[i];
                else
                    break;
            }

            cartridgeType = gbCPU.generalMemory.memory[0x0147];
            romSize = gbCPU.generalMemory.memory[0x0148];
            ramSize = gbCPU.generalMemory.memory[0x0149];
            destinationCode = gbCPU.generalMemory.memory[0x014A];
        }

        //void LoadRamDump(string path)
        //{
        //    using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
        //    {
        //        using (BinaryReader romReader = new BinaryReader(fileStream))
        //        {
        //            // load the first junk into the general memory
        //            int i = 0;
        //            while (i < romReader.BaseStream.Length)
        //            {
        //                gbCPU.generalMemory.memory[i++] = romReader.ReadByte();
        //            }
        //        }
        //    }
        //}
    }
}

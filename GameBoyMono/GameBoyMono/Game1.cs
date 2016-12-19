using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System;
using System.Collections.Generic;

namespace GameBoyMono
{
    public class Game1 : Game
    {
        Color backgroundColor = new Color(74, 65, 42);
        Color screenBackgroundColor = new Color(163, 169, 129);

        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;

        public static Texture2D sprWhite, sprMemory, sprBackground;
        public static SpriteFont font0;
        Effect gbShader, gbColorShader;

        public static GameBoyCPU gbCPU = new GameBoyCPU();
        public static ScreenRenderer gbRenderer = new ScreenRenderer();
        public static Sound gbSound = new Sound();

        RenderTarget2D shaderRenderTarget, gbRenderTarget;
        Rectangle renderRectangle;

        Texture2D sprOverlay;
        //Rectangle overlayWindow = new Rectangle(640, 252, 640, 572);
        Rectangle overlayWindow = new Rectangle(648, 260, 624, 556);

        public static string[] parameter;

        float renderScale;
        bool debugMode = true;

        public List<string> romList = new List<string>();

        int windowSizeX, windowSizeY;

        bool romLoaded;

        int buttonHeight = 30;

        Vector4[] bgColors = new Vector4[4], objColors = new Vector4[4];

        TetrisPlayer tetrisPlayer = new TetrisPlayer();

        string[] cartridgeTypeStrings = new string[] {
            "ROM ONLY", "MBC1", "MBC1+RAM", "MBC1+RAM+BATTERY", "ERROR", "MBC2", "MBC2+B", "ERROR", "ROM+RAM", "ROM+RAM+BATTERY",
            "ERROR", "MMM01", "MMM01+RAM", "MMM01+RAM+BATTERY", "ERROR", "MBC3+TIMER+BATTERY", "MBC3", "MBC3+RAM",
            "MBC3+RAM+BATTERY", "MBC3+RAM+BATTERY", "ERROR", "MBC4", "MBC4+RAM", "MBC4+RAM+BATTERY", "ERROR", "MBC5", "MBC5+RAM", "MBC5+RAM+BATTERY",
            "MBC5+RUMBLE", "MBC5+RUMBLE+RAM", "MBC5+RUMBLE+RAM+BATTERY"};

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 160 * 2 + 256 * 4 + 20;
            graphics.PreferredBackBufferHeight = 870;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";

            IsMouseVisible = true;

            Window.AllowUserResizing = true;

            this.IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            Components.Add(new InputHandler(this));

            base.Initialize();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            gbSound.Exit();

            base.OnExiting(sender, args);
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font0 = Content.Load<SpriteFont>("font0");
            gbColorShader = Content.Load<Effect>("shaders/colorShader");
            gbShader = Content.Load<Effect>("shaders/screenShader");

            sprOverlay = Content.Load<Texture2D>("gbc");
            sprBackground = Content.Load<Texture2D>("noise");

            sprMemory = new Texture2D(graphics.GraphicsDevice, 512, 1024);

            sprWhite = new Texture2D(graphics.GraphicsDevice, 1, 1);
            sprWhite.SetData(new Color[] { Color.White });

            gbRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, 160, 144, false, SurfaceFormat.Color, DepthFormat.None);

            gbRenderer.Load(Content);

            LoadRomList(parameter[0]);

            bgColors[0] = new Vector4(0, 0, 0, 0.05f);
            bgColors[1] = new Vector4(0, 0, 0, 0.4f);
            bgColors[2] = new Vector4(0, 0, 0, 0.6f);
            bgColors[3] = new Vector4(0, 0, 0, 0.8f);

            objColors[0] = new Vector4(1.0f, 1.0f, 1.0f, 0.05f);
            objColors[1] = new Vector4(0.8f, 0.5f, 0.5f, 0.3f);
            objColors[2] = new Vector4(0.5f, 0.2f, 0.2f, 0.5f);
            objColors[3] = new Vector4(0.0f, 0.0f, 0.0f, 0.7f);

            SearchTree tree = new SearchTree();
            tree.AddItem(new byte[] { 0xFF, 0xFA, 0x0F, 0xFF }, 12);
            tree.AddItem(new byte[] { 0xFF, 0xCA, 0x0F, 0xFF }, 154);
            tree.AddItem(new byte[] { 0xFF, 0xCA, 0xFF, 0xFF }, 554);

            int search = tree.Search(new byte[] { 0xFF, 0xFA, 0x0F, 0xFF });
        }

        public void UpdateRenderTargets()
        {
            float widthScale = graphics.PreferredBackBufferWidth / 160f;
            float heightScale = graphics.PreferredBackBufferHeight / 144f;

            renderScale = MathHelper.Min(widthScale, heightScale);

            int width = (int)renderScale * 160;
            int height = (int)renderScale * 144;

            renderRectangle = new Rectangle(graphics.PreferredBackBufferWidth / 2 - width / 2,
                graphics.PreferredBackBufferHeight / 2 - height / 2, width, height);

            shaderRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, 160 * (int)renderScale,
                144 * (int)renderScale, false, SurfaceFormat.Color, DepthFormat.None);
        }

        protected override void Update(GameTime gameTime)
        {
            if (windowSizeX != Window.ClientBounds.Width || windowSizeY != Window.ClientBounds.Height)
            {
                windowSizeX = Window.ClientBounds.Width;
                windowSizeY = Window.ClientBounds.Height;

                graphics.PreferredBackBufferWidth = windowSizeX;
                graphics.PreferredBackBufferHeight = windowSizeY;
                graphics.ApplyChanges();

                UpdateRenderTargets();
            }

            gbRenderer.debugMode = debugMode;

            if (!romLoaded)
            {
                if (InputHandler.MouseLeftPressed(new Rectangle(0, 0, 500, buttonHeight * romList.Count)))
                    LoadRom(romList[InputHandler.MousePosition().Y / buttonHeight]);
            }
            else
            {
                gbCPU.keyStateLeft = InputHandler.KeyDown(Keys.Left);
                gbCPU.keyStateRight = InputHandler.KeyDown(Keys.Right);
                gbCPU.keyStateUp = InputHandler.KeyDown(Keys.Up);
                gbCPU.keyStateDown = InputHandler.KeyDown(Keys.Down);

                gbCPU.keyStateA = InputHandler.KeyDown(Keys.A);
                gbCPU.keyStateB = InputHandler.KeyDown(Keys.S);
                gbCPU.keyStateSelect = InputHandler.KeyDown(Keys.Back);
                gbCPU.keyStateStart = InputHandler.KeyDown(Keys.Enter);
                
                // update the tetris player
                tetrisPlayer.Update();

                // update the cpu
                gbCPU.Update(gameTime);
                // update the renderer
                gbRenderer.Update();

                if (InputHandler.KeyPressed(Keys.T))
                    tetrisPlayer.isRunning = !tetrisPlayer.isRunning;

                if (debugMode)
                    UpdateSprMemory();

                if (InputHandler.KeyPressed(Keys.F1))
                    debugMode = !debugMode;
                if (InputHandler.KeyPressed(Keys.Escape))
                    romLoaded = false;
            }

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(backgroundColor);

            if (romLoaded)
            {
                // debug mode
                if (debugMode)
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);

                    // draw debug screen
                    gbRenderer.Draw(spriteBatch);

                    string strDebugger = "AF: 0x" + string.Format("{0:X}", gbCPU.reg_AF) + "\n" +
                        "BC: 0x" + string.Format("{0:X}", gbCPU.reg_BC) + "\n" +
                        "DE: 0x" + string.Format("{0:X}", gbCPU.reg_DE) + "\n" +
                        "HL: 0x" + string.Format("{0:X}", gbCPU.reg_HL) + "\n" +
                        "SP: 0x" + string.Format("{0:X}", gbCPU.reg_SP) + "\n" +
                        "PC: 0x" + string.Format("{0:X}", gbCPU.reg_PC) + "\n\n" +

                        "cartridgeType: " + cartridgeTypeStrings[gbCPU.cartridge.cartridgeType] + "\n" +
                        "ROM Size: " + gbCPU.cartridge.romSize + "\n" +
                        "RAM Size: " + gbCPU.cartridge.ramSize + "\n" +
                        "destination code: " + gbCPU.cartridge.destinationCode;

                    spriteBatch.DrawString(font0, strDebugger, new Vector2(5, graphics.PreferredBackBufferHeight - font0.MeasureString(strDebugger).Y - 5), Color.White);

                    //spriteBatch.Draw(sprMemory, new Vector2(300, 0), Color.White);

                    tetrisPlayer.Draw(spriteBatch);

                    spriteBatch.End();
                }
                else
                {
                    graphics.GraphicsDevice.SetRenderTarget(gbRenderTarget);
                    graphics.GraphicsDevice.Clear(Color.Transparent);

                    spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null, gbColorShader);

                    // draw window + background
                    gbColorShader.Parameters["color1"].SetValue(bgColors[(Game1.gbCPU.generalMemory[0xFF47] >> 0) & 0x03]);
                    gbColorShader.Parameters["color2"].SetValue(bgColors[(Game1.gbCPU.generalMemory[0xFF47] >> 2) & 0x03]);
                    gbColorShader.Parameters["color3"].SetValue(bgColors[(Game1.gbCPU.generalMemory[0xFF47] >> 4) & 0x03]);
                    gbColorShader.Parameters["color4"].SetValue(bgColors[(Game1.gbCPU.generalMemory[0xFF47] >> 6) & 0x03]);

                    spriteBatch.Draw(gbRenderer.sprBackground, Vector2.Zero, Color.White);

                    //// draw objects
                    //gbColorShader.Parameters["color1"].SetValue(objColors[0]);
                    //gbColorShader.Parameters["color2"].SetValue(objColors[1]);
                    //gbColorShader.Parameters["color3"].SetValue(objColors[2]);
                    //gbColorShader.Parameters["color4"].SetValue(objColors[3]);

                    spriteBatch.Draw(gbRenderer.sprObjects, Vector2.Zero, Color.White);

                    spriteBatch.End();

                    gbShader.Parameters["spriteWidth"].SetValue(shaderRenderTarget.Width);
                    gbShader.Parameters["spriteHeight"].SetValue(shaderRenderTarget.Height);
                    gbShader.Parameters["scale"].SetValue((int)renderScale);

                    graphics.GraphicsDevice.SetRenderTarget(shaderRenderTarget);
                    graphics.GraphicsDevice.Clear(Color.Transparent);

                    spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, gbShader);

                    spriteBatch.Draw(gbRenderTarget, new Rectangle(0, 0, shaderRenderTarget.Width, shaderRenderTarget.Height), Color.White);

                    spriteBatch.End();

                    graphics.GraphicsDevice.SetRenderTarget(null);

                    // draw the output
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicWrap, null, null);

                    spriteBatch.Draw(sprBackground, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
                        new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), screenBackgroundColor);

                    // draw the scaled up output
                    spriteBatch.Draw(shaderRenderTarget, renderRectangle, Color.White);

                    float scaleX = renderRectangle.Width / (float)overlayWindow.Width;
                    float scaleY = renderRectangle.Height / (float)overlayWindow.Height;

                    spriteBatch.Draw(sprOverlay, new Rectangle(-(int)(overlayWindow.X * scaleX) + renderRectangle.X,
                        -(int)(overlayWindow.Y * scaleY) + renderRectangle.Y, (int)(sprOverlay.Width * scaleX), (int)(sprOverlay.Height * scaleY)), Color.White);

                    spriteBatch.End();
                }
            }
            // Menü
            else
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);

                if (InputHandler.MouseIntersect(new Rectangle(0, 0, 500, buttonHeight * romList.Count)))
                    spriteBatch.Draw(sprWhite, new Rectangle(0, (InputHandler.MousePosition().Y / buttonHeight) * buttonHeight, 500, buttonHeight), Color.White);

                for (int i = 0; i < romList.Count; i++)
                    spriteBatch.DrawString(font0, Path.GetFileName(romList[i]), new Vector2(0, i * buttonHeight), Color.Black);

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
        
        public void LoadRomList(string path)
        {
            romList.Clear();
            romList.AddRange(Directory.GetFiles(path));

            for (int i = 0; i < romList.Count; i++)
            {
                if (!romList[i].Contains(".gb"))
                {
                    romList.RemoveAt(i);
                    i--;
                }
            }
        }

        void LoadRom(string path)
        {
            gbCPU.cartridge.ROM = File.ReadAllBytes(path);

            gbCPU.cartridge.Init();

            gbCPU.Start();

            romLoaded = true;
        }

        void SaveState(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter writer = new BinaryWriter(fileStream))
                {
                    writer.Write(gbCPU.reg_PC);
                    writer.Write(gbCPU.reg_SP);

                    writer.Write(gbCPU.reg_AF);
                    writer.Write(gbCPU.reg_BC);
                    writer.Write(gbCPU.reg_DE);
                    writer.Write(gbCPU.reg_HL);

                    writer.Write(gbCPU.IME);

                    // cartridge
                    writer.Write(gbCPU.cartridge.cartridgeType);

                    for (int i = 0x8000; i < gbCPU.generalMemory.memory.Length; i++)
                    {
                        writer.Write(gbCPU.generalMemory.memory[i]);
                    }
                }
            }
        }

        void LoadState()
        {

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
    }
}

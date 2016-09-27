using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// InputHandler v.1.0
/// </summary>

namespace GameBoyMono
{
    #region ChatZeichen
    /** Stellt eine Tastaturtaste dar mit maximal 3 Funktionen. Normal, SHIFT, ALT. */
    class ChatZeichen
    {
        /** Zeichen welches bei SHIFT zurück gegeben werden soll. */
        private String zeichenBig;

        /** Zeichen welches bei normale anwendung zurück gegeben werden soll. */
        private String zeichenSmall;

        /** Zeichen welches bei ALT zurück gegeben werden soll. */
        private String zeichenAlt;

        /** XNA Keycode für dieses Zeichen. */
        Keys code;


        /** Konstruktor erstellt eine neue Instanz der Klasse ChatZeichen.
         *
         * @param big Zeichen bei SHIFT.
         * @param small Zeichen bei NOSHIFT und NOALT.
         * @param code XNA Keyscode.
         */
        public ChatZeichen(String big, String small, Keys code)
        {
            this.init(big, small, small, code);
        }

        /** Konstruktor erstellt eine neue Instanz der Klasse ChatZeichen.
         *
         * @param big Zeichen bei SHIFT.
         * @param small Zeichen bei NOSHIFT und NOALT.
         * @param alt Zeicheb bei ALT.
         * @param code XNA Keyscode.
         */
        public ChatZeichen(String big, String small, String alt, Keys code)
        {
            this.init(big, small, alt, code);
        }

        /** Initialisierung der Klasse.
         *
         * @param big Zeichen bei SHIFT.
         * @param small Zeichen bei NOSHIFT und NOALT.
         * @param alt Zeicheb bei ALT.
         * @param code XNA Keyscode.
         */
        public void init(String big, String small, String alt, Keys code)
        {
            this.zeichenBig = big;
            this.zeichenSmall = small;
            this.zeichenAlt = alt;
            this.code = code;
        }

        /** Gibt das Zeichen zurück.
         *
         * @param shiftGedrueckt Wurde die SHIFT Taste gedrueckt?
         * @param altGedrueckt Wurde die ALT Taste gedrueckt?
         * @return Zeichen als String.
         */
        public String gibZeichen(bool shiftGedrueckt, bool altGedrueckt)
        {
            return (altGedrueckt) ? this.zeichenAlt : (shiftGedrueckt) ? this.zeichenBig : this.zeichenSmall;
        }

        /** Gibt den XNA Keycode zurück.
         *
         * @return XNA KeysCode.
         */
        public Keys returnKey()
        {
            return this.code;
        }
    }
    #endregion

    class InputHandler : Microsoft.Xna.Framework.GameComponent
    {
        static KeyboardState keyboardState;
        static KeyboardState lastKeyboardState;
        
        static MouseState mouseState;
        static MouseState lastMouseState;
        
        static GamePadState gamePadState;
        static GamePadState lastGamePadState;
        static float GamePadAccuracy = 0.2f;

        static List<ChatZeichen> alphabet;

        #region Keyboard Field Region + Keyboard Region + KeyboardState
        
        public static bool KeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        public static bool KeyPressed(Keys key)
        {
            return keyboardState.IsKeyDown(key) &&
                lastKeyboardState.IsKeyUp(key);
        }

        public static bool KeyReleased(Keys key)
        {
            return keyboardState.IsKeyUp(key) &&
                lastKeyboardState.IsKeyDown(key);
        }


        public static KeyboardState KeyboardState
        {
            get { return keyboardState; }
        }

        public static KeyboardState LastKeyboardState
        {
            get { return lastKeyboardState; }
        }

        #endregion

        #region GamePad Field Region +  GamePad Region
        
        public static bool GamePadDown(Buttons _button)
        {
            return gamePadState.IsButtonDown(_button);
        }

        public static bool GamePadPressed(Buttons _button)
        {
            return gamePadState.IsButtonDown(_button) &&
                lastGamePadState.IsButtonUp(_button);
        }

        public static bool GamePadReleased(Buttons _button)
        {
            return gamePadState.IsButtonUp(_button) &&
                lastGamePadState.IsButtonDown(_button);
        }


        public static bool GamePadLeftStick(Vector2 _dir)
        {
            return ((_dir.X < 0 && gamePadState.ThumbSticks.Left.X < -GamePadAccuracy) || (_dir.X > 0 && gamePadState.ThumbSticks.Left.X > GamePadAccuracy) ||
                (_dir.Y < 0 && gamePadState.ThumbSticks.Left.Y < -GamePadAccuracy) || (_dir.Y > 0 && gamePadState.ThumbSticks.Left.Y > GamePadAccuracy));
        }
        public static bool LastGamePadLeftStick(Vector2 _dir)
        {
            return ((_dir.X < 0 && lastGamePadState.ThumbSticks.Left.X < -GamePadAccuracy) || (_dir.X > 0 && lastGamePadState.ThumbSticks.Left.X > GamePadAccuracy) ||
                (_dir.Y < 0 && lastGamePadState.ThumbSticks.Left.Y < -GamePadAccuracy) || (_dir.Y > 0 && lastGamePadState.ThumbSticks.Left.Y > GamePadAccuracy));
        }

        #endregion
        
        #region Mouse Field Region + Mouse Property Region
        
        public static MouseState MouseState
        {
            get { return mouseState; }
        }

        public static MouseState LastMousState
        {
            get { return lastMouseState; }
        }

        #endregion

        #region Constructor Region

        public InputHandler(Game game)
            : base(game)
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            InputHandler.alphabet = new List<ChatZeichen>();

            /* Alphabet. */
            InputHandler.alphabet.Add(new ChatZeichen("A", "a", Keys.A));
            InputHandler.alphabet.Add(new ChatZeichen("B", "b", Keys.B));
            InputHandler.alphabet.Add(new ChatZeichen("C", "c", Keys.C));
            InputHandler.alphabet.Add(new ChatZeichen("D", "d", Keys.D));
            InputHandler.alphabet.Add(new ChatZeichen("E", "e", "€", Keys.E));
            InputHandler.alphabet.Add(new ChatZeichen("F", "f", Keys.F));
            InputHandler.alphabet.Add(new ChatZeichen("G", "g", Keys.G));
            InputHandler.alphabet.Add(new ChatZeichen("H", "h", Keys.H));
            InputHandler.alphabet.Add(new ChatZeichen("I", "i", Keys.I));
            InputHandler.alphabet.Add(new ChatZeichen("J", "j", Keys.J));
            InputHandler.alphabet.Add(new ChatZeichen("K", "k", Keys.K));
            InputHandler.alphabet.Add(new ChatZeichen("L", "l", Keys.L));
            InputHandler.alphabet.Add(new ChatZeichen("M", "m", "µ", Keys.M));
            InputHandler.alphabet.Add(new ChatZeichen("N", "n", Keys.N));
            InputHandler.alphabet.Add(new ChatZeichen("O", "o", Keys.O));
            InputHandler.alphabet.Add(new ChatZeichen("P", "p", Keys.P));
            InputHandler.alphabet.Add(new ChatZeichen("Q", "q", "@", Keys.Q));
            InputHandler.alphabet.Add(new ChatZeichen("R", "r", Keys.R));
            InputHandler.alphabet.Add(new ChatZeichen("S", "s", Keys.S));
            InputHandler.alphabet.Add(new ChatZeichen("T", "t", Keys.T));
            InputHandler.alphabet.Add(new ChatZeichen("U", "u", Keys.U));
            InputHandler.alphabet.Add(new ChatZeichen("V", "v", Keys.V));
            InputHandler.alphabet.Add(new ChatZeichen("W", "w", Keys.W));
            InputHandler.alphabet.Add(new ChatZeichen("X", "x", Keys.X));
            InputHandler.alphabet.Add(new ChatZeichen("Y", "y", Keys.Y));
            InputHandler.alphabet.Add(new ChatZeichen("Z", "z", Keys.Z));

            /* Dezimalzahlen. */
            InputHandler.alphabet.Add(new ChatZeichen("!", "1", Keys.D1));
            InputHandler.alphabet.Add(new ChatZeichen("\"", "2", "²", Keys.D2));
            InputHandler.alphabet.Add(new ChatZeichen("§", "3", "³", Keys.D3));
            InputHandler.alphabet.Add(new ChatZeichen("$", "4", Keys.D4));
            InputHandler.alphabet.Add(new ChatZeichen("%", "5", Keys.D5));
            InputHandler.alphabet.Add(new ChatZeichen("&", "6", Keys.D6));
            InputHandler.alphabet.Add(new ChatZeichen("/", "7", "{", Keys.D7));
            InputHandler.alphabet.Add(new ChatZeichen("(", "8", "[", Keys.D8));
            InputHandler.alphabet.Add(new ChatZeichen(")", "9", "]", Keys.D9));
            InputHandler.alphabet.Add(new ChatZeichen("=", "0", "}", Keys.D0));

            /* Sonderelemente. */
            InputHandler.alphabet.Add(new ChatZeichen(" ", " ", Keys.Space));
            //InputHandler.alphabet.Add(new ChatZeichen("Ü", "ü", Keys.OemSemicolon));
            //InputHandler.alphabet.Add(new ChatZeichen("Ö", "ö", Keys.OemTilde));
            //InputHandler.alphabet.Add(new ChatZeichen("Ä", "ä", Keys.OemQuotes));
            InputHandler.alphabet.Add(new ChatZeichen(";", ",", Keys.OemComma));
            InputHandler.alphabet.Add(new ChatZeichen("*", "+", "~", Keys.OemPlus));
            InputHandler.alphabet.Add(new ChatZeichen("'", "#", Keys.OemQuestion));
            InputHandler.alphabet.Add(new ChatZeichen(":", ".", Keys.OemPeriod));
            InputHandler.alphabet.Add(new ChatZeichen("_", "-", Keys.OemMinus));
            InputHandler.alphabet.Add(new ChatZeichen("?", "ß", Keys.OemOpenBrackets));
            InputHandler.alphabet.Add(new ChatZeichen("`", "´", Keys.OemCloseBrackets));

            InputHandler.alphabet.Add(new ChatZeichen("°", "^", Keys.OemPipe));
        }

        #endregion

        #region Methods + Text Eingabe

        public override void Update(GameTime gameTime)
        {
            // don't update if the window is not active
            //if (!Game1.wasActive)
            //    return;

            lastKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            //if the game was not active the last mousestate is unintersting
            //lastMouseState = Game1.wasActive ? mouseState : Mouse.GetState();
            lastMouseState = mouseState;
            mouseState = Mouse.GetState();

            lastGamePadState = gamePadState;
            gamePadState = GamePad.GetState(0);
        }

        /// <summary>
        /// set the last input state to the current state
        /// </summary>
        public static void resetInputState()
        {
            lastKeyboardState = keyboardState;
            lastMouseState = mouseState;
            lastGamePadState = gamePadState;
        }

        #endregion 

        #region Mouse Region

        //scroll
        public static bool MouseWheelUp()
        {
            return mouseState.ScrollWheelValue > lastMouseState.ScrollWheelValue;
        }
        public static bool MouseWheelDown()
        {
            return mouseState.ScrollWheelValue < lastMouseState.ScrollWheelValue;
        }

        //down
        public static bool MouseLeftDown()
        {
            return mouseState.LeftButton == ButtonState.Pressed;
        }
        public static bool MouseLeftDown(Rectangle _rectangle)
        {
            return MouseIntersect(_rectangle) && MouseLeftDown();
        }
        public static bool MouseRightDown()
        {
            return mouseState.RightButton == ButtonState.Pressed;
        }
        public static bool MouseMiddleDown()
        {
            return mouseState.MiddleButton == ButtonState.Pressed;
        }

        //start
        public static bool MouseLeftStart()
        {
            return mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released;
        }
        public static bool MouseRightStart()
        {
            return mouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released;
        }
        public static bool MouseMiddleStart()
        {
            return mouseState.MiddleButton == ButtonState.Pressed && lastMouseState.MiddleButton == ButtonState.Released;
        }

        //released
        public static bool MouseLeftReleased()
        {
            return mouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed;
        }
        public static bool MouseRightReleased()
        {
            return mouseState.RightButton == ButtonState.Released && lastMouseState.RightButton == ButtonState.Pressed;
        }

        //pressed
        public static bool MouseLeftPressed()
        {
            return mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released;
        }
        public static bool MouseLeftPressed(Rectangle _rectangle)
        {
            return _rectangle.Contains(MousePosition()) && MouseLeftPressed();
        }

        public static bool MouseRightPressed()
        {
            return mouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released;
        }
        public static bool MouseRightPressed(Rectangle _rectangle)
        {
            return MouseIntersect(_rectangle) && MouseRightPressed();
        }

        public static bool MouseIntersect(Rectangle _rectangle)
        {
            return _rectangle.Contains(MousePosition());
        }

        public static Point MousePosition()
        {
            return mouseState.Position;
        }
        public static Point LastMousePosition()
        {
            return lastMouseState.Position;
        }

        #endregion

        #region return text + return number

        /// <summary>
        /// returns the pressed keys if they are in the InputHandler.alphabet
        /// only retunrs one key at a time
        /// </summary>
        /// <returns></returns>
        public static string returnZeichen()
        {
            bool shiftDown = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);
            bool altDown = keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt);

            foreach (ChatZeichen zeichen in InputHandler.alphabet)
            {
                if (InputHandler.KeyPressed(zeichen.returnKey()))
                    return zeichen.gibZeichen(shiftDown, altDown);
            }

            return "";
        }

        /// <summary>
        /// returns if a number was pressed
        /// if not returns -1
        /// </summary>
        /// <returns></returns>
        public static int returnNumber()
        {
            if (KeyPressed(Keys.D0) || KeyPressed(Keys.NumPad0))
                return 0;
            if (KeyPressed(Keys.D1) || KeyPressed(Keys.NumPad1))
                return 1;
            if (KeyPressed(Keys.D2) || KeyPressed(Keys.NumPad2))
                return 2;
            if (KeyPressed(Keys.D3) || KeyPressed(Keys.NumPad3))
                return 3;
            if (KeyPressed(Keys.D4) || KeyPressed(Keys.NumPad4))
                return 4;
            if (KeyPressed(Keys.D5) || KeyPressed(Keys.NumPad5))
                return 5;
            if (KeyPressed(Keys.D6) || KeyPressed(Keys.NumPad6))
                return 6;
            if (KeyPressed(Keys.D7) || KeyPressed(Keys.NumPad7))
                return 7;
            if (KeyPressed(Keys.D8) || KeyPressed(Keys.NumPad8))
                return 8;
            if (KeyPressed(Keys.D9) || KeyPressed(Keys.NumPad9))
                return 9;

            return -1;
        }
        #endregion
    }
}

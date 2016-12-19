using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBoyMono
{
    class TetrisPlayer
    {
        class fieldType
        {
            public int[,] field;
            public int rotationState;
        }

        //10 * 18
        int[,] field = new int[10, 18];

        Point[] currentBlock = new Point[4];

        public bool isRunning;
        bool rotateBlock;

        bool tictac;
        int direction = 0;

        int[][][,] blocks = new int[7][][,];
        int[][][,] comparisonBlocks = new int[7][][,];

        int currentBlockType, currentBlockRotationState;


        public TetrisPlayer()
        {
            blocks[0] = new int[2][,];
            blocks[0][0] = new int[,] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 1, 1, 1, 1 }, { 0, 0, 0, 0 } };
            blocks[0][1] = new int[,] { { 0, 0, 1, 0 }, { 0, 0, 1, 0 }, { 0, 0, 1, 0 }, { 0, 0, 1, 0 } };

            blocks[1] = new int[1][,];
            blocks[1][0] = new int[,] { { 1, 1 }, { 1, 1 } };

            blocks[2] = new int[4][,];
            blocks[2][0] = new int[,] { { 0, 0, 0 }, { 1, 1, 1 }, { 0, 0, 1 } };
            blocks[2][1] = new int[,] { { 0, 1, 0 }, { 0, 1, 0 }, { 1, 1, 0 } };
            blocks[2][2] = new int[,] { { 1, 0, 0 }, { 1, 1, 1 }, { 0, 0, 0 } };
            blocks[2][3] = new int[,] { { 0, 1, 1 }, { 0, 1, 0 }, { 0, 1, 0 } };

            blocks[3] = new int[4][,];
            blocks[3][0] = new int[,] { { 0, 0, 0 }, { 1, 1, 1 }, { 1, 0, 0 } };
            blocks[3][1] = new int[,] { { 1, 1, 0 }, { 0, 1, 0 }, { 0, 1, 0 } };
            blocks[3][2] = new int[,] { { 0, 0, 1 }, { 1, 1, 1 }, { 0, 0, 0 } };
            blocks[3][3] = new int[,] { { 0, 1, 0 }, { 0, 1, 0 }, { 0, 1, 1 } };

            blocks[4] = new int[2][,];
            blocks[4][0] = new int[,] { { 0, 0, 0 }, { 0, 1, 1 }, { 1, 1, 0 } };
            blocks[4][1] = new int[,] { { 0, 1, 0 }, { 0, 1, 1 }, { 0, 0, 1 } };

            blocks[5] = new int[4][,];
            blocks[5][0] = new int[,] { { 0, 0, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
            blocks[5][1] = new int[,] { { 0, 1, 0 }, { 1, 1, 0 }, { 0, 1, 0 } };
            blocks[5][2] = new int[,] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 0, 0 } };
            blocks[5][3] = new int[,] { { 0, 1, 0 }, { 0, 1, 1 }, { 0, 1, 0 } };

            blocks[6] = new int[2][,];
            blocks[6][0] = new int[,] { { 0, 0, 0 }, { 1, 1, 0 }, { 0, 1, 1 } };
            blocks[6][1] = new int[,] { { 0, 0, 1 }, { 0, 1, 1 }, { 0, 1, 0 } };


            comparisonBlocks[0] = new int[2][,];
            comparisonBlocks[0][0] = new int[,] { { 1, 1, 1, 1 } };
            comparisonBlocks[0][1] = new int[,] { { 1 }, { 1 }, { 1 }, { 1 } };

            comparisonBlocks[1] = new int[1][,];
            comparisonBlocks[1][0] = new int[,] { { 1, 1 }, { 1, 1 } };

            comparisonBlocks[2] = new int[4][,];
            comparisonBlocks[2][0] = new int[,] { { 1, 1, 1 }, { 0, 0, 1 } };
            comparisonBlocks[2][1] = new int[,] { { 0, 1 }, { 0, 1 }, { 1, 1 } };
            comparisonBlocks[2][2] = new int[,] { { 1, 0, 0 }, { 1, 1, 1 } };
            comparisonBlocks[2][3] = new int[,] { { 1, 1 }, { 1, 0 }, { 1, 0 } };

            comparisonBlocks[3] = new int[4][,];
            comparisonBlocks[3][0] = new int[,] { { 1, 1, 1 }, { 1, 0, 0 } };
            comparisonBlocks[3][1] = new int[,] { { 1, 1 }, { 0, 1 }, { 0, 1 } };
            comparisonBlocks[3][2] = new int[,] { { 0, 0, 1 }, { 1, 1, 1 } };
            comparisonBlocks[3][3] = new int[,] { { 1, 0 }, { 1, 0 }, { 1, 1 } };

            comparisonBlocks[4] = new int[2][,];
            comparisonBlocks[4][0] = new int[,] { { 0, 1, 1 }, { 1, 1, 0 } };
            comparisonBlocks[4][1] = new int[,] { { 1, 0 }, { 1, 1 }, { 0, 1 } };

            comparisonBlocks[5] = new int[4][,];
            comparisonBlocks[5][0] = new int[,] { { 1, 1, 1 }, { 0, 1, 0 } };
            comparisonBlocks[5][1] = new int[,] { { 0, 1 }, { 1, 1 }, { 0, 1 } };
            comparisonBlocks[5][2] = new int[,] { { 0, 1, 0 }, { 1, 1, 1 } };
            comparisonBlocks[5][3] = new int[,] { { 1, 0 }, { 1, 1 }, { 1, 0 } };

            comparisonBlocks[6] = new int[2][,];
            comparisonBlocks[6][0] = new int[,] { { 1, 1, 0 }, { 0, 1, 1 } };
            comparisonBlocks[6][1] = new int[,] { { 0, 1 }, { 1, 1 }, { 1, 0 } };
        }

        public void Update()
        {
            if (isRunning)
            {
                // update the data
                for (int y = 0; y < field.GetLength(1); y++)
                    for (int x = 0; x < field.GetLength(0); x++)
                        field[x, y] = Game1.gbCPU.generalMemory.memory[0x9800 + 2 + x + y * 32] == 47 ? 0 : 1;

                for (int i = 0; i < currentBlock.Length; i++)
                {
                    int currentObject = 0xFE00 + 4 * 4 + (i * 4);

                    int posY = ((Game1.gbCPU.generalMemory.memory[currentObject] - 16) / 8);
                    int posX = ((Game1.gbCPU.generalMemory.memory[currentObject + 1] - 8 - 16) / 8);

                    currentBlock[i] = new Point(posX, posY);
                }

                direction = getDirection(field, (Point[])currentBlock.Clone());

                if (tictac)
                {
                    //if (rotateBlock)
                    //    Game1.gbCPU.keyStateA = true;
                    //else
                    //{
                    //    Game1.gbCPU.keyStateLeft = direction == -1;
                    //    Game1.gbCPU.keyStateRight = direction == 1;
                    //}

                    tictac = false;
                }
                else
                    tictac = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Point drawPosition = new Point(330, 540);

            for (int y = 0; y < field.GetLength(1); y++)
            {
                for (int x = 0; x < field.GetLength(0); x++)
                {
                    if (field[x, y] == 0)
                        spriteBatch.Draw(Game1.sprWhite, new Rectangle(drawPosition.X + x * 16, drawPosition.Y + y * 16, 16, 16), Color.LightGray);
                    else
                        spriteBatch.Draw(Game1.sprWhite, new Rectangle(drawPosition.X + x * 16, drawPosition.Y + y * 16, 16, 16), Color.Red);
                }
            }

            for (int i = 0; i < currentBlock.Length; i++)
            {
                spriteBatch.Draw(Game1.sprWhite, new Rectangle(drawPosition.X + currentBlock[i].X * 16, drawPosition.Y + currentBlock[i].Y * 16, 16, 16), Color.Green * ((i + 1) / 8f));
            }

            for (int y = 0; y < blocks[currentBlockType][currentBlockRotationState].GetLength(1); y++)
            {
                for (int x = 0; x < blocks[currentBlockType][currentBlockRotationState].GetLength(0); x++)
                {
                    if (blocks[currentBlockType][currentBlockRotationState][x, y] == 1)
                        spriteBatch.Draw(Game1.sprWhite, new Rectangle(drawPosition.X + x * 16, drawPosition.Y + y * 16, 16, 16), Color.Pink);
                }
            }

            spriteBatch.DrawString(Game1.font0, direction + "", new Vector2(drawPosition.X, drawPosition.Y), Color.Red);
        }

        public void getBlockType(Point[] _currentBlock)
        {
            int leftPosition = _currentBlock[0].X;
            int rightPosition = _currentBlock[0].X;
            int upPostion = _currentBlock[0].Y;
            int downPostion = _currentBlock[0].Y;

            for (int i = 0; i < _currentBlock.Length; i++)
            {
                if (_currentBlock[i].X < leftPosition)
                    leftPosition = _currentBlock[i].X;
                if (_currentBlock[i].X > rightPosition)
                    rightPosition = _currentBlock[i].X;

                if (_currentBlock[i].Y < upPostion)
                    upPostion = _currentBlock[i].Y;
                if (_currentBlock[i].Y > downPostion)
                    downPostion = _currentBlock[i].Y;

                // not a block
                if (_currentBlock[i].X < 0 || _currentBlock[i].X >= 10 ||
                    _currentBlock[i].Y < 0 || _currentBlock[i].Y >= 18)
                    return;
            }

            int width = rightPosition - leftPosition + 1;
            int height = downPostion - upPostion + 1;

            int size = width > height ? width : height;

            // create a int array to compare to the blocks
            int[,] cBlockArray0 = new int[width, height];

            for (int i = 0; i < _currentBlock.Length; i++)
                cBlockArray0[_currentBlock[i].X - leftPosition, _currentBlock[i].Y - upPostion] = 1;

            // blocks
            for (int i = 0; i < comparisonBlocks.Length; i++)
            {
                // rotation
                for (int j = 0; j < comparisonBlocks[i].Length; j++)
                    if (arrayEquals(comparisonBlocks[i][j], cBlockArray0))
                    {
                        currentBlockType = i;
                        currentBlockRotationState = j;
                    }
            }
        }

        public bool arrayEquals(int[,] array1, int[,] array2)
        {
            if (array1.GetLength(0) != array2.GetLength(0) ||
                array1.GetLength(1) != array2.GetLength(1))
                return false;

            for (int y = 0; y < array1.GetLength(1); y++)
                for (int x = 0; x < array1.GetLength(0); x++)
                    if (array1[x, y] != array2[x, y])
                        return false;

            return true;
        }

        public int getDirection(int[,] _field, Point[] _currentBlock)
        {
            // no block
            for (int i = 0; i < _currentBlock.Length; i++)
            {
                if (_currentBlock[i].X < 0 || _currentBlock[i].X >= _field.GetLength(0) ||
                    _currentBlock[i].Y < 0 || _currentBlock[i].Y >= _field.GetLength(1))
                    return 0;
            }

            // get block type
            getBlockType((Point[])_currentBlock.Clone());

            List<fieldType> fields = new List<fieldType>();
            List<int> heights = new List<int>();

            int rotationState = currentBlockRotationState;

            // get the position
            int blockPosition = moveLeft(_currentBlock);

            // for every possible rotation
            for (int j = 0; j < blocks[currentBlockType].Length; j++)
            {
                if (j != 0)
                {
                    int state = 0;
                    // rotate the block
                    // this is soooooo stupit...
                    for (int y = 0; y < blocks[currentBlockType][rotationState].GetLength(1); y++)
                        for (int x = 0; x < blocks[currentBlockType][rotationState].GetLength(0); x++)
                            if (blocks[currentBlockType][rotationState][x, y] == 1)
                            {
                                _currentBlock[state] = new Point(x, y);
                                state++;
                            }

                }

                int blockWidth = getBlockWidth(_currentBlock);
                // number of fields
                int fieldCount = _field.GetLength(0) - blockWidth + 1;

                // set the block to the left
                moveLeft(_currentBlock);

                for (int i = 0; i < fieldCount; i++)
                {
                    fields.Add(new fieldType() { field = getField((int[,])field.Clone(), (Point[])_currentBlock.Clone()), rotationState = rotationState });
                    heights.Add(getHeight(fields[fields.Count - 1].field));

                    // move block right by one
                    for (int n = 0; n < _currentBlock.Length; n++)
                        _currentBlock[n].X++;
                }

                rotationState = (rotationState + 1) % blocks[currentBlockType].Length;
            }

            int minPosition = heights.IndexOf(heights.Min());

            if (currentBlockRotationState != fields[minPosition].rotationState)
                rotateBlock = true;
            else
                rotateBlock = false;

            // go right
            if (minPosition > blockPosition)
                return 1;
            // go left
            if (minPosition < blockPosition)
                return -1;

            return 0;
        }

        public int moveLeft(Point[] _currentBlock)
        {
            bool moving = true;
            int steps = 0;
            while (moving)
            {
                for (int i = 0; i < _currentBlock.Length; i++)
                {
                    if (_currentBlock[i].X == 0)
                        moving = false;
                }

                if (moving)
                {
                    for (int i = 0; i < _currentBlock.Length; i++)
                        _currentBlock[i].X--;

                    steps++;
                }
            }

            return steps;
        }

        /// <summary>
        /// get the widht of a block
        /// </summary>
        /// <param name="_currentBlock"></param>
        /// <returns></returns>
        public int getBlockWidth(Point[] _currentBlock)
        {
            int leftPosition = _currentBlock[0].X;
            int rightPosition = _currentBlock[0].X;

            for (int i = 1; i < _currentBlock.Length; i++)
            {
                if (_currentBlock[i].X < leftPosition)
                    leftPosition = _currentBlock[i].X;
                if (_currentBlock[i].X > rightPosition)
                    rightPosition = _currentBlock[i].X;
            }

            return rightPosition - leftPosition + 1;
        }

        /// <summary>
        /// let the currentBlock fall down and return the field
        /// </summary>
        /// <param name="_field"></param>
        /// <param name="_currentBlock"></param>
        /// <returns></returns>
        public int[,] getField(int[,] _field, Point[] _currentBlock)
        {
            bool isRunning = true;

            while (isRunning)
            {
                for (int i = 0; i < _currentBlock.Length; i++)
                {
                    if (_currentBlock[i].Y == _field.GetLength(1) - 1 || _field[_currentBlock[i].X, _currentBlock[i].Y + 1] == 1)
                        isRunning = false;
                }

                if (isRunning)
                {
                    for (int i = 0; i < _currentBlock.Length; i++)
                        _currentBlock[i].Y++;
                }
                else
                {
                    for (int i = 0; i < _currentBlock.Length; i++)
                        _field[_currentBlock[i].X, _currentBlock[i].Y] = 1;
                }
            }

            return _field;
        }

        /// <summary>
        /// get the height of an field
        /// </summary>
        /// <param name="_field"></param>
        /// <returns></returns>
        public int getHeight(int[,] _field)
        {
            bool foundStone;
            for (int y = _field.GetLength(1) - 1; y >= 0; y--)
            {
                foundStone = false;
                for (int x = 0; x < _field.GetLength(0); x++)
                {
                    if (_field[x, y] == 1)
                    {
                        foundStone = true;
                        break;
                    }
                }
                if (!foundStone)
                    return _field.GetLength(1) - y - 1;
            }

            return _field.GetLength(1);
        }

        public int rateField(int[,] _field)
        {
            int rating = 0;



            return rating;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;
#pragma warning disable IDE1006
namespace testexetrisathlon
{
    public class Tetrominoe
    {
        public static int[,] I = new int[1, 4] { { 1, 1, 1, 1 } };

        public static int[,] O = new int[2, 2] { { 1, 1 },
                                                 { 1, 1 } };

        public static int[,] T = new int[2, 3] { { 0, 1, 0 },
                                                 { 1, 1, 1 } };

        public static int[,] S = new int[2, 3] { { 0, 1, 1 },
                                                 { 1, 1, 0 } };

        public static int[,] Z = new int[2, 3] { { 1, 1, 0 },
                                                 { 0, 1, 1 } };

        public static int[,] J = new int[2, 3] { { 1, 0, 0 },
                                                 { 1, 1, 1 } };

        public static int[,] L = new int[2, 3] { { 0, 0, 1 },
                                                 { 1, 1, 1 } };
        public static List<int[,]> tetrominoes = new List<int[,]>() { I, O, T, S, Z, J, L };
        private readonly int[,] shape;
        public List<int[]> location = new List<int[]>();
        public Tetrominoe()
        {
            shape = tetrominoes[Program.rnd.Next(0, tetrominoes.Count)];
            for (int i = 23; i < 33; ++i)
            {
                for (int j = 3; j < 10; j++)
                {
                    SetCursorPosition(i, j);
                    Write(" ");
                }
            }
            Program.DrawBorder();
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] == 1)
                    {
                        SetCursorPosition(((10 - shape.GetLength(1)) / 2 + j) * 2 + 20, i + 5);
                        Write(Program.sqr);
                    }
                }
            }
        }
        public void Spawn()
        {
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] == 1)
                    {
                        location.Add(new int[] { i, (10 - shape.GetLength(1)) / 2 + j });
                    }
                }
            }
            Update();
        }
        public void Drop()
        {
            if (isSomethingBelow())
            {
                for (int i = 0; i < 4; i++)
                {
                    Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] = 1;
                }
                Program.isDropped = true;
                Beep(800, 200);
            }
            else
            {
                for (int numCount = 0; numCount < 4; numCount++)
                {
                    location[numCount][0] += 1;
                }
                Update();
            }
        }
        public void Rotate()
        {
            List<int[]> templocation = new List<int[]>();
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] == 1)
                    {
                        templocation.Add(new int[] { i, (10 - shape.GetLength(1)) / 2 + j });
                    }
                }
            }
            if (shape == tetrominoes[0])
            {
                for (int i = 0; i < location.Count; i++)
                {
                    templocation[i] = TransformMatrix(location[i], location[2]);
                }
            }
            else if (shape == tetrominoes[3])
            {
                for (int i = 0; i < location.Count; i++)
                {
                    templocation[i] = TransformMatrix(location[i], location[3]);
                }
            }
            else if (shape == tetrominoes[1])
                return;
            else
            {
                for (int i = 0; i < location.Count; i++)
                {
                    templocation[i] = TransformMatrix(location[i], location[2]);
                }
            }
            for (int count = 0; isOverlayLeft(templocation) != false | isOverlayRight(templocation) != false | isOverlayBelow(templocation) != false; count++)
            {
                if (isOverlayLeft(templocation) == true)
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i][1] += 1;
                    }
                }
                if (isOverlayRight(templocation) == true)
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i][1] -= 1;
                    }
                }
                if (isOverlayBelow(templocation) == true)
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i][0] -= 1;
                    }
                }
                if (count == 3)
                {
                    return;
                }
            }
            location = templocation;
        }
        public bool notFalse(bool? inp) => (inp ?? true);
        public int[] TransformMatrix(int[] coord, int[] axis) => new int[] { axis[0] - axis[1] + coord[1], axis[0] + axis[1] - coord[0] };
        public bool isSomethingBelow()
        {
            for (int i = 0; i < 4; i++)
            {
                if (location[i][0] + 1 >= 23)
                    return true;
                if (location[i][0] + 1 < 23 & Program.droppedtetrominoeLocationGrid[location[i][0] + 1, location[i][1]] == 1)
                    return true;
            }
            return false;
        }
        public bool? isOverlayBelow(List<int[]> location)
        {
            List<int> ycoords = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                ycoords.Add(location[i][0]);
                if (location[i][0] >= 23)
                    return true;
                if (location[i][0] < 0 | location[i][1] < 0 | location[i][1] > 9)
                    return null;
            }
            for (int i = 0; i < 4; i++)
            {
                if (ycoords.Max() - ycoords.Min() == 3)
                {
                    if ((ycoords.Max() == location[i][0] | ycoords.Max() - 1 == location[i][0]) & (Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1))
                    {
                        return true;
                    }
                }
                else
                {
                    if ((ycoords.Max() == location[i][0]) & (Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool isSomethingLeft()
        {
            for (int i = 0; i < 4; i++)
            {
                if (location[i][1] == 0)
                    return true;
                else if (Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1] - 1] == 1)
                    return true;
            }
            return false;
        }
        public bool? isOverlayLeft(List<int[]> location)
        {
            List<int> xcoords = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                xcoords.Add(location[i][1]);
                if (location[i][1] < 0)
                    return true;
                if (location[i][1] > 9)
                    return false;
                if (location[i][0] >= 23 | location[i][0] < 0)
                    return null;
            }
            for (int i = 0; i < 4; i++)
            {
                if (xcoords.Max() - xcoords.Min() == 3)
                {
                    if (xcoords.Min() == location[i][1] | xcoords.Min() + 1 == location[i][1])
                    {
                        if (Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (xcoords.Min() == location[i][1])
                    {
                        if (Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public bool isSomethingRight()
        {
            for (int i = 0; i < 4; i++)
            {
                if (location[i][1] == 9)
                    return true;
                else if (Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1] + 1] == 1)
                    return true;
            }
            return false;
        }
        public bool? isOverlayRight(List<int[]> location)
        {
            List<int> xcoords = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                xcoords.Add(location[i][1]);
                if (location[i][1] > 9)
                    return true;
                if (location[i][1] < 0)
                    return false;
                if (location[i][0] >= 23 | location[i][0] < 0)
                    return null;
            }
            for (int i = 0; i < 4; i++)
            {
                if (xcoords.Max() - xcoords.Min() == 3)
                {
                    if ((xcoords.Max() == location[i][1] | xcoords.Max() - 1 == location[i][1]) & Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                    {
                        return true;
                    }
                }
                else
                {
                    if (xcoords.Max() == location[i][1] & Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void Update()
        {
            for (int i = 0; i < 23; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Program.grid[i, j] = 0;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                Program.grid[location[i][0], location[i][1]] = 1;
            }
            Program.Draw();
        }
    }
}

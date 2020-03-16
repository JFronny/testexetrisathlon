using System.Collections.Generic;
using System.Linq;
using testexetrisathlon.SoundManagement;
using static System.Console;

namespace testexetrisathlon
{
    public class Tetrominoe
    {
        private static readonly int[,] I = {{1, 1, 1, 1}};

        private static readonly int[,] O =
        {
            {1, 1},
            {1, 1}
        };

        private static readonly int[,] T =
        {
            {0, 1, 0},
            {1, 1, 1}
        };

        private static readonly int[,] S =
        {
            {0, 1, 1},
            {1, 1, 0}
        };

        private static readonly int[,] Z =
        {
            {1, 1, 0},
            {0, 1, 1}
        };

        private static readonly int[,] J =
        {
            {1, 0, 0},
            {1, 1, 1}
        };

        private static readonly int[,] L =
        {
            {0, 0, 1},
            {1, 1, 1}
        };

        private static readonly List<int[,]> TetrominoeList = new List<int[,]> {I, O, T, S, Z, J, L};
        private readonly int[,] _shape;
        public List<int[]> Location = new List<int[]>();

        public Tetrominoe()
        {
            _shape = TetrominoeList[Program.Rnd.Next(0, TetrominoeList.Count)];
            for (int i = 0; i < 7; i++)
            for (int j = 0; j < 2; j++)
            {
                SetCursorPosition(i + 26, j + 6);
                Write(Program.Debug ? "X" : " ");
            }
            Program.DrawBorder();
            for (int i = 0; i < _shape.GetLength(0); i++)
            for (int j = 0; j < _shape.GetLength(1); j++)
                if (_shape[i, j] == 1)
                {
                    SetCursorPosition((30 - _shape.GetLength(1)) + (2 * j), i + 6);
                    Write(Program.Sqr);
                }
        }

        public bool IsSomethingBelow => Location.Any(s => s[0] + 1 >= 23 || (s[0] + 1 < 23) &
            (Program.DroppedTetrominoeLocationGrid[s[0] + 1, s[1]] == 1));

        public void Spawn()
        {
            for (int i = 0; i < _shape.GetLength(0); i++)
            for (int j = 0; j < _shape.GetLength(1); j++)
                if (_shape[i, j] == 1)
                    Location.Add(new[] {i, (5 - (_shape.GetLength(1) / 2)) + j});
            Update();
        }

        public void Drop()
        {
            if (IsSomethingBelow)
            {
                Location.ForEach(s => Program.DroppedTetrominoeLocationGrid[s[0], s[1]] = 1);
                Program.IsDropped = true;
                Beeper.Beep(800, 200);
            }
            else
            {
                Location.ForEach(s => s[0]++);
                Update();
            }
        }

        public void Rotate()
        {
            List<int[]> tempLocation = new List<int[]>();
            for (int i = 0; i < _shape.GetLength(0); i++)
            for (int j = 0; j < _shape.GetLength(1); j++)
                if (_shape[i, j] == 1)
                    tempLocation.Add(new[] {i, ((10 - _shape.GetLength(1)) / 2) + j});
            if (_shape == TetrominoeList[1])
                return;
            for (int i = 0; i < Location.Count; i++)
                tempLocation[i] = TransformMatrix(Location[i], Location[_shape == TetrominoeList[3] ? 3 : 2]);
            for (int count = 0;
                (IsOverlayLeft(tempLocation) != false) | (IsOverlayRight(tempLocation) != false) |
                (IsOverlayBelow(tempLocation) != false);
                count++)
            {
                if (IsOverlayLeft(tempLocation) == true)
                    for (int i = 0; i < Location.Count; i++)
                        tempLocation[i][1]++;
                if (IsOverlayRight(tempLocation) == true)
                    for (int i = 0; i < Location.Count; i++)
                        tempLocation[i][1]--;
                if (IsOverlayBelow(tempLocation) == true)
                    for (int i = 0; i < Location.Count; i++)
                        tempLocation[i][0]--;
                if (count == 3)
                    return;
            }
            Location = tempLocation;
        }

        private static int[] TransformMatrix(IReadOnlyList<int> coords, IReadOnlyList<int> axis) =>
            new[] {(axis[0] - axis[1]) + coords[1], (axis[0] + axis[1]) - coords[0]};

        private static bool? IsOverlayBelow(IReadOnlyList<int[]> location)
        {
            List<int> yCoords = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                yCoords.Add(location[i][0]);
                if (location[i][0] >= 23)
                    return true;
                if ((location[i][0] < 0) | (location[i][1] < 0) | (location[i][1] > 9))
                    return null;
            }
            return location.Any(s =>
                yCoords.Max() - yCoords.Min() == 3
                    ? ((yCoords.Max() == s[0]) | (yCoords.Max() - 1 == s[0])) &
                      (Program.DroppedTetrominoeLocationGrid[s[0], s[1]] == 1)
                    : (yCoords.Max() == s[0]) & (Program.DroppedTetrominoeLocationGrid[s[0], s[1]] == 1));
        }

        public bool IsSomethingLeft() =>
            Location.Any(s => s[1] == 0 || Program.DroppedTetrominoeLocationGrid[s[0], s[1] - 1] == 1);

        private static bool? IsOverlayLeft(IReadOnlyList<int[]> location)
        {
            List<int> xCoords = new List<int>();
            xCoords.AddRange(location.Select(s => s[1]));
            for (int i = 0; i < 4; i++)
            {
                if (location[i][1] < 0)
                    return true;
                if (location[i][1] > 9)
                    return false;
                if ((location[i][0] >= 23) | (location[i][0] < 0))
                    return null;
            }
            return location.Any(s => xCoords.Max() - xCoords.Min() == 3
                ? (xCoords.Min() == s[1]) | (xCoords.Min() + 1 == s[1]) &&
                  Program.DroppedTetrominoeLocationGrid[s[0], s[1]] == 1
                : xCoords.Min() == s[1] && Program.DroppedTetrominoeLocationGrid[s[0], s[1]] == 1);
        }

        public bool IsSomethingRight() =>
            Location.Any(s => s[1] == 9 || Program.DroppedTetrominoeLocationGrid[s[0], s[1] + 1] == 1);

        private static bool? IsOverlayRight(IReadOnlyList<int[]> location)
        {
            List<int> xCoords = new List<int>();
            xCoords.AddRange(location.Select(s => s[1]));
            for (int i = 0; i < 4; i++)
            {
                if (location[i][1] > 9)
                    return true;
                if (location[i][1] < 0)
                    return false;
                if ((location[i][0] >= 23) | (location[i][0] < 0))
                    return null;
            }
            return location.Any(s => xCoords.Max() - xCoords.Min() == 3
                ? ((xCoords.Max() == s[1]) | (xCoords.Max() - 1 == s[1])) &
                  (Program.DroppedTetrominoeLocationGrid[s[0], s[1]] == 1)
                : (xCoords.Max() == s[1]) & (Program.DroppedTetrominoeLocationGrid[s[0], s[1]] == 1));
        }

        public void Update()
        {
            for (int i = 0; i < 23; i++)
            for (int j = 0; j < 10; j++)
                Program.Grid[i, j] = 0;
            Location.ForEach(s => Program.Grid[s[0], s[1]] = 1);
            Program.Draw();
        }
    }
}
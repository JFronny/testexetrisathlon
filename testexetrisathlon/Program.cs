using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using testexetrisathlon.SoundManagement;
using static System.Console;

//┌─┐
//│ │
//└─┘

//          ▀▀▀██████▄▄▄
//                 ▀▀▀████▄
//          ▄███████▀   ▀███▄
//        ▄███████▀       ▀███▄
//      ▄████████           ███▄
//     ██████████▄           ███▌
//     ▀█████▀ ▀███▄         ▐███
//       ▀█▀     ▀███▄       ▐███
//                 ▀███▄     ███▌
//    ▄██▄           ▀███▄  ▐███
//  ▄██████▄           ▀███▄███
// █████▀▀████▄▄        ▄█████
// ████▀   ▀▀█████▄▄▄▄█████████▄
//  ▀▀         ▀▀██████▀▀   ▀▀██

namespace testexetrisathlon
{
    internal static class Program
    {
        public const string Sqr = "■";
        private const string Intro = "Intro";
        private const string GameOver = "GameOver";
        public static int[,] Grid = new int[23, 10];
        public static int[,] DroppedTetrominoeLocationGrid = new int[23, 10];
        private static Stopwatch _dropTimer = new Stopwatch();
        private static int _dropTime;
        private static int _dropRate = 300;
        public static bool IsDropped;
        private static Tetrominoe _tet;
        private static Tetrominoe _nextTet;
        private static ConsoleKeyInfo _key;
        private static bool _isKeyPressed;
        private static int _linesCleared;
        private static int _score;
        private static int _level = 1;
        private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
        private static readonly ConsoleColor[] Colors = {BackgroundColor, ForegroundColor};
        public static bool Debug;

        public static readonly Random Rnd = new Random();
        private static ISoundManager soundManager;
        private static string InGame => SettingsMan.UsingAltTrack ? "InGame2" : "InGame1";

#if DEBUG
        private static void Main()
        {
            Debug = true;
#else
        private static void Main(string[] args)
        {
            Debug = args.Contains("debug");
#endif
            BackgroundColor = ConsoleColor.Red;
            ForegroundColor = ConsoleColor.Yellow;
            soundManager = OSCheck.IsWindows ? (ISoundManager) new WindowsSoundManager() : new LinuxSoundManager();
            soundManager.Init(new Dictionary<string, string>
            {
                {"Intro", "testexetrisathlon.Intro.wav"},
                {"InGame1", "testexetrisathlon.InGame1.wav"},
                {"InGame2", "testexetrisathlon.InGame2.wav"},
                {"GameOver", "testexetrisathlon.GameOver.wav"}
            });
            if (OSCheck.IsWindows)
            {
                SetWindowSize(42, 29);
                if (Debug)
                    SetWindowSize(50, 40);
            }
            SetCursorPosition(0, 0);
            bool playing = true;
            GameState state = GameState.Menu;
            try
            {
                while (playing)
                    switch (state)
                    {
                        case GameState.Menu:
                            Clear();
                            soundManager.SetCurrent(Intro);
                            DrawSymbol();
                            SetCursorPosition(12, 18);
                            Write("HighScore: " + SettingsMan.HighScore);
                            SetCursorPosition(12, 20);
                            Write("Controls: Space");
                            SetCursorPosition(13, 21);
                            Write("Up, Down, Right");
                            SetCursorPosition(13, 22);
                            Write("Left");
                            SetCursorPosition(12, 24);
                            Write("Press s to start");
                            SetCursorPosition(12, 25);
                            Write("Press x to exit");
                            SetCursorPosition(12, 26);
                            Write("Press v for settings");
                            SetCursorPosition(0, 28);
                            Write("Icon made by Freepik from www.flaticon.com");
                            string tmp = ReadKey(true).KeyChar.ToString().ToLower();
                            switch (tmp)
                            {
                                case "s":
                                    state = GameState.Game;
                                    Clear();
                                    DrawBorder();
                                    break;
                                case "x":
                                    state = GameState.Exit;
                                    break;
                                case "v":
                                    SettingsMenu();
                                    break;
                            }
                            break;
                        case GameState.Game:
                            soundManager.SetCurrent(InGame);
                            _dropTimer.Start();
                            SetCursorPosition(25, 0);
                            WriteLine("Level " + _level);
                            SetCursorPosition(25, 1);
                            WriteLine("Score " + _score + "/" + (Math.Pow(_level, 2) * 100));
                            SetCursorPosition(25, 2);
                            WriteLine("LinesCleared " + _linesCleared);
                            SetCursorPosition(25, 4);
                            WriteLine("HighScore " + SettingsMan.HighScore);
                            _nextTet = new Tetrominoe();
                            _tet = _nextTet;
                            _tet.Spawn();
                            _nextTet = new Tetrominoe();
                            Update();
                            state = GameState.GameOver;
                            break;
                        case GameState.GameOver:
                            SettingsMan.HighScore = _score;
                            soundManager.SetCurrent(GameOver);
                            string input = "";
                            while (input != "y" && input != "n")
                            {
                                Clear();
                                DrawBorder();
                                Draw();
                                SetCursorPosition(0, 0);
                                WriteLine("┌───────────────────┐");
                                WriteLine("│     Game Over     │");
                                WriteLine("│   Replay? (Y/N)   │");
                                WriteLine("├───────────────────┤");
                                input = ReadKey().KeyChar.ToString().ToLower();
                            }
                            Grid = new int[23, 10];
                            DroppedTetrominoeLocationGrid = new int[23, 10];
                            _dropTimer = new Stopwatch();
                            _dropRate = 300;
                            IsDropped = false;
                            _isKeyPressed = false;
                            _linesCleared = 0;
                            _score = 0;
                            _level = 1;
                            GC.Collect();
                            Clear();
                            DrawBorder();
                            state = input == "y" ? GameState.Game : GameState.Menu;
                            break;
                        case GameState.Exit:
                            playing = false;
                            break;
                        default: throw new ArgumentOutOfRangeException();
                    }
            }
            finally
            {
                soundManager.Dispose();
            }
            BackgroundColor = Colors[0];
            ForegroundColor = Colors[1];
            SetCursorPosition(0, 0);
            Clear();
            Beeper.Dispose();
        }

        private static void DrawSymbol()
        {
            SetCursorPosition(0, 1);
            Write(
                "               ▀▀▀██████▄▄▄\r\n" +
                "                      ▀▀▀████▄\r\n" +
                "               ▄███████▀   ▀███▄\r\n" +
                "             ▄███████▀       ▀███▄\r\n" +
                "           ▄████████           ███▄\r\n" +
                "          ██████████▄           ███▌\r\n" +
                "          ▀█████▀ ▀███▄         ▐███\r\n" +
                "            ▀█▀     ▀███▄       ▐███\r\n" +
                "                      ▀███▄     ███▌\r\n" +
                "         ▄██▄           ▀███▄  ▐███\r\n" +
                "       ▄██████▄           ▀███▄███\r\n" +
                "      █████▀▀████▄▄        ▄█████\r\n" +
                "      ████▀   ▀▀█████▄▄▄▄█████████▄\r\n" +
                "       ▀▀         ▀▀██████▀▀   ▀▀██\r\n\r\n" +
                "     testexetrisathlon v." + Assembly.GetName().Version);
        }

        private static void SettingsMenu()
        {
            Clear();
            DrawSymbol();
            bool barActive = true;
            int currentSetting = 0;
            while (barActive)
            {
                bool curr = SettingsMan.UsingAltTrack;
                SetCursorPosition(3, 20);
                ForegroundColor = currentSetting == 0 ? ConsoleColor.White : ConsoleColor.Yellow;
                Write("Volume: " + new string('=', SettingsMan.Volume * 2) + "[" + SettingsMan.Volume.ToString("00") +
                      "]" + new string('=', 20 - (SettingsMan.Volume * 2)));
                SetCursorPosition(5, 22);
                ForegroundColor = currentSetting == 1 ? ConsoleColor.White : ConsoleColor.Yellow;
                Write($"{(curr ? "  Using" : "Not using")} alternative soundtrack  ");
                ForegroundColor = ConsoleColor.Yellow;
                switch (currentSetting)
                {
                    case 0:
                        switch (ReadKey().Key)
                        {
                            case ConsoleKey.LeftArrow:
                                SettingsMan.Volume--;
                                break;
                            case ConsoleKey.RightArrow:
                                SettingsMan.Volume++;
                                break;
                            case ConsoleKey.Enter:
                                barActive = false;
                                break;
                            case ConsoleKey.Tab:
                                currentSetting = 1;
                                break;
                        }
                        break;
                    case 1:
                        switch (ReadKey().Key)
                        {
                            case ConsoleKey.LeftArrow:
                            case ConsoleKey.RightArrow:
                            case ConsoleKey.Spacebar:
                                SettingsMan.UsingAltTrack = !curr;
                                break;
                            case ConsoleKey.Enter:
                                barActive = false;
                                break;
                            case ConsoleKey.Tab:
                                currentSetting = 0;
                                break;
                        }
                        break;
                }
                soundManager.SetVolume(SettingsMan.Volume * 10);
            }
        }

        private static void Update()
        {
            while (true)
            {
                _dropTime = (int) _dropTimer.ElapsedMilliseconds;
                if (_dropTime > _dropRate)
                {
                    _dropTime = 0;
                    _dropTimer.Restart();
                    _tet.Drop();
                }
                if (IsDropped)
                {
                    _tet = _nextTet;
                    _nextTet = new Tetrominoe();
                    _tet.Spawn();
                    IsDropped = false;
                    _score += 10;
                }
                for (int j = 0; j < 10; j++)
                    if (DroppedTetrominoeLocationGrid[0, j] == 1)
                        return;
                if (Debug)
                {
                    SetCursorPosition(0, 25);
                    WriteLine("!DEBUG MODE ENABLED!");
                }
                Input();
                ClearBlock();
            }
        }

        private static void ClearBlock()
        {
            int combo = 0;
            for (int i = 0; i < 23; i++)
                if (Enumerable.Range(0, 10).All(s => DroppedTetrominoeLocationGrid[i, s] != 0))
                {
                    _linesCleared++;
                    combo++;
                    Beeper.Beep(400, 200);
                    for (int j = 0; j < 10; j++) DroppedTetrominoeLocationGrid[i, j] = 0;
                    int[,] newDroppedTetrominoeLocationGrid = new int[23, 10];
                    for (int k = 1; k < i; k++)
                    for (int l = 0; l < 10; l++)
                        newDroppedTetrominoeLocationGrid[k + 1, l] = DroppedTetrominoeLocationGrid[k, l];
                    for (int k = 1; k < i; k++)
                    for (int l = 0; l < 10; l++)
                        DroppedTetrominoeLocationGrid[k, l] = 0;
                    for (int k = 0; k < 23; k++)
                    for (int l = 0; l < 10; l++)
                        if (newDroppedTetrominoeLocationGrid[k, l] == 1)
                            DroppedTetrominoeLocationGrid[k, l] = 1;
                    Draw();
                }
            _score += (int) Math.Round(Math.Sqrt(Math.Max((combo * 50) - 50, 0)) * 5);
            _level = (int) Math.Round(Math.Sqrt(_score * 0.01)) + 1;
            _dropRate = 300 - (22 * _level);
        }

        private static void Input()
        {
            _isKeyPressed = KeyAvailable;
            SetCursorPosition(0, 24);
            if (_isKeyPressed)
                _key = ReadKey();
            if ((_key.Key == ConsoleKey.LeftArrow) & !_tet.IsSomethingLeft() & _isKeyPressed)
            {
                for (int i = 0; i < 4; i++)
                    _tet.Location[i][1] -= 1;
                _tet.Update();
            }
            else if ((_key.Key == ConsoleKey.RightArrow) & !_tet.IsSomethingRight() & _isKeyPressed)
            {
                for (int i = 0; i < 4; i++)
                    _tet.Location[i][1] += 1;
                _tet.Update();
            }
            if ((_key.Key == ConsoleKey.DownArrow) & _isKeyPressed)
                _tet.Drop();
            if ((_key.Key == ConsoleKey.UpArrow) & _isKeyPressed)
                for (; _tet.IsSomethingBelow != true;)
                    _tet.Drop();
            if (!((_key.Key == ConsoleKey.Spacebar) & _isKeyPressed)) return;
            _tet.Rotate();
            _tet.Update();
        }

        public static void Draw()
        {
            for (int i = 0; i < 23; ++i)
            for (int j = 0; j < 10; j++)
            {
                SetCursorPosition((2 * j) + 1, i);
                if ((Grid[i, j] == 1) | (DroppedTetrominoeLocationGrid[i, j] == 1))
                {
                    SetCursorPosition((2 * j) + 1, i);
                    Write(Sqr);
                }
                else
                {
                    Write(" ");
                }
            }
            SetCursorPosition(25, 0);
            WriteLine("Level " + _level);
            SetCursorPosition(25, 1);
            WriteLine("Score " + _score + "/" + (Math.Pow(_level, 2) * 100));
            SetCursorPosition(25, 2);
            WriteLine("LinesCleared " + _linesCleared);
            DrawBorder();
        }

        public static void DrawBorder()
        {
            for (int lengthCount = 0; lengthCount <= 22; lengthCount++)
            {
                SetCursorPosition(0, lengthCount);
                Write("│");
                SetCursorPosition(20, lengthCount);
                Write("│");
            }
            SetCursorPosition(0, 23);
            Write("└");
            for (int widthCount = 0; widthCount <= 18; widthCount++) Write("─");
            Write("┘");
        }

        private enum GameState
        {
            Exit,
            Menu,
            Game,
            GameOver
        }
    }
}
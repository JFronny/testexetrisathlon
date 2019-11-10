using System;
using System.Linq;
using System.Diagnostics;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
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
    class Program
    {
        [DllImport("winmm.dll")]
        static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")]
        static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);
        public static string sqr = "■";
        public static int[,] grid = new int[23, 10];
        public static int[,] droppedtetrominoeLocationGrid = new int[23, 10];
        public static Stopwatch dropTimer = new Stopwatch();
        public static Stopwatch inputTimer = new Stopwatch();
        public static int dropTime, dropRate = 300;
        public static bool isDropped = false;
        static Tetrominoe tet;
        static Tetrominoe nexttet;
        public static ConsoleKeyInfo key;
        public static bool isKeyPressed = false;
        public static int linesCleared = 0, score = 0, level = 1;
        static readonly Assembly assembly = Assembly.GetExecutingAssembly();
        static ConsoleColor[] colors;
        public static bool debug;
        public static Random rnd;
        static void Main(string[] args)
        {
            rnd = new Random();
            colors = new ConsoleColor[2] { BackgroundColor, ForegroundColor };
            BackgroundColor = ConsoleColor.Red;
            ForegroundColor = ConsoleColor.Yellow;
            SetWindowSize(42, 29);
            SetCursorPosition(0, 0);
            Clear();
#if (DEBUG)
            debug = true;
#else
            debug = args.Contains("debug");
#endif
            if (debug)
                SetWindowSize(50, 40);
            new Program().MainN(
                new SoundPlayer(assembly.GetManifestResourceStream("testexetrisathlon.Intro.wav")),
                new SoundPlayer(assembly.GetManifestResourceStream("testexetrisathlon.InGame.wav")),
                new SoundPlayer(assembly.GetManifestResourceStream("testexetrisathlon.GameOver.wav")));
            BackgroundColor = colors[0];
            ForegroundColor = colors[1];
            SetCursorPosition(0, 0);
            Clear();
        }
        enum GameState { exit, menu, game, gameOver }
        static void DrawSymbol()
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

                "     testexetrisathlon v." + assembly.GetName().Version.ToString());
        }
        void MainN(SoundPlayer intro, SoundPlayer inGame, SoundPlayer gameOver)
        {
            int NewVolume = (ushort.MaxValue / 10) * SettingsMan.Volume;
            waveOutSetVolume(IntPtr.Zero, ((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
            bool playing = true;
            GameState state = GameState.menu;
            try
            {
                while (playing)
                {
                    switch (state)
                    {
                        case GameState.menu:
                            Clear();
                            gameOver.Stop();
                            intro.PlayLooping();
                            DrawSymbol();
                            SetCursorPosition(12, 18);
                            Write("Highscore: " + SettingsMan.HighScore.ToString());
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
                                    intro.Stop();
                                    state = GameState.game;
                                    Clear();
                                    DrawBorder();
                                    break;
                                case "x":
                                    state = GameState.exit;
                                    break;
                                case "v":
                                    VolumeSlider();
                                    break;
                            }
                            break;
                        case GameState.game:
                            inGame.PlayLooping();
                            dropTimer.Start();
                            SetCursorPosition(25, 0);
                            WriteLine("Level " + level);
                            SetCursorPosition(25, 1);
                            WriteLine("Score " + score);
                            SetCursorPosition(25, 2);
                            WriteLine("LinesCleared " + linesCleared);
                            SetCursorPosition(25, 4);
                            WriteLine("Highscore " + SettingsMan.HighScore);
                            nexttet = new Tetrominoe();
                            tet = nexttet;
                            tet.Spawn();
                            nexttet = new Tetrominoe();
                            Update();
                            inGame.Stop();
                            state = GameState.gameOver;
                            break;
                        case GameState.gameOver:
                            SettingsMan.HighScore = score;
                            gameOver.PlayLooping();
                            string input = "";
                            while ((input != "y") && (input != "n"))
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
                            grid = new int[23, 10];
                            droppedtetrominoeLocationGrid = new int[23, 10];
                            dropTimer = new Stopwatch();
                            inputTimer = new Stopwatch();
                            dropRate = 300;
                            isDropped = false;
                            isKeyPressed = false;
                            linesCleared = 0;
                            score = 0;
                            level = 1;
                            GC.Collect();
                            Clear();
                            DrawBorder();
                            if (input == "y")
                                state = GameState.game;
                            else
                                state = GameState.menu;
                            break;
                        default:
                            playing = false;
                            break;
                    }
                }
            }
            finally
            {
                intro.Dispose();
                inGame.Dispose();
                gameOver.Dispose();
            }
        }

        static void VolumeSlider()
        {
            Clear();
            DrawSymbol();
            bool barActive = true;
            while (barActive)
            {
                SetCursorPosition(3, 20);
                Write("Volume: " + new string('=', SettingsMan.Volume * 2) + "[" + SettingsMan.Volume.ToString("00") + "]" + new string('=', 20 - (SettingsMan.Volume * 2)));
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
                }
                int NewVolume = (ushort.MaxValue / 10) * SettingsMan.Volume;
                waveOutSetVolume(IntPtr.Zero, ((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
            }
        }

        static void Update()
        {
            while (true)
            {
                dropTime = (int)dropTimer.ElapsedMilliseconds;
                if (dropTime > dropRate)
                {
                    dropTime = 0; dropTimer.Restart(); tet.Drop();
                }
                if (isDropped == true)
                {
                    tet = nexttet;
                    nexttet = new Tetrominoe();
                    tet.Spawn();
                    isDropped = false;
                    score += 10;
                }
                for (int j = 0; j < 10; j++)
                {
                    if (droppedtetrominoeLocationGrid[0, j] == 1)
                        return;
                }
                if (debug)
                {
                    SetCursorPosition(0, 25);
                    WriteLine("!DEBUG MODE ENABLED!");
                }
                Input();
                ClearBlock();
            }
        }
        static void ClearBlock()
        {
            int combo = 0;
            for (int i = 0; i < 23; i++)
            {
                if (Enumerable.Range(0, 10).Where(s => droppedtetrominoeLocationGrid[i, s] == 0).Count() == 0)
                {
                    linesCleared++;
                    combo++;
                    Beep(400, 200);
                    for (int j = 0; j < 10; j++)
                    {
                        droppedtetrominoeLocationGrid[i, j] = 0;
                    }
                    int[,] newdroppedtetrominoeLocationGrid = new int[23, 10];
                    for (int k = 1; k < i; k++)
                    {
                        for (int l = 0; l < 10; l++)
                        {
                            newdroppedtetrominoeLocationGrid[k + 1, l] = droppedtetrominoeLocationGrid[k, l];
                        }
                    }
                    for (int k = 1; k < i; k++)
                    {
                        for (int l = 0; l < 10; l++)
                        {
                            droppedtetrominoeLocationGrid[k, l] = 0;
                        }
                    }
                    for (int k = 0; k < 23; k++)
                        for (int l = 0; l < 10; l++)
                            if (newdroppedtetrominoeLocationGrid[k, l] == 1)
                                droppedtetrominoeLocationGrid[k, l] = 1;
                    Draw();
                }
            }
            score += (int)Math.Round(Math.Sqrt(Math.Max((combo * 50) - 50, 0)) * 5);
            level = (int)Math.Round(Math.Sqrt(score * 0.01)) + 1;
            dropRate = 300 - (22 * level);
        }
        static void Input()
        {
            isKeyPressed = KeyAvailable;
            SetCursorPosition(0, 24);
            if (isKeyPressed)
                key = ReadKey();
            if (key.Key == ConsoleKey.LeftArrow & !tet.isSomethingLeft() & isKeyPressed)
            {
                for (int i = 0; i < 4; i++)
                    tet.location[i][1] -= 1;
                tet.Update();
            }
            else if (key.Key == ConsoleKey.RightArrow & !tet.isSomethingRight() & isKeyPressed)
            {
                for (int i = 0; i < 4; i++)
                    tet.location[i][1] += 1;
                tet.Update();
            }
            if (key.Key == ConsoleKey.DownArrow & isKeyPressed)
                tet.Drop();
            if (key.Key == ConsoleKey.UpArrow & isKeyPressed)
                for (; tet.isSomethingBelow!= true;)
                    tet.Drop();
            if (key.Key == ConsoleKey.Spacebar & isKeyPressed)
            {
                tet.Rotate();
                tet.Update();
            }
        }
        public static void Draw()
        {
            for (int i = 0; i < 23; ++i)
            {
                for (int j = 0; j < 10; j++)
                {
                    SetCursorPosition((2 * j) + 1, i);
                    if (grid[i, j] == 1 | droppedtetrominoeLocationGrid[i, j] == 1)
                    {
                        SetCursorPosition((2 * j) + 1, i);
                        Write(sqr);
                    }
                    else
                    {
                        Write(" ");
                    }
                }
            }
            SetCursorPosition(25, 0);
            WriteLine("Level " + level);
            SetCursorPosition(25, 1);
            WriteLine("Score " + score);
            SetCursorPosition(25, 2);
            WriteLine("LinesCleared " + linesCleared);
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
            for (int widthCount = 0; widthCount <= 18; widthCount++)
            {
                Write("─");
            }
            Write("┘");
        }
    }
}

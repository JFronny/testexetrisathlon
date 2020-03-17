using System;

namespace testexetrisathlon.SoundManagement
{
    public static class Beeper
    {
        public static void Beep(int frequency, int duration)
        {
            if (OSCheck.IsWindows)
                Console.Beep(frequency, duration);
            else
                Console.Write("\a");
        }
    }
}
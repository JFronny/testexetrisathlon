using System;

namespace testexetrisathlon.SoundManagement
{
    public class Beeper : IBeeper
    {
        public void Beep(int frequency, int duration) => Console.Beep(frequency, duration);
        public void Dispose()
        {
        }
    }
}
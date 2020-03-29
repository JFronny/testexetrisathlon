using System;

namespace testexetrisathlon.SoundManagement
{
    public interface IBeeper : IDisposable
    {
        public void Beep(int frequency, int duration);
    }
}
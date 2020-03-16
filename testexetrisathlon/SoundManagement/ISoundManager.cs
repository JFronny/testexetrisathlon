using System;
using System.Collections.Generic;

namespace testexetrisathlon.SoundManagement
{
    public interface ISoundManager : IDisposable
    {
        public void Init(Dictionary<string, string> manifestResources);
        public void SetCurrent(string id);
        public void SetVolume(int percent);
    }
}
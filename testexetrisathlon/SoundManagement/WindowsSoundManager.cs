using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NAudio.Wave;

namespace testexetrisathlon.SoundManagement
{
    public sealed class WindowsSoundManager : ISoundManager
    {
        private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
        private static WaveOutEvent _output;
        private static WaveStream _current;
        private static string _currentId;
        private Dictionary<string, LoopStream> _loadedSounds;

        public void Init(Dictionary<string, string> manifestResources)
        {
            _output = new WaveOutEvent();
            _loadedSounds = manifestResources.ToDictionary(s => s.Key,
                s => new LoopStream(new WaveFileReader(Assembly.GetManifestResourceStream(s.Value))));
        }

        public void SetCurrent(string id)
        {
            if (_currentId == id) return;
            _currentId = id;
            if (_current != null)
                _current.Position = 0;
            _current = _loadedSounds[id];
            _output.Stop();
            _output.Init(_current);
            _output.Play();
        }

        public void SetVolume(int percent) => _output.Volume = percent / 100f;

        public void Dispose()
        {
            foreach (LoopStream reader in _loadedSounds.Values) reader.Dispose();
            _output.Dispose();
        }
    }
}
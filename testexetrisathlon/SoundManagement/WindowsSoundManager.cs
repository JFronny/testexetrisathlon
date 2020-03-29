using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NAudio.Wave;

namespace testexetrisathlon.SoundManagement
{
    public sealed class SoundManager : ISoundManager
    {
        private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
        private WaveStream _current;
        private string _currentId;
        private Dictionary<string, LoopStream> _loadedSounds;
        private WaveOutEvent _output;

        public void Init(Dictionary<string, string> manifestResources)
        {
            _output = new WaveOutEvent();
            _loadedSounds = manifestResources.ToDictionary(s => s.Key,
                s => new LoopStream(new WaveFileReader(Mp3ToWav(Assembly.GetManifestResourceStream(s.Value)))));
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

        private static MemoryStream Mp3ToWav(Stream mp3File)
        {
            using Mp3FileReader reader = new Mp3FileReader(mp3File);
            using WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader);
            MemoryStream ms = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(ms, pcmStream);
            ms.Position = 0;
            return ms;
        }
    }
}
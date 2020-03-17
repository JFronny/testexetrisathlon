using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Bassoon;

namespace testexetrisathlon.SoundManagement
{
    public class LinuxSoundManager : ISoundManager
    {
        private BassoonEngine _bassoon;
        private Dictionary<string, Sound> _loadedSounds;
        private Dictionary<string, string> _files;
        private string _current;
        private int _volume = 100;
        private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
        public void Dispose()
        {
            foreach (Sound sound in _loadedSounds.Values) sound.Dispose();
            _bassoon.Dispose();
            foreach (string file in _files.Values) File.Delete(file);
        }

        public void Init(Dictionary<string, string> manifestResources)
        {
            _bassoon = new BassoonEngine();
            _files = new Dictionary<string, string>();
            _loadedSounds = new Dictionary<string, Sound>();
            foreach ((string name, string key) in manifestResources)
            {
                string file = Path.GetTempFileName();
                File.Move(file, Path.ChangeExtension(file, "wav"));
                file = Path.ChangeExtension(file, "wav");
                using Stream resource = Assembly.GetManifestResourceStream(key);
                using FileStream fileStream = File.Create(file);
                resource.Seek(0, SeekOrigin.Begin);
                resource.CopyTo(fileStream);
                _files.Add(name, file);
                _loadedSounds.Add(name, new Sound(file));
            }
            Console.Clear();
        }

        public void SetCurrent(string id)
        {
            if (_current == id) return;
            if (!string.IsNullOrWhiteSpace(_current))
            {
                _loadedSounds[_current].Pause();
                _loadedSounds[_current].Cursor = 0;
            }
            _current = id;
            _loadedSounds[_current].Play();
            Console.Clear();
        }

        public void SetVolume(int percent)
        {
            _volume = percent;
            foreach (Sound sound in _loadedSounds.Values) sound.Volume = _volume / 100f;
        }
    }
}
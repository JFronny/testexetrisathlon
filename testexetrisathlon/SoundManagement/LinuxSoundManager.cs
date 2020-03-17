using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace testexetrisathlon.SoundManagement
{
    public class LinuxSoundManager : ISoundManager
    {
        private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
        private ProcessLoop? _alsaProc;
        private string _current;
        private Dictionary<string, string> _files;

        public void Dispose()
        {
            foreach (string file in _files.Values) File.Delete(file);
            if (_alsaProc != null) _alsaProc.IsRunning = false;
        }

        public void Init(Dictionary<string, string> manifestResources)
        {
            _files = new Dictionary<string, string>();
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
            }
        }

        public void SetCurrent(string id)
        {
            if (_current == id) return;
            if (_alsaProc != null) _alsaProc.IsRunning = false;
            _current = id;
            //TODO fix actually killing, remove orphan processes
            _alsaProc = new ProcessLoop(new ProcessStartInfo
            {
                FileName = "aplay",
                Arguments = $"-q {_files[id].Replace("\"", "\\\"")}",
                CreateNoWindow = true
            });
            _alsaProc.CreateLoopThread().Start();
        }

        public void SetVolume(int percent)
        {
        }
    }
}
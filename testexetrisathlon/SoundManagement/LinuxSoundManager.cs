using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace testexetrisathlon.SoundManagement
{
    public class LinuxSoundManager : ISoundManager
    {
        private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
        private Dictionary<string, string> _files;
        private string _current;
        private Process? alsa;
        public void Dispose()
        {
            foreach (string file in _files.Values) File.Delete(file);
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
            alsa?.Kill();
            _current = id;
            //TODO fix actually killing, remove orphan processes
            alsa = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/sh",
                    Arguments = $"-c \"while [ true ]; do aplay -q {_files[id].Replace("\"", "\\\"")}; done\"",
                    RedirectStandardOutput = false,
                    RedirectStandardInput = false,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            alsa.Start();
        }

        public void SetVolume(int percent)
        {
            
        }
    }
}
using System.Diagnostics;
using System.Threading;

namespace testexetrisathlon.SoundManagement
{
    internal class ProcessLoop
    {
        private readonly ProcessStartInfo _info;
        private Process _currentProc;
        private bool _isRunning = true;

        public ProcessLoop(ProcessStartInfo info) => _info = info;

        public bool IsRunning
        {
            set
            {
                if (_isRunning && !value)
                    _currentProc.Kill();
                _isRunning = value;
            }
        }

        public Thread CreateLoopThread() => new Thread(() =>
        {
            while (_isRunning)
            {
                _currentProc = Process.Start(_info);
                _currentProc.WaitForExit();
            }
        });
    }
}
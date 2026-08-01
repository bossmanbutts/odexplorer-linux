using System;
using System.Diagnostics;
using ODExplorer.Audio;

namespace ODExplorer.UI.Avalonia.Services
{
    public class AudioPlayer : IAudioPlayer
    {
        private Process? _proc;
        public bool IsPlaying => _proc != null && !_proc.HasExited;

        public void Play(string filePath)
        {
            try
            {
                var psi = new ProcessStartInfo("paplay", $"\"{filePath}\"")
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                _proc = Process.Start(psi);
            }
            catch
            {
                try
                {
                    var psi = new ProcessStartInfo("aplay", $"\"{filePath}\"")
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    _proc = Process.Start(psi);
                }
                catch
                {
                    // no-op
                }
            }
        }

        public void Stop()
        {
            try
            {
                if (IsPlaying)
                {
                    _proc?.Kill();
                    _proc = null;
                }
            }
            catch { }
        }
    }
}

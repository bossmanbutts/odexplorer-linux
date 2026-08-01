namespace ODExplorer.Audio
{
    public interface IAudioPlayer
    {
        void Play(string filePath);
        bool IsPlaying { get; }
        void Stop();
    }
}

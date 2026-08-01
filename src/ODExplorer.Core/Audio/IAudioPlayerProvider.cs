namespace ODExplorer.Audio
{
    public static class IAudioPlayerProvider
    {
        // UI layer should set Current to provide audio playback.
        public static IAudioPlayer? Current { get; set; }
    }
}

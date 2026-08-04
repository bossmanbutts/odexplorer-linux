// Countdown timer used by the Spansh fleet-carrier jump timer. Mirrors the
// ODUtils.Helpers.CountdownTimer surface consumed by Stores/SpanshCsvStore.cs.

using System;
using System.Threading;

namespace ODUtils.Helpers
{
    public sealed class CountdownTimer : IDisposable
    {
        private readonly object gate = new();
        private Timer? timer;
        private DateTime endTime;
        private bool running;
        private int lastEmittedSecond;

        public CountdownTimer(TimeSpan initial)
        {
            remaining = initial;
        }

        private TimeSpan remaining;

        public event EventHandler? CountDownFinishedEvent;
        public event EventHandler<string>? OnTick;
        public event EventHandler<bool>? OnTimerRunning;

        public bool TimerRunning => running;

        public void UpdateRuntime(TimeSpan span)
        {
            lock (gate)
            {
                remaining = span;
            }
        }

        public void Start()
        {
            lock (gate)
            {
                if (running)
                {
                    return;
                }

                running = true;
                lastEmittedSecond = -1;
                endTime = DateTime.UtcNow + remaining;
                timer?.Dispose();
                timer = new Timer(Tick, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(250));
            }

            OnTimerRunning?.Invoke(this, true);
        }

        public void Stop()
        {
            lock (gate)
            {
                if (running == false)
                {
                    return;
                }

                running = false;
                timer?.Dispose();
                timer = null;
            }

            OnTimerRunning?.Invoke(this, false);
        }

        private void Tick(object? state)
        {
            TimeSpan span;
            lock (gate)
            {
                span = endTime - DateTime.UtcNow;
            }

            if (span <= TimeSpan.Zero)
            {
                Stop();
                CountDownFinishedEvent?.Invoke(this, EventArgs.Empty);
                return;
            }

            var seconds = (int)span.TotalSeconds;
            if (seconds == lastEmittedSecond)
            {
                return;
            }

            lastEmittedSecond = seconds;
            OnTick?.Invoke(this, $"{span.Hours:00}:{span.Minutes:00}:{span.Seconds:00}");
        }

        public void Dispose()
        {
            Stop();
        }
    }
}

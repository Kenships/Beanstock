using System;

namespace Util
{
    public class Timer
    {
        public float RemainingSeconds { get; private set; }

        public float PreviousDuration { get; private set; }

        public event Action OnTimerEnd;

        public Timer(float duration)
        {
            RemainingSeconds = duration;
            PreviousDuration = duration;
        }

        public void Tick(float deltaTime)
        {
            if (RemainingSeconds == 0f) return;

            RemainingSeconds -= deltaTime;

            CheckForEnd();
        }

        public void Restart()
        {
            Restart(PreviousDuration);
        }

        public void Restart(float duration)
        {
            RemainingSeconds = duration;
        }

        public void ForceEnd()
        {
            Cancel();
            OnTimerEnd?.Invoke();
        }

        public void Cancel()
        {
            RemainingSeconds = 0;
        }

        private void CheckForEnd()
        {
            if (RemainingSeconds > 0) return;

            RemainingSeconds = 0;

            OnTimerEnd?.Invoke();
        }
    }
}

using System;

namespace Util
{
    public class Timer
    {
        public float RemainingSeconds { get; private set; }

        public float PreviousDuration { get; private set; }

        public event Action OnTimerEnd;
        
        public bool IsRunning {get; private set;}

        public Timer(float duration)
        {
            RemainingSeconds = duration;
            PreviousDuration = duration;
            IsRunning = true;
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
            IsRunning = true;
        }

        public void ForceEnd()
        {
            Cancel();
            OnTimerEnd?.Invoke();
        }

        public void Cancel()
        {
            RemainingSeconds = 0;
            IsRunning = false;
        }

        private void CheckForEnd()
        {
            if (RemainingSeconds > 0) return;

            RemainingSeconds = 0;
            IsRunning = false;
            OnTimerEnd?.Invoke();
        }
    }
}

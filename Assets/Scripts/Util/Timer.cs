using System;
using UnityEngine;

namespace Util
{
    public class Timer
    {
        public float RemainingSeconds { get; private set; }

        public float PreviousDuration { get; private set; }

        public event Action OnTimerEnd;
        public event Action OnTimerStart;
        
        public bool IsRunning {get; private set;}

        private bool _isStarted;

        public Timer(float duration)
        {
            RemainingSeconds = duration;
            PreviousDuration = duration;
            IsRunning = duration != 0f;
        }

        public void Tick(float deltaTime)
        {
            if (RemainingSeconds == 0f) return;
            
            if (!_isStarted)
            {
                OnTimerStart?.Invoke();
                _isStarted = true;
            }
            
            RemainingSeconds -= deltaTime;

            CheckForEnd();
        }

        public void Restart()
        {
            Restart(PreviousDuration);
        }

        public void Restart(float duration)
        {
            PreviousDuration = duration;
            RemainingSeconds = duration;
            IsRunning = duration != 0;
            _isStarted = false;
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

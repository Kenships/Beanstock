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
        private bool _invokeStartOnZero;
        private bool _invokeEndOnZero;
        private bool _isLooping;

        public Timer(float duration, bool invokeStartOnZero = false, bool invokeEndOnZero = false)
        {
            _invokeStartOnZero = invokeStartOnZero;
            _invokeEndOnZero = invokeEndOnZero;
            
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
            
            if (duration != 0) return;
            
            if(_invokeStartOnZero)
            {
                OnTimerStart?.Invoke();
            }
            if(_invokeEndOnZero)
            {
                OnTimerEnd?.Invoke();
            }

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
            if (IsLooping())
            {
                Restart();
            }
        }
        
        public void AddTime(float time)
        {
            RemainingSeconds += time;
        }

        public void SetInvokeStartOnZero(bool value)
        {
            _invokeStartOnZero = value;
        }
        
        public void SetInvokeEndOnZero(bool value)
        {
            _invokeEndOnZero = value;
        }
        public void Loop()
        {
            _isLooping = true;
        }
        public bool IsLooping()
        {
            return _isLooping;
        }
        public void StopLooping()
        {
            _isLooping = false;
        }
        
    }
}

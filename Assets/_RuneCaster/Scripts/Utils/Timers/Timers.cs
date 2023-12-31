using System;
using UnityEngine;

namespace Timers {
    public class CountdownTimer {
        public float Duration;
        public bool IsTicking { get; private set; }

        /// <summary>
        /// TickEvent invokes with percentage of time remaining.
        /// </summary>
        public Action<float> TickEvent;
        public Action EndEvent;

        float _timer;

        public CountdownTimer(float duration) {
            Duration = duration;
        }

        public void Start() {
            if (IsTicking) Stop();
            
            _timer = Duration;
            IsTicking = true;
            GlobalClock.onTick += Timer;
        }
        public void Stop() {
            IsTicking = false;
            GlobalClock.onTick -= Timer;
            EndEvent?.Invoke();
        }

        void Timer(float deltaTime) {
            _timer -= deltaTime;

            // Invoke TickEvent
            if (_timer < 0) _timer = 0; // needed for invoking TickEvent with non-negative percent
            if (TickEvent != null) TickEvent?.Invoke(_timer / Duration);

            // Finished Timer
            if (_timer == 0) {
                Stop();
            }
        }
    }
}
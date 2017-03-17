using System;

namespace Overlay.NET.Common {
    /// <summary>
    /// </summary>
    public class TickEngine {
        /// <summary>
        ///     Gets or sets the interval.
        /// </summary>
        /// <value>
        ///     The interval.
        /// </value>
        public TimeSpan Interval {
            get { return _waitTimer.WaitTime; }
            set { _waitTimer.WaitTime = value; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is ticking.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is ticking; otherwise, <c>false</c>.
        /// </value>
        public bool IsTicking { get; set; }

        /// <summary>
        ///     The wait timer
        /// </summary>
        private readonly WaitTimer _waitTimer = new WaitTimer();

        /// <summary>
        ///     Occurs when [pre tick].
        /// </summary>
        public event EventHandler PreTick;

        /// <summary>
        ///     Occurs when [tick].
        /// </summary>
        public event EventHandler Tick;

        /// <summary>
        ///     Occurs when [post tick].
        /// </summary>
        public event EventHandler PostTick;

        /// <summary>
        ///     Stops this instance.
        /// </summary>
        public void Stop() => _waitTimer.Stop();

        /// <summary>
        ///     Pulses this instance.
        /// </summary>
        public void Pulse() {
            if (!IsTicking) {
                return;
            }

            if (!_waitTimer.Update()) {
                return;
            }

            OnPreTick();

            OnTick();

            OnPostTick();

            _waitTimer.Reset();
        }

        /// <summary>
        ///     Called when [tick].
        /// </summary>
        protected virtual void OnTick() => Tick?.Invoke(this, EventArgs.Empty);

        /// <summary>
        ///     Called when [pre tick].
        /// </summary>
        protected virtual void OnPreTick() => PreTick?.Invoke(this, EventArgs.Empty);

        /// <summary>
        ///     Called when [post tick].
        /// </summary>
        protected virtual void OnPostTick() => PostTick?.Invoke(this, EventArgs.Empty);
    }
}
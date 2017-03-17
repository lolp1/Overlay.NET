using System;

namespace Overlay.NET.Common {
    /// <summary>
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public sealed class WaitTimerEventArgs : EventArgs {
        /// <summary>
        ///     Gets or sets the time finished.
        /// </summary>
        /// <value>
        ///     The time finished.
        /// </value>
        public DateTime TimeFinished { get; set; }

        /// <summary>
        ///     Gets or sets the wait time.
        /// </summary>
        /// <value>
        ///     The wait time.
        /// </value>
        public TimeSpan WaitTime { get; set; }

        /// <summary>
        ///     Gets or sets the time started.
        /// </summary>
        /// <value>
        ///     The time started.
        /// </value>
        public DateTime TimeStarted { get; set; }
    }

    /// <summary>
    /// </summary>
    public class WaitTimer {
        /// <summary>
        ///     Gets or sets the wait time.
        /// </summary>
        /// <value>
        ///     The wait time.
        /// </value>
        public TimeSpan WaitTime { get; set; }

        /// <summary>
        ///     Gets the end time.
        /// </summary>
        /// <value>
        ///     The end time.
        /// </value>
        public DateTime EndTime => StartTime + WaitTime;

        /// <summary>
        ///     Gets the start time.
        /// </summary>
        /// <value>
        ///     The start time.
        /// </value>
        public DateTime StartTime { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is finished.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is finished; otherwise, <c>false</c>.
        /// </value>
        public bool IsFinished => EndTime <= DateTime.Now;

        /// <summary>
        ///     Gets the time left.
        /// </summary>
        /// <value>
        ///     The time left.
        /// </value>
        public TimeSpan TimeLeft => EndTime - DateTime.Now;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WaitTimer" /> class.
        /// </summary>
        /// <param name="waitTime">The wait time.</param>
        public WaitTimer(TimeSpan waitTime) {
            WaitTime = waitTime;
            Stop();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WaitTimer" /> class.
        /// </summary>
        public WaitTimer() {
            WaitTime = TimeSpan.Zero;
            Stop();
        }

        /// <summary>
        ///     Occurs when [finished].
        /// </summary>
        public event EventHandler<WaitTimerEventArgs> Finished;

        /// <summary>
        ///     Resets this instance.
        /// </summary>
        public void Reset() => StartTime = DateTime.Now;

        /// <summary>
        ///     Stops this instance.
        /// </summary>
        public void Stop() => StartTime = DateTime.Now.AddDays(-5.0);

        /// <summary>
        ///     Updates this instance.
        /// </summary>
        /// <returns></returns>
        public bool Update() {
            if (!IsFinished) {
                return false;
            }

            OnFinished(new WaitTimerEventArgs {
                TimeFinished = DateTime.Now,
                TimeStarted = StartTime,
                WaitTime = WaitTime
            });

            return true;
        }

        /// <summary>
        ///     Raises the <see cref="E:Finished" /> event.
        /// </summary>
        /// <param name="e">The <see cref="WaitTimerEventArgs" /> instance containing the event data.</param>
        protected virtual void OnFinished(WaitTimerEventArgs e) => Finished?.Invoke(this, e);
    }
}
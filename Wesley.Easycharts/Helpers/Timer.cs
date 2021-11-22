namespace DCMS.Easycharts
{
    using System;

    /// <summary>
    /// A loop that executes an action.
    /// </summary>
    public static class Timer
    {
        /// <summary>
        /// Gets or sets a factory used to instanciate timers.
        /// </summary>
        /// <value>The factory function.</value>
        public static Func<ITimer> Create { get; set; } = () => new DelayTimer();
    }
}

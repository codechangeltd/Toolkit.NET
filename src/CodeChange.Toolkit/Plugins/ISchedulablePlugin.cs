namespace CodeChange.Toolkit.Plugins
{
    using System;

    /// <summary>
    /// Defines a contract for a single schedulable plug-in
    /// </summary>
    public interface ISchedulablePlugin : IPlugin
    {
        /// <summary>
        /// Executes the schedule
        /// </summary>
        void ExecuteSchedule();

        /// <summary>
        /// Gets the next scheduled date for the plug-in
        /// </summary>
        /// <returns>The next scheduled date</returns>
        DateTime? GetNextScheduledDate();

        /// <summary>
        /// Determines if the scheduled task is overdue
        /// </summary>
        /// <returns>True, if the task is overdue; otherwise false</returns>
        bool IsOverdue();
    }
}

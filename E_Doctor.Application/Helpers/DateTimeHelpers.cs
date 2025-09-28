using System.Globalization;
namespace E_Doctor.Application.Helpers
{
    /// <summary>
    /// Provides helper methods for common DateTime conversions and formatting.
    /// </summary>
    public static class DateTimeHelper
    {
        // Custom format string for "Sep-28-2025 5:32 PM"
        private const string CustomFormat = "MMM-dd-yyyy h:mm tt";

        /// <summary>
        /// Converts a DateTime (assumed to be UTC) to the local time 
        /// and formats it as a string in the pattern "MMM-dd-yyyy h:mm tt".
        /// </summary>
        /// <param name="utcDateTime">The UTC DateTime object to convert and format.</param>
        /// <returns>A formatted string representing the local date and time.</returns>
        public static string ToLocalShortDateTimeString(DateTime utcDateTime)
        {
            // 1. Convert the UTC DateTime to the local time of the executing machine.
            DateTime localTime = utcDateTime.ToLocalTime();

            // 2. Format the local time using the custom pattern.
            // We use CultureInfo.InvariantCulture to ensure consistency for month abbreviations (MMM).
            return localTime.ToString(CustomFormat, CultureInfo.InvariantCulture);
        }

        // Optional: A public method that specifically uses DateTime.UtcNow
        public static string CurrentUtcToLocalShortDateTimeString()
        {
            return ToLocalShortDateTimeString(DateTime.UtcNow);
        }
    }
}

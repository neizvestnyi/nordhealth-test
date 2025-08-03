namespace Api.Helpers;

public static class DateRangeHelper
{
    /// <summary>
    /// Checks if two date ranges overlap.
    /// </summary>
    /// <param name="range1Start">Start of the first date range</param>
    /// <param name="range1End">End of the first date range</param>
    /// <param name="range2Start">Start of the second date range</param>
    /// <param name="range2End">End of the second date range</param>
    /// <returns>True if the ranges overlap, false otherwise</returns>
    public static bool IsOverlapping(DateTime range1Start, DateTime range1End, DateTime range2Start, DateTime range2End)
    {
        if (range1Start > range1End || range2Start > range2End)
        {
            throw new ArgumentException("Start date must be before or equal to end date");
        }

        return range1Start < range2End && range1End > range2Start;
    }
}
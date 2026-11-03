namespace TravelClaim.Web.Services;

public class AllowanceCalculator
{
    public decimal GetDailyAllowance(DateTime departure, DateTime currentDate)
    {
        if (currentDate.Date > departure.Date.AddYears(2))
        {
            return 150;
        }

        if (currentDate.Date > departure.Date.AddMonths(3))
        {
            return 210;
        }

        return 300;
    }
}
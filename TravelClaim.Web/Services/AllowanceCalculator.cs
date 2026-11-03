using TravelClaim.Web.Models;
using TravelClaimModel = global::TravelClaim.Web.Models.TravelClaim;

namespace TravelClaim.Web.Services;

public class AllowanceCalculator
{
    public List<DailyCalculation> CalculateTrip(TravelClaimModel claim)
    {
        if (!claim.HasOvernightStay ||
            claim.DistanceFromHome <= 50 ||
            claim.DistanceFromWorkplace <= 50)
        {
            claim.TotalTaxFreeAmount = 0;
            claim.TaxableAmount = claim.PaidAmount;

            return new List<DailyCalculation>();
        }

        List<DailyCalculation> calculations = new List<DailyCalculation>();

        DateTime currentDate = claim.Departure.Date;

        while (currentDate <= claim.ReturnDate.Date)
        {
            DailyCalculation day = new DailyCalculation();
            day.Date = DateOnly.FromDateTime(currentDate);

            day = CalculateDay(day, claim);
            calculations.Add(day);

            currentDate = currentDate.AddDays(1);
        }

        decimal eligibleAmount = 0;

        foreach (DailyCalculation day in calculations)
        {
            eligibleAmount += day.TaxFreeAmount;
        }

        claim.TotalTaxFreeAmount = Math.Min(claim.PaidAmount, eligibleAmount);
        claim.TaxableAmount = Math.Max(0, claim.PaidAmount - eligibleAmount);

        return calculations;
    }

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

    public decimal CalculateMealDeduction(DailyCalculation day, bool isFullDay, decimal amount)
    {
        decimal breakfastDeduction = 0;
        decimal lunchDeduction = 0;
        decimal dinnerDeduction = 0;
        decimal deduction = 0;

        if (amount == 300)
        {
            breakfastDeduction = 60;
            lunchDeduction = 105;
            dinnerDeduction = 105;
        }
        else if (amount == 210)
        {
            breakfastDeduction = 42;
            lunchDeduction = 74;
            dinnerDeduction = 74;
        }
        else if (amount == 150)
        {
            breakfastDeduction = 30;
            lunchDeduction = 53;
            dinnerDeduction = 53;
        }

        if (day.BreakfastPaid)
        {
            deduction += breakfastDeduction;
        }

        if (day.LunchPaid && day.DinnerPaid)
        {
            if (amount == 300 && isFullDay)
            {
                deduction += 210;
            }
            else if (amount == 300 && !isFullDay)
            {
                deduction += 105;
            }
            else if (amount == 210)
            {
                deduction += 147;
            }
            else
            {
                deduction += 105;
            }
        }
        else
        {
            if (day.LunchPaid)
            {
                deduction += lunchDeduction;
            }

            if (day.DinnerPaid)
            {
                deduction += dinnerDeduction;
            }
        }

        return deduction;
    }

    public DailyCalculation CalculateDay(DailyCalculation day, TravelClaimModel claim)
    {
        decimal amount = 0;
        bool isFullDay = false;

        DateTime currentDate = day.Date.ToDateTime(TimeOnly.MinValue);
        decimal dailyAllowance = GetDailyAllowance(claim.Departure, currentDate);

        // Avresedag
        if (day.Date == DateOnly.FromDateTime(claim.Departure))
        {
            if (claim.Departure.TimeOfDay < new TimeSpan(12, 0, 0))
            {
                amount = dailyAllowance;
                isFullDay = true;
            }
            else if (dailyAllowance == 300)
            {
                amount = 150;
            }
        }
        // Hemkomstdag
        else if (day.Date == DateOnly.FromDateTime(claim.ReturnDate))
        {
            if (claim.ReturnDate.TimeOfDay > new TimeSpan(19, 0, 0))
            {
                amount = dailyAllowance;
                isFullDay = true;
            }
            else if (dailyAllowance == 300)
            {
                amount = 150;
            }
        }
        // Mellandagar
        else
        {
            amount = dailyAllowance;
            isFullDay = true;
        }

        decimal deduction = CalculateMealDeduction(day, isFullDay, amount);

        // Nattraktamente
        if (claim.HasOvernightStay &&
            !claim.LodgingPaidByEmployer &&
            day.Date < DateOnly.FromDateTime(claim.ReturnDate))
        {
            day.NightAllowance = 150;
        }
        else
        {
            day.NightAllowance = 0;
        }

        day.BaseAllowance = amount;
        day.MealDeduction = deduction;

        day.TaxFreeAmount = Math.Max(0, amount - deduction)
                            + day.NightAllowance;

        return day;
    }
}
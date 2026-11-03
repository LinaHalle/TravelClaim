
using Xunit;
using TravelClaim.Web.Models;
using TravelClaim.Web.Services;
using TravelClaimModel = global::TravelClaim.Web.Models.TravelClaim;

namespace TravelClaim.Tests;

public class AllowanceCalculatorTests
{
    private readonly AllowanceCalculator calculator = new AllowanceCalculator();

    [Fact]
    public void FullDay_AllMealsPaid_Returns270Deduction()
    {
        DailyCalculation day = new DailyCalculation();

        day.BreakfastPaid = true;
        day.LunchPaid = true;
        day.DinnerPaid = true;

        decimal result = calculator.CalculateMealDeduction(day, true, 300);

        Assert.Equal(270, result);
    }

    [Fact]
    public void HalfDay_AllMealsPaid_Returns135Deduction()
    {
        DailyCalculation day = new DailyCalculation();

        day.BreakfastPaid = true;
        day.LunchPaid = true;
        day.DinnerPaid = true;

        decimal result = calculator.CalculateMealDeduction(day, false, 150);

        Assert.Equal(135, result);
    }

    [Fact]
    public void HalfDay_LunchAndDinnerPaid_Returns105Deduction()
    {
        DailyCalculation day = new DailyCalculation();

        day.LunchPaid = true;
        day.DinnerPaid = true;

        decimal result = calculator.CalculateMealDeduction(day, false, 150);

        Assert.Equal(105, result);
    }

    [Fact]
    public void DepartureBefore12_Returns300ForFullDay()
    {
        TravelClaimModel claim = new TravelClaimModel();

        claim.Departure = new DateTime(2026, 11, 2, 10, 0, 0);
        claim.ReturnDate = new DateTime(2026, 11, 4, 20, 0, 0);

        DailyCalculation day = new DailyCalculation();
        day.Date = new DateOnly(2026, 11, 2);

        DailyCalculation result = calculator.CalculateDay(day, claim);

        Assert.Equal(300, result.TaxFreeAmount);
    }

    [Fact]
    public void DepartureAt12_Returns150ForHalfDay()
    {
        TravelClaimModel claim = new TravelClaimModel();

        claim.Departure = new DateTime(2026, 11, 2, 12, 0, 0);
        claim.ReturnDate = new DateTime(2026, 11, 4, 20, 0, 0);

        DailyCalculation day = new DailyCalculation();
        day.Date = new DateOnly(2026, 11, 2);

        DailyCalculation result = calculator.CalculateDay(day, claim);

        Assert.Equal(150, result.TaxFreeAmount);
    }

    [Fact]
    public void ReturnAt19_Returns150ForHalfDay()
    {
        TravelClaimModel claim = new TravelClaimModel();

        claim.Departure = new DateTime(2026, 11, 2, 10, 0, 0);
        claim.ReturnDate = new DateTime(2026, 11, 4, 19, 0, 0);

        DailyCalculation day = new DailyCalculation();
        day.Date = new DateOnly(2026, 11, 4);

        DailyCalculation result = calculator.CalculateDay(day, claim);

        Assert.Equal(150, result.TaxFreeAmount);
    }

    [Fact]
    public void ThreeDayTrip_CreatesThreeDailyCalculations()
    {
        TravelClaimModel claim = new TravelClaimModel();

        claim.HasOvernightStay = true;
        claim.DistanceFromHome = 60;
        claim.DistanceFromWorkplace = 70;
        claim.Departure = new DateTime(2026, 11, 2, 10, 0, 0);
        claim.ReturnDate = new DateTime(2026, 11, 4, 20, 0, 0);

        List<DailyCalculation> result = calculator.CalculateTrip(claim);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void TripWithoutOvernightStay_ReturnsEmptyList()
    {
        TravelClaimModel claim = new TravelClaimModel();

        claim.Departure = new DateTime(2026, 11, 2, 10, 0, 0);
        claim.ReturnDate = new DateTime(2026, 11, 2, 20, 0, 0);
        claim.HasOvernightStay = false;

        List<DailyCalculation> result = calculator.CalculateTrip(claim);

        Assert.Empty(result);
    }

    [Fact]
    public void DistanceExactly50Km_ReturnsEmptyList()
    {
        TravelClaimModel claim = new TravelClaimModel();

        claim.HasOvernightStay = true;
        claim.DistanceFromHome = 50;
        claim.DistanceFromWorkplace = 70;
        claim.Departure = new DateTime(2026, 11, 2, 10, 0, 0);
        claim.ReturnDate = new DateTime(2026, 11, 4, 20, 0, 0);

        List<DailyCalculation> result = calculator.CalculateTrip(claim);

        Assert.Empty(result);
    }

    [Fact]
    public void StayOverThreeMonths_Returns210PerDay()
    {
        DateTime departure = new DateTime(2026, 1, 1);
        DateTime currentDate = new DateTime(2026, 4, 2);

        decimal result = calculator.GetDailyAllowance(departure, currentDate);

        Assert.Equal(210, result);
    }

    [Fact]
    public void StayOverTwoYears_Returns150PerDay()
    {
        DateTime departure = new DateTime(2026, 1, 1);
        DateTime currentDate = new DateTime(2028, 1, 2);

        decimal result = calculator.GetDailyAllowance(departure, currentDate);

        Assert.Equal(150, result);
    }

    [Fact]
    public void OvernightStay_NotEmployerPaid_Returns150NightAllowance()
    {
        TravelClaimModel claim = new TravelClaimModel();

        claim.HasOvernightStay = true;
        claim.LodgingPaidByEmployer = false;
        claim.Departure = new DateTime(2026, 11, 2, 10, 0, 0);
        claim.ReturnDate = new DateTime(2026, 11, 3, 20, 0, 0);

        DailyCalculation day = new DailyCalculation();
        day.Date = new DateOnly(2026, 11, 2);

        DailyCalculation result = calculator.CalculateDay(day, claim);

        Assert.Equal(150, result.NightAllowance);
        Assert.Equal(450, result.TaxFreeAmount);
    }

    [Fact]
    public void OvernightStay_EmployerPaidLodging_ReturnsNoNightAllowance()
    {
        TravelClaimModel claim = new TravelClaimModel();

        claim.HasOvernightStay = true;
        claim.LodgingPaidByEmployer = true;
        claim.Departure = new DateTime(2026, 11, 2, 10, 0, 0);
        claim.ReturnDate = new DateTime(2026, 11, 3, 20, 0, 0);

        DailyCalculation day = new DailyCalculation();
        day.Date = new DateOnly(2026, 11, 2);

        DailyCalculation result = calculator.CalculateDay(day, claim);

        Assert.Equal(0, result.NightAllowance);
        Assert.Equal(300, result.TaxFreeAmount);
    }

    [Fact]
    public void HalfDayAfterThreeMonths_ReturnsZeroTaxFreeAllowance()
    {
        TravelClaimModel claim = new TravelClaimModel();

        claim.Departure = new DateTime(2026, 1, 1, 10, 0, 0);
        claim.ReturnDate = new DateTime(2026, 4, 2, 12, 0, 0);

        DailyCalculation day = new DailyCalculation();
        day.Date = new DateOnly(2026, 4, 2);

        DailyCalculation result = calculator.CalculateDay(day, claim);

        Assert.Equal(0, result.TaxFreeAmount);
    }

    [Fact]
    public void TripWithPaidAmountAboveAllowance_SplitsTaxFreeAndTaxableAmounts()
    {
        TravelClaimModel claim = new TravelClaimModel();

        claim.HasOvernightStay = true;
        claim.DistanceFromHome = 60;
        claim.DistanceFromWorkplace = 70;
        claim.PaidAmount = 1500;
        claim.Departure = new DateTime(2026, 11, 2, 10, 0, 0);
        claim.ReturnDate = new DateTime(2026, 11, 4, 20, 0, 0);

        calculator.CalculateTrip(claim);

        Assert.Equal(1200, claim.TotalTaxFreeAmount);
        Assert.Equal(300, claim.TaxableAmount);
    }
}

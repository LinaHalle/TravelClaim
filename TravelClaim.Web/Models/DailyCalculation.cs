namespace TravelClaim.Web.Models;

public class DailyCalculation
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public bool BreakfastPaid { get; set; }
    public bool LunchPaid { get; set; }
    public bool DinnerPaid { get; set; }

    public decimal TaxFreeAmount { get; set; }
    public decimal TaxableAmount { get; set; }

    public decimal BaseAllowance { get; set; }
    public decimal MealDeduction { get; set; }
    public decimal NightAllowance { get; set; }
}
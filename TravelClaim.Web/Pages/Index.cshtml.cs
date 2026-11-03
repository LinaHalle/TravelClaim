using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelClaim.Web.Data;
using TravelClaim.Web.Models;
using TravelClaim.Web.Services;
using TravelClaimModel = global::TravelClaim.Web.Models.TravelClaim;

namespace TravelClaim.Web.Pages;

public class IndexModel : PageModel
{
    private readonly TravelClaimContext context;
    private readonly AllowanceCalculator calculator;

    public List<TravelClaimModel> SavedClaims { get; set; } = new List<TravelClaimModel>();

    [BindProperty]
    public TravelClaimModel Claim { get; set; } = new TravelClaimModel
    {
        Departure = DateTime.Now,
        ReturnDate = DateTime.Now.AddDays(1)
    };

    public IndexModel(TravelClaimContext context, AllowanceCalculator calculator)
    {
        this.context = context;
        this.calculator = calculator;
    }

    public async Task OnGetAsync()
    {
        await LoadClaims();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Claim.ReturnDate < Claim.Departure)
        {
            ModelState.AddModelError(
                "Claim.ReturnDate",
                "Hemkomstdatum måste vara efter avresedatum.");
        }

        if (!ModelState.IsValid)
        {
            await LoadClaims();
            return Page();
        }

        List<DailyCalculation> postedDays = Claim.DailyCalculations;

        List<DailyCalculation> calculations = calculator.CalculateTrip(Claim);

        foreach (DailyCalculation day in calculations)
        {
            DailyCalculation? postedDay = postedDays.FirstOrDefault(
                x => x.Date == day.Date);

            if (postedDay != null)
            {
                day.BreakfastPaid = postedDay.BreakfastPaid;
                day.LunchPaid = postedDay.LunchPaid;
                day.DinnerPaid = postedDay.DinnerPaid;
            }

            calculator.CalculateDay(day, Claim);
        }

        Claim.DailyCalculations = calculations;

        decimal eligibleAmount = calculations.Sum(
            x => x.TaxFreeAmount);

        Claim.TotalTaxFreeAmount = Math.Min(
            Claim.PaidAmount, eligibleAmount);

        Claim.TaxableAmount = Math.Max(
            0, Claim.PaidAmount - Claim.TotalTaxFreeAmount);

        context.TravelClaims.Add(Claim);

        await context.SaveChangesAsync();

        return RedirectToPage();
    }

    private async Task LoadClaims()
    {
        SavedClaims = await context.TravelClaims
            .Include(x => x.DailyCalculations)
            .OrderByDescending(x => x.Departure)
            .ToListAsync();
    }
}

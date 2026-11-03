namespace TravelClaim.Web.Models;

public class TravelClaim 
{
    public int Id {get; set;}

    //employee info
    public string EmployeeName {get; set;} = "";
    public string PersonalNumber {get; set;} = "";
    public string HomeAddress {get; set;} = "";
    public string WorkplaceAddress {get; set;} = "";

    //Travel info
    public string Destination {get; set;} = "";
    public string Purpose {get; set;} = "";
    public DateTime Departure {get; set;}
    public DateTime ReturnDate {get; set;}
    public string LodgingAddress {get; set;} = "";

    //travel conditions
    public bool LodgingPaidByEmployer {get; set;}
    public bool HasOvernightStay {get; set;}
    public decimal DistanceFromHome {get; set;}
    public decimal DistanceFromWorkplace {get; set;}

    //Allowance amounts
    public decimal PaidAmount {get; set;}
    public decimal TotalTaxFreeAmount {get; set;}
    public decimal TaxableAmount {get; set;}

    public List<DailyCalculation> DailyCalculations {get; set;} = new List<DailyCalculation>();
}
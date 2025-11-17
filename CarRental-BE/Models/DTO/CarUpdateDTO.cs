namespace CarRental_BE.Models.DTO
{
    /*
  licensePlate 
  color 
  brand 
  model                     
  productionYear 
  numberOfSeats 
  isAutomatic             
  isGasoline             
  
  mileage 
  fuelConsumption? 
  cityProvince 
  district 
  ward
  houseNumberStreet
  description?
  additionalFunction?     
  
  carImageFront? 
  carImageBack? 
  carImageLeft? 
  carImageRight? 
  
  basePrice 
  deposit 
  termsOfUse?              
  
  registrationPaperUrl? 
  registrationPaperUrlIsVerified 
  certificateOfInspectionUrl? 
  certificateOfInspectionUrlIsVerified 
  insuranceUrl? 
  insuranceUrlIsVerified 
  
  // Status & System
  status 
  accountId 
  createdAt? 
  updatedAt? 
}
     
     */


    public class CarUpdateDTO
    {

        public string? Brand { get; set; }

        public string? Model { get; set; }

        public string? Color { get; set; }

        public long? BasePrice { get; set; }

        public long? Deposit { get; set; }

        public int? NumberOfSeats { get; set; }

        public int? ProductionYear { get; set; }

        public double? Mileage { get; set; }

        public double? FuelConsumption { get; set; }

        public bool? IsGasoline { get; set; }

        public bool? IsAutomatic { get; set; }

        public string? TermOfUse { get; set; }

        public string? AdditionalFunction { get; set; }

        public string? Description { get; set; }

        public string? LicensePlate { get; set; }

        public string? HouseNumberStreet { get; set; }

        public string? Ward { get; set; }

        public string? District { get; set; }

        public string? CityProvince { get; set; }

        public string? CarImageFront { get; set; }

        public string? CarImageBack { get; set; }

        public string? CarImageLeft { get; set; }

        public string? CarImageRight { get; set; }

        public string? InsuranceUri { get; set; }

        public bool? InsuranceUriIsVerified { get; set; }

        public string? RegistrationPaperUri { get; set; }

        public bool? RegistrationPaperUriIsVerified { get; set; }

        public string? CertificateOfInspectionUri { get; set; }

        public bool? CertificateOfInspectionUriIsVerified { get; set; }

        public string Status { get; set; } = null!;
        public List<CarPricingPlanDTO>? PricePlans { get; set; }
        public CarPricingPlanDTO? ActivePricePlan { get; set; }
    }
}

namespace CarRental_BE.Models.VO.Car
{
    public class CarVO_ViewACar

    {
        /*
            id
            brand
            model
            color
            base_price
            number_of_seats
            production_year
            car_image_front
            car_image_back
            car_image_left
            car_image_right
            status
            ward
            district
            city_province
         */

        public Guid Id { get; set; }

        public string? Brand { get; set; }

        public string? Model { get; set; }

        public string? Color { get; set; }

        public long BasePrice { get; set; }

        public int? NumberOfSeats { get; set; }

        public int? ProductionYear { get; set; }

        public string? CarImageFront { get; set; }

        public string? CarImageBack { get; set; }

        public string? CarImageLeft { get; set; }

        public string? CarImageRight { get; set; }

        public string Status { get; set; } = null!;

        public string? Ward { get; set; }

        public string? District { get; set; }

        public string? CityProvince { get; set; }
    }

}

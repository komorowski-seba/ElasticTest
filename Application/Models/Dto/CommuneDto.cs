namespace Application.Models.Dto
{
    public class CommuneDto
    {
        public string DistrictName { get; set; }
        public string CommuneName { get; set; }
        public CityDto City { get; set; }
    }
}
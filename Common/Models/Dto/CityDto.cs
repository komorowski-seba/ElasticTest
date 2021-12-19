namespace Common.Models.Dto
{
    public class CityDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public StationDto Station { get; set; }
    }
}
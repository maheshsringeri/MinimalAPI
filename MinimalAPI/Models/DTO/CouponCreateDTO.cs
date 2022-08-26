namespace MinimalAPI.Data.DTO
{
    public class CouponCreateDTO
    {
        public string Name { get; set; }

        public int Percent { get; set; }

        public bool IsActive { get; set; }

    }
}

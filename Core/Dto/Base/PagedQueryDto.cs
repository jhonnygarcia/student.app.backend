namespace Core.Dto.Base
{
    public class PagedQueryDto
    {
        public string Search { get; set; }
        public int? Page { get; set; }
        public int? PerPage { get; set; }
    }
}

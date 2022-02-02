using System.Text.Json.Serialization;

namespace WebApplication.Dto
{
    public class PagedListResDto<T>
    {
        [JsonPropertyName("data")]
        public T[] Data { get; set; }
        [JsonPropertyName("totalElements")]
        public int TotalElements { get; set; }
    }
}

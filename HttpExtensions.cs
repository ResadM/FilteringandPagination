using System.Text.Json;

namespace FilteringandPagination
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, MetaData metadata)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            response.Headers.Add("Pagination", JsonSerializer.Serialize(metadata, options));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}

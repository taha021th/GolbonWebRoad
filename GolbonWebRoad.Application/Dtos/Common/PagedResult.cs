namespace GolbonWebRoad.Application.Dtos.Common
{
    public class PagedResult<T>
    {
        public List<T> Data { get; set; }
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}

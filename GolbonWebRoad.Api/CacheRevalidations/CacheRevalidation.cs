namespace GolbonWebRoad.Api.CacheRevalidations
{
    public class CacheRevalidation
    {
        public static CancellationTokenSource ProductTokenSource { get; private set; } = new CancellationTokenSource();
        public static CancellationTokenSource CategoryTokenSource { get; private set; } = new CancellationTokenSource();

        public static void RevalidateProductAndCategoryCache()
        {
            ProductTokenSource.Cancel();
            ProductTokenSource.Dispose();
            ProductTokenSource=new CancellationTokenSource();
            CategoryTokenSource.Cancel();
            CategoryTokenSource.Dispose();
            CategoryTokenSource=new CancellationTokenSource();
        }
    }
}

//using GolbonWebRoad.Application.Interfaces.Services.Storage;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Options;

//namespace GolbonWebRoad.Infrastructure.Services.Storage
//{
//    public class StorageService : IStorageService
//    {
//        private readonly IEnumerable<IStorageProvider> _providers;
//        private readonly StorageOptions _options;
//        public StorageService(IEnumerable<IStorageProvider> providers, IOptions<StorageOptions> options)
//        {
//            _providers=providers;
//            _options=options.Value;
//        }
//        public Task<string> UploadFileAsync(IFormFile file, string subDirectory)
//        {
//            return UploadFileAsync(file, _options.DefaultProvider, subDirectory);
//        }

//        public Task<string> UploadFileAsync(IFormFile file, string providername, string subDirectory)
//        {
//            if (!_options.Providers.TryGetValue(providername, out var providerOptions))
//            {
//                throw new InvalidOperationException($"سرویس دهنده با نام '{providername}' یافت نشد.");
//            }
//            var provider = _providers.FirstOrDefault(p =>
//            p.providerType
//            );

//        }
//    }
//}

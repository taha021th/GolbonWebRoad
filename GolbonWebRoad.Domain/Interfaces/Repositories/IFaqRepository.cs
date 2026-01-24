namespace GolbonWebRoad.Domain.Interfaces.Repositories
{
    public interface IFaqRepository
    {
        Task<ICollection<GolbonWebRoad.Domain.Entities.Faq>> GetAllAsync(bool onlyActive = false);
        Task<GolbonWebRoad.Domain.Entities.Faq?> GetByIdAsync(int id);
        Task<GolbonWebRoad.Domain.Entities.Faq?> GetBySlogAsync(string slog);
        Task AddAsync(GolbonWebRoad.Domain.Entities.Faq faq);
        void Update(GolbonWebRoad.Domain.Entities.Faq faq);
        Task DeleteAsync(int id);
    }
}

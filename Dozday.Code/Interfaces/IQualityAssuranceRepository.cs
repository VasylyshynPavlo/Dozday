using Dozday.Core.Models;

namespace Dozday.Core.Interfaces;

public interface IQualityAssuranceRepository
{
    Task<IEnumerable<QualityAssurance>> GetAllAsync();
    Task<QualityAssurance?> GetByIdAsync(Guid id);
    Task AddAsync(QualityAssurance qualityAssurance);
    Task UpdateAsync(QualityAssurance qualityAssurance);
    Task DeleteAsync(Guid id);
}
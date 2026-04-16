using Dozday.Core.Models;

namespace Dozday.Core.Interfaces;

public interface IQualityAssuranceService
{
    Task<IEnumerable<QualityAssurance>> GetAllAsync();
    Task<QualityAssurance?> GetByIdAsync(string id);
    Task AddAsync(QualityAssurance qualityAssurance);
    Task UpdateAsync(QualityAssurance qualityAssurance);
    Task DeleteAsync(string id);
}
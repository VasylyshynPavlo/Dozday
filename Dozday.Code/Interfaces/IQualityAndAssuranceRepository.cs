using Dozday.Core.Models;

namespace Dozday.Core.Interfaces;

public interface IQualityAndAssuranceRepository
{
    Task<IEnumerable<QualityAndAssurance>> GetAllAsync();
    Task<QualityAndAssurance?> GetByIdAsync(Guid id);
    Task AddAsync(QualityAndAssurance qualityAssurance);
    Task<IEnumerable<QualityAndAssurance>> SearchAsync(string text);
    Task UpdateAsync(QualityAndAssurance qualityAssurance);
    Task DeleteAsync(Guid id);
}
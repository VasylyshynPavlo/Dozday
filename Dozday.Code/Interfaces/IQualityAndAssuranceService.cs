using Dozday.Core.Models;

namespace Dozday.Core.Interfaces;

public interface IQualityAndAssuranceService
{
    Task<IEnumerable<QualityAndAssurance>> GetAllAsync();
    Task<QualityAndAssurance?> GetByIdAsync(string id);
    Task AddAsync(QualityAndAssurance qualityAssurance);
    Task UpdateAsync(QualityAndAssurance qualityAssurance);
    Task<IEnumerable<QualityAndAssurance>> SearchAsync(string text);
    Task DeleteAsync(string id);
}
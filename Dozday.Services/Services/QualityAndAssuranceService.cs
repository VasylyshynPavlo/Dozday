using Dozday.Core.Interfaces;
using Dozday.Core.Models;

namespace Dozday.Services.Services;

public class QualityAndAssuranceService(IQualityAndAssuranceRepository repository) : IQualityAndAssuranceService
{
    private readonly IQualityAndAssuranceRepository _repository = repository;

    public async Task AddAsync(QualityAndAssurance qualityAssurance)
    {
        await _repository.AddAsync(qualityAssurance);
    }

    public async Task DeleteAsync(string id)
    {
        await _repository.DeleteAsync(Guid.Parse(id));
    }

    public async Task<IEnumerable<QualityAndAssurance>> SearchAsync(string text)
    {
        return await _repository.SearchAsync(text);
    }

    public async Task<IEnumerable<QualityAndAssurance>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<QualityAndAssurance?> GetByIdAsync(string id)
    {
        return await _repository.GetByIdAsync(Guid.Parse(id));
    }

    public async Task UpdateAsync(QualityAndAssurance qualityAssurance)
    {
        await _repository.UpdateAsync(qualityAssurance);
    }
}
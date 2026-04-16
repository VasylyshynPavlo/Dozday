using Dozday.Core.Interfaces;
using Dozday.Core.Models;

namespace Dozday.Services.Services;

public class QualityAssuranceService(IQualityAssuranceRepository repository) : IQualityAssuranceService
{
    private readonly IQualityAssuranceRepository _repository = repository;

    public async Task AddAsync(QualityAssurance qualityAssurance)
    {
        await _repository.AddAsync(qualityAssurance);
    }

    public async Task DeleteAsync(string id)
    {
        await _repository.DeleteAsync(Guid.Parse(id));
    }

    public async Task<IEnumerable<QualityAssurance>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<QualityAssurance?> GetByIdAsync(string id)
    {
        return await _repository.GetByIdAsync(Guid.Parse(id));
    }

    public async Task UpdateAsync(QualityAssurance qualityAssurance)
    {
        await _repository.UpdateAsync(qualityAssurance);
    }
}
using Dozday.Core.Interfaces;
using Dozday.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Dozday.Data.Repositories;

public class QualityAssuranceRepository(DozdayDbContext context) : IQualityAssuranceRepository
{
    private readonly DozdayDbContext _context = context;

    public async Task AddAsync(QualityAssurance qualityAssurance)
    {
        await _context.QualityAssurances.AddAsync(qualityAssurance);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _context.QualityAssurances.Where(q => q.Id == id).ExecuteDeleteAsync();
    }

    public async Task<IEnumerable<QualityAssurance>> GetAllAsync()
    {
        return await _context.QualityAssurances.ToListAsync();
    }

    public async Task<QualityAssurance?> GetByIdAsync(Guid id)
    {
        return await _context.QualityAssurances.Where(q => q.Id == id).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(QualityAssurance qualityAssurance)
    {
        await _context.QualityAssurances.Where(q => q.Id == qualityAssurance.Id).ExecuteUpdateAsync(q => q
            .SetProperty(q => q.Tittle, qualityAssurance.Tittle)
            .SetProperty(q => q.AssuranceMarkdown, qualityAssurance.AssuranceMarkdown));
    }
}
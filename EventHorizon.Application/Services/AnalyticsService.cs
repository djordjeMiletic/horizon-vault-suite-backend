using EventHorizon.Application.Interfaces;
using EventHorizon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Application.Services;

public class AnalyticsService
{
    private readonly IUnitOfWork _unitOfWork;

    public AnalyticsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Dictionary<string, decimal>> GetSeriesAsync(string range, string? advisorEmail = null)
    {
        var months = GetMonthsForRange(range);
        var advisorEmails = string.IsNullOrEmpty(advisorEmail) 
            ? null 
            : advisorEmail.Split(',').Select(e => e.Trim()).ToList();

        var query = _unitOfWork.Repository<Payment>().Query();

        if (advisorEmails != null)
            query = query.Where(p => advisorEmails.Contains(p.AdvisorEmail));

        var payments = await query
            .Where(p => months.Contains(p.Date.ToString("yyyy-MM")))
            .ToListAsync();

        var result = new Dictionary<string, decimal>();
        foreach (var month in months)
        {
            var monthPayments = payments.Where(p => p.Date.ToString("yyyy-MM") == month);
            result[month] = monthPayments.Sum(p => p.APE);
        }

        return result;
    }

    public async Task<SeriesPoint[]> GetSeriesArrayAsync(string range, string? advisorEmail = null)
    {
        var seriesDict = await GetSeriesAsync(range, advisorEmail);
        return seriesDict.Select(kvp => new SeriesPoint(kvp.Key, kvp.Value)).ToArray();
    }

    public async Task<Dictionary<string, decimal>> GetProductMixAsync(string range, string? advisorEmail = null)
    {
        var months = GetMonthsForRange(range);
        var advisorEmails = string.IsNullOrEmpty(advisorEmail) 
            ? null 
            : advisorEmail.Split(',').Select(e => e.Trim()).ToList();

        var query = _unitOfWork.Repository<Payment>().Query().Include(p => p.Product);

        if (advisorEmails != null)
            query = query.Where(p => advisorEmails.Contains(p.AdvisorEmail));

        var payments = await query
            .Where(p => months.Contains(p.Date.ToString("yyyy-MM")))
            .ToListAsync();

        return payments
            .GroupBy(p => p.Product.ProductName)
            .ToDictionary(g => g.Key, g => g.Sum(p => p.APE));
    }

    public async Task<ProductMixItem[]> GetProductMixArrayAsync(string range, string? advisorEmail = null)
    {
        var productMixDict = await GetProductMixAsync(range, advisorEmail);
        return productMixDict.Select(kvp => new ProductMixItem(kvp.Key, kvp.Value)).ToArray();
    }

    private List<string> GetMonthsForRange(string range)
    {
        var now = DateTime.Now;
        var months = new List<string>();

        var monthCount = range switch
        {
            "last3" => 3,
            "last6" => 6,
            "ytd" => now.Month,
            _ => 6
        };

        for (int i = monthCount - 1; i >= 0; i--)
        {
            var month = now.AddMonths(-i);
            months.Add(month.ToString("yyyy-MM"));
        }

        return months;
    }
}
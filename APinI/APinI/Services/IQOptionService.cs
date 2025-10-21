using System;
using APinI.Models;
using APinI.Repository;

namespace APinI.Services;

public class IQOptionService : IIQOptionService
{
    private readonly PinDataRepository _pinDataRepository = new PinDataRepository();
    public void AddCandles(List<IQOptionCandle> candles)
    {
        _pinDataRepository.SaveCandles(candles);
    }

    public void ValidateCandles(List<IQOptionCandle> candles)
    {
        if (candles == null || !candles.Any())
        {
            throw new ArgumentException("Candle list cannot be null or empty.");
        }
        var groupedCandles = candles.GroupBy(c => c.Id);
        candles = groupedCandles.Select(g => g.LastOrDefault() ?? new IQOptionCandle()).Where(c => c.Id != 0).ToList();
    }
}

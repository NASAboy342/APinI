using System;
using APinI.Models;

namespace APinI.Services;

public interface IIQOptionService
{
    void AddCandles(List<IQOptionCandle> candles);
    void ValidateCandles(List<IQOptionCandle> candles);
}

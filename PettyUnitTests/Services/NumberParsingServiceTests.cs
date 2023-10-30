using Moq;
using Petty;
using Petty.Services.Local;
using Petty.Services.Local.Localization;
using SpeechEngine.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PettyUnitTests.Services;

public class NumberParsingServiceTests
{
    static NumberParsingServiceTests()
    {
        var services = new ServiceCollection();
        services.AddSingleton<LoggerService>();
        MauiProgram.ServiceProvider = new CustomServiceProvider();
        _localizationService = new Mock<LocalizationService>(null);
    }

    //private static readonly NumberParsingService _numberParsingService;
    private static readonly Mock<LocalizationService> _localizationService;

    [Theory]
    [InlineData("ru-Ru", "ноль", 0, 3)]
    [InlineData("ru-Ru", "девять", 9, 3)]
    [InlineData("ru-Ru", "девятнадцать", 19, 3)]
    [InlineData("ru-Ru", "девяносто", 90, 3)]
    [InlineData("ru-Ru", "девяносто восемь", 98, 3)]
    [InlineData("ru-Ru", "девятьсот", 900, 3)]
    [InlineData("ru-Ru", "девятьсот восемьдесят семь", 987, 3)]
    [InlineData("ru-Ru", "восемь девяносто девять девятьсот девяносто девять триста сорок пять девяносто восемь", 89999934598, 1)]
    [InlineData("en-Us", "zero", 0, 3)]
    [InlineData("en-Us", "nine", 9, 3)]
    [InlineData("en-Us", "nineteen", 19, 3)]
    [InlineData("en-Us", "ninety", 90, 3)]
    [InlineData("en-Us", "ninety eight", 98, 3)]
    [InlineData("en-Us", "nine hundred", 900, 3)]
    [InlineData("en-Us", "nine hundred eighty seven", 987, 3)]
    [InlineData("en-Us", "eight ninety nine nine hundred ninety nine three hundred forty five ninety eight", 89999934598, 1)]
    public void ParseNumbers_OneNumber_ReturnsPointMark(
        string lang,
        string strNumber, 
        long expectedNumber,
        int repeatStrNumberCount)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
        var numberParsingService = new NumberParsingService();
        var strExpectedNumber = "";
        var strNumberWithRepeat = "";

        for (var i = 0; i < repeatStrNumberCount; i++)
            strExpectedNumber += expectedNumber.ToString();

        for (var i = 0; i < repeatStrNumberCount; i++)
            strNumberWithRepeat += " " + strNumber;

        var numbers = numberParsingService.ParseNumbers(strNumberWithRepeat.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(long.Parse(strExpectedNumber), long.Parse(string.Join(null, numbers)));
    }

    private class CustomServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType)
        {
            return null;
        }
    }
}

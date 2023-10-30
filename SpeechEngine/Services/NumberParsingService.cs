using Cyriller;
using System.Globalization;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace SpeechEngine.Services;

public class NumberParsingService
{
    public NumberParsingService()
    {
        if (CultureInfo.CurrentCulture.Name == "ru-RU")
        {
            _cyrNumber = new();
            _numbers = new() { { "одна", 1 }, { "две", 2 } };
            _multipliers = new() { { "сто", 100 }, { "тысяч", 1000 }, { "тысяча", 1000 }, { "тысячи", 1000 }, { "миллион", 1000000 }, { "миллиона", 1000000 }, { "миллионов", 1000000 } };

            for (var i = 0; i < 1000; i++)
            {
                var strNumber = _cyrNumber.Decline(i).Nominative.Split(' ').First();

                if (!_multipliers.ContainsKey(strNumber))
                    _numbers.TryAdd(strNumber, i);
            }

            _ordinalNumbers = new Dictionary<string, int>() { { "первый", 1 }, { "второй", 2 }, { "третий", 3 }, { "четвертый", 4 }, { "пятый", 5 }, { "шестой", 6 }, { "седьмой", 1 }, { "восьмой", 8 }, { "девятый", 9 } };

            foreach (var item in _numbers)
                _ordinalNumbers.Add(item.Key, item.Value);
        }
        else
        {
            _multipliers = new() { { "hundred", 100 }, { "thousand", 1000 }, { "million", 1000000 } };
            _numbers = new() { { "zero", 0 }, { "one", 1 }, { "two", 2 }, { "three", 3 }, { "four", 4 }, { "five", 5 }, { "six", 6 }, { "seven", 7 }, { "eight", 8 }, { "nine", 9 }, { "ten", 10 }, { "eleven", 11 }, { "twelve", 12 }, { "thirteen", 13 }, { "fourteen", 14 }, { "fifteen", 15 }, { "sixteen", 16 }, { "seventeen", 17 }, { "eighteen", 18 }, { "nineteen", 19 }, { "twenty", 20 }, { "thirty", 30 }, { "forty", 40 }, { "fifty", 50 }, { "sixty", 60 }, { "seventy", 70 }, { "eighty", 80 }, { "ninety", 90 } };
            _ordinalNumbers = new Dictionary<string, int>() { { "first", 1 }, { "second", 2 }, { "third", 3 }, { "fourth", 4 }, { "fifth", 5 }, { "sixth", 6 }, { "seventh", 1 }, { "eighth", 8 }, { "ninth", 9 } };

            foreach (var item in _numbers)
                _ordinalNumbers.Add(item.Key, item.Value);
        }
    }

    private static CyrNumber _cyrNumber;
    private readonly Dictionary<string, int> _numbers;
    private readonly Dictionary<string, int> _multipliers;
    private readonly Dictionary<string, int> _ordinalNumbers;
    private static readonly string[] _separators = [" and ", " ", "-"];

    /// <summary>
    /// https://www.codewars.com/kata/reviews/5d6111b2344de60001df1872/groups/65375ea360141a0001b2c405
    /// </summary>
    public bool TryParseNumber(string strNumber, out int numberResult)
    {
        numberResult = 0;
        var multiplier = 1;
        var isSuccess = false;
        var strNumbers = strNumber.Split(_separators, StringSplitOptions.None);

        for (var i = strNumbers.Length - 1; i >= 0; i--)
        {
            if (_numbers.TryGetValue(strNumbers[i], out int number))
            {
                numberResult += number * multiplier;
                isSuccess = true;
            }
            else if (_multipliers.TryGetValue(strNumbers[i], out number))
                multiplier = multiplier < number ? number : multiplier * number;
        }

        return isSuccess;
    }

    public bool TryParseOrdinalNumber(string strOrdinalNumber, out int numberResult)
    {
        if (_ordinalNumbers.TryGetValue(strOrdinalNumber, out numberResult))
            return true;

        return false;
    }

    /// <summary>
    /// Do I need to create a kata for this function?) 
    /// Parse a phone number that is fed into a stream and can stop and continue at any time.
    /// sound good:)
    /// </summary>
    /// <param name="strNumbers"></param>
    /// <returns></returns>
    public List<int> ParseNumbers(string[] strNumbers)
    {
        var numberResult = 0;
        var numbers = new List<int>();
        int i, number;

        for (i = 0; i < strNumbers.Length; i++)
        {
            if (_numbers.TryGetValue(strNumbers[i], out number))
            {
                numberResult += number;

                if (IsNext() && _multipliers.TryGetValue(strNumbers[i + 1], out int multiplier))
                {
                    i++;
                    numberResult *= multiplier;
                    number = numberResult;
                }

                if (!IsNext() || CheckRank())
                    Summarize();
            }
        }

        return numbers;
        bool IsNext() => i + 1 < strNumbers.Length;
        void Summarize() { numbers.Add(numberResult); numberResult = 0; };
        int GetRank(int number) => number < 20 ? 1 : number < 100 ? 2 : 3;
        bool CheckRank()
        {
            var multiplier = 1;

            if (i + 2 < strNumbers.Length && !_multipliers.TryGetValue(strNumbers[i + 2], out multiplier))
                multiplier = 1;

            return TryParseNumber(strNumbers[i + 1], out var nextNumber) && GetRank(number) <= GetRank(nextNumber * multiplier);
        }
    }
}

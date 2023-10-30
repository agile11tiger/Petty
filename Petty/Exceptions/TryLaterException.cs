using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SpeechEngine.Speech.Exceptions;

public class TryLaterException(string message) : Exception(message)
{
}

using System.Collections;
using System.Diagnostics;
using System.Linq;
namespace SpeechEngine.Speech;

/// <summary>
/// Add editor commands to speech. An index will appear above each word.
/// </summary>
public class SpeechWords : IEnumerable<string>, IEnumerator<string>
{
    private int _position = -1;
    private readonly List<string> _speech = [];
    private readonly List<string> _partialSpeech = [];

    public int Length => _speech.Count + _partialSpeech.Count;
    public string Current => _position == -1 || _position >= Length ? throw new ArgumentException() : this[_position];
    object IEnumerator.Current => Current;
    public string this[int index] 
    { 
        get => index < _speech.Count ? _speech[index] : _partialSpeech[index - _speech.Count];
        set 
        {
            if (index < _speech.Count)
                _speech[index] = value;
            else
                _partialSpeech[index - _speech.Count] = value;
        }
    }

    public void Add(SpeechRecognizerResult speechResult)
    {
        Debug.WriteLine(speechResult.ToString());
        _partialSpeech.Clear();
        //prevents merging of collections.
        //For example, a punctuation mark is attached to the previous word
        //which may be from another collection, which is unacceptable, since there will be repetitions.
        _partialSpeech.Add(string.Empty); 

        if (speechResult.IsPartialSpeech)
            _partialSpeech.AddRange(speechResult.Speech.Split(' '));

        //resultSpeech == finalSpeech 
        //Depends on how to call either because of silence or manually because of the point.
        if (speechResult.IsResultSpeech || speechResult.IsFinalSpeech)
            _speech.AddRange(speechResult.Speech.Split(' ')); // add to resultSpeech
    }

    public void Clear()
    {
        _speech.Clear();
        _partialSpeech.Clear();
    }

    public bool MoveNext()
    {
        if (_position < Length - 1)
        {
            _position++;
            return true;
        }
        else
            return false;
    }

    public void Dispose() { }
    public void Reset() => _position = -1;
    IEnumerator IEnumerable.GetEnumerator() => this;
    public IEnumerator<string> GetEnumerator() => this;
    public override string ToString() => string.Join(' ', this);
}

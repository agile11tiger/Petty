namespace Petty.Views.Controls.Magic.CssParser;

public class CssReader
{
    public CssReader(string css)
    {
        _tokens = css.Replace("\r\n", "").Split(_separators, StringSplitOptions.RemoveEmptyEntries);
    }

    public CssReader(string css, params char[] separator)
    {
        _tokens = css.Split(separator, StringSplitOptions.RemoveEmptyEntries);
    }

    private int _cursor;
    private readonly string[] _tokens;
    private readonly static char[] _separators = ['(', ')', ','];
    public bool CanRead => _cursor < _tokens.Length;
    public string Read() => _tokens[_cursor];
    public void MoveNext() => _cursor++;
    public void Rollback() => _cursor--;

    public string ReadNext()
    {
        MoveNext();

        if (!CanRead)
            return string.Empty;

        return Read();
    }
}
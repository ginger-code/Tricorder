namespace Tricorder;

public readonly struct ControlCharacters
{
    public char FieldSeparator { get; } = '|';
    public char ComponentSeparator { get; } = '^';
    public char RepetitionSeparator { get; } = '~';
    public char EscapeCharacter { get; } = '\\';
    public char SubcomponentSeparator { get; } = '&';

    public ControlCharacters()
    {
        FieldSeparator = '|';
        ComponentSeparator = '^';
        RepetitionSeparator = '~';
        EscapeCharacter = '\\';
        SubcomponentSeparator = '&';
    }
    public ControlCharacters(
        char fieldSeparator = '|',
        char componentSeparator = '^',
        char repetitionSeparator = '~',
        char escapeCharacter = '\\',
        char subcomponentSeparator = '&'
    )
    {
        FieldSeparator = fieldSeparator;
        ComponentSeparator = componentSeparator;
        RepetitionSeparator = repetitionSeparator;
        EscapeCharacter = escapeCharacter;
        SubcomponentSeparator = subcomponentSeparator;
    }
}
using System.Text.RegularExpressions;

namespace Tricorder;

public record HL7Path
{
    internal string MessageType { get; }
    internal int? FieldIndex { get; }
    internal int? ComponentIndex { get; }

    public HL7Path(string hl7Path)
    {
        if (string.IsNullOrEmpty(hl7Path))
        {
            throw new Exception("HL7 paths must not be null or empty.");
        }

        var match = Regex.Match(hl7Path, @"^(?<MessageType>\w{3})(\.(?<FieldIndex>\d+))?(\.(?<ComponentIndex>\d+))?$");
        if (!match.Success)
            throw new Exception("HL7 must conform to pattern: ^\\w{3}(\\.\\d+)?(\\.\\d+)?$");
        MessageType = match.Groups["MessageType"].Value;
        FieldIndex = match.Groups["FieldIndex"].Success ? int.Parse(match.Groups["FieldIndex"].Value) : null;
        ComponentIndex = match.Groups["ComponentIndex"].Success ? int.Parse(match.Groups["ComponentIndex"].Value) : null;
    }

    public static implicit operator HL7Path(string str) => new (str);
}
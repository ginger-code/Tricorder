namespace Tricorder;

public static class HL7
{
    public static async Task<IEnumerable<string>> CollectValues(string hl7Path, string hl7FileDirectory, ControlCharacters? controlCharacters = null) 
        => await CollectValues(new HL7Path(hl7Path), hl7FileDirectory, controlCharacters);

    public static async Task<IEnumerable<string>> CollectValues(HL7Path hl7Path, string hl7FileDirectory, ControlCharacters? controlCharacters = null) 
        => (
                await Task.WhenAll(
                    Directory.EnumerateFiles(hl7FileDirectory, "*.hl7")
                        .Select(async path
                            => await File.ReadAllTextAsync(path))
                        .Select(async message
                            => await Task.Run(async () => QueryMessage(hl7Path, await message, controlCharacters))
                        )
                )
            )
            .SelectMany(x => x);

    public static async Task<IEnumerable<string>> CollectValues(string hl7Path, IEnumerable<string> hl7Messages, ControlCharacters? controlCharacters = null)
        => await CollectValues(new HL7Path(hl7Path), hl7Messages, controlCharacters);

    public static async Task<IEnumerable<string>> CollectValues(HL7Path hl7Path, IEnumerable<string> hl7Messages, ControlCharacters? controlCharacters = null)
        => (await Task.WhenAll(hl7Messages.Select(message => Task.Run(() => QueryMessage(hl7Path, message, controlCharacters)))))
            .SelectMany(x => x);

    public static IEnumerable<string> QueryMessage(string path, string hl7Message, ControlCharacters? controlCharacters = null) 
        => QueryMessage(new HL7Path(path), hl7Message, controlCharacters);

    public static IEnumerable<string> QueryMessage(HL7Path path, string hl7Message, ControlCharacters? controlCharacters = null)
    {
        if (controlCharacters is null)
        {
            var trimmedMessage = hl7Message.Trim();
            controlCharacters = trimmedMessage.StartsWith("MSH")
                ? new ControlCharacters(trimmedMessage[3], trimmedMessage[4], trimmedMessage[5], trimmedMessage[6], trimmedMessage[7])
                : new ControlCharacters();
        }

        var controls = (ControlCharacters)controlCharacters;

        foreach (var segment in hl7Message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (string.IsNullOrEmpty(segment) || !segment.StartsWith(path.MessageType))
                continue;
            if (path.FieldIndex is null)
                yield return segment;
            var fields = segment.Split(controls.FieldSeparator);
            if (path.FieldIndex < fields.Length)
            {
                if (path.ComponentIndex is null)
                {
                    yield return fields[path.FieldIndex!.Value];
                }

                var components = fields[path.FieldIndex!.Value].Split(controls.ComponentSeparator);
                if (path.ComponentIndex - 1 < components.Length)
                {
                    yield return components[path.ComponentIndex!.Value - 1];
                }
            }
        }
    }
}
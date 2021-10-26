using System.Collections.Concurrent;
using System.Threading.Channels;
using HL7lite;
using Open.ChannelExtensions;

namespace Tricorder;

public static class HL7
{
    public static async Task<IDictionary<string, int>> CollectValues(string searchPattern, string searchPath, CancellationToken cancellationToken = default)
    {
        var maxConcurrency = Environment.ProcessorCount;
        HL7Path pattern = searchPattern;
        ConcurrentDictionary<string, int> dict = new();
        await Channel.CreateUnbounded<string>()
            .Source(Directory.EnumerateFiles(searchPath, "*.hl7"), cancellationToken)
            .PipeAsync(maxConcurrency, PathToMessageTransform, cancellationToken: cancellationToken)
            .Filter(message => message is not null)
            .Pipe(maxConcurrency, CollectValueTransform!, cancellationToken: cancellationToken)
            .ReadAllConcurrently(maxConcurrency, newDict =>
            {
                foreach (var key in newDict.Keys)
                {
                    if (dict.ContainsKey(key))
                    {
                        dict[key] += newDict[key];
                    }
                    else
                    {
                        dict[key] = newDict[key];
                    }
                }
            }, cancellationToken);
        return dict;

        async ValueTask<Message?> PathToMessageTransform(string path)
        {
            try
            {
                Message message = new(await File.ReadAllTextAsync(path, cancellationToken));
                message.ParseMessage(false);
                return message;
            }
            catch
            {
                return null;
            }
        }

        Dictionary<string, int> CollectValueTransform(Message message)
        {
            var newDict = new Dictionary<string, int>();
            if (pattern.FieldIndex is null)
            {
                try
                {
                    foreach (var segment in message.Segments(pattern.MessageType))
                    {
                        if (newDict.ContainsKey(segment.Value))
                        {
                            newDict[segment.Value]++;
                        }
                        else
                        {
                            newDict[segment.Value] = 1;
                        }
                    }
                }
                catch
                {
                }

                return newDict;
            }

            if (pattern.ComponentIndex is null)
            {
                try
                {
                    foreach (var segment in message.Segments(pattern.MessageType))
                    {
                        var field = segment.Fields(pattern.FieldIndex.Value);
                        if (field is null)
                        {
                            continue;
                        }

                        if (newDict.ContainsKey(field.Value))
                        {
                            newDict[field.Value]++;
                        }
                        else
                        {
                            newDict[field.Value] = 1;
                        }
                    }
                }
                catch
                {
                }

                return newDict;
            }

            try
            {
                foreach (var segment in message.Segments(pattern.MessageType))
                {
                    var field = segment.Fields(pattern.FieldIndex.Value);
                    if (field is null)
                    {
                        continue;
                    }

                    var component = field.Components(pattern.ComponentIndex.Value);

                    if (component is null)
                    {
                        continue;
                    }

                    if (newDict.ContainsKey(component.Value))
                    {
                        newDict[component.Value]++;
                    }
                    else
                    {
                        newDict[component.Value] = 1;
                    }
                }
            }
            catch
            {
            }

            return newDict;
        }
    }
}
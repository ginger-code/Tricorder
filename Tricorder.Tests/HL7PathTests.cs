using NUnit.Framework;
using Tricorder;

namespace Tricorder.Tests;

public class HL7PathTests
{
    [Test]
    public void HL7PathsParse()
    {
        const string hl7pathFull = "PV1.1.2";
        const string hl7pathPartial = "PV1.1";
        const string hl7pathShort = "PV1";

        HL7Path shortPath = new(hl7pathShort);
        Assert.NotNull(shortPath);
        Assert.AreEqual("PV1", shortPath.MessageType);
        Assert.Null(shortPath.FieldIndex);
        Assert.Null(shortPath.ComponentIndex);
        HL7Path partialPath = hl7pathPartial;
        Assert.NotNull(partialPath);
        Assert.AreEqual("PV1", partialPath.MessageType);
        Assert.AreEqual(1, partialPath.FieldIndex);
        Assert.Null(partialPath.ComponentIndex);
        HL7Path fullPath = new (hl7pathFull);
        Assert.NotNull(fullPath);
        Assert.AreEqual("PV1", fullPath.MessageType);
        Assert.AreEqual(1, fullPath.FieldIndex);
        Assert.AreEqual(2, fullPath.ComponentIndex);
    }
}
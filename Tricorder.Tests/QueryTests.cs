using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Tricorder.Tests;

public class QueryTests
{
    [Test]
    public void Queries_PID_5_1()
    {
        HL7Path path = "PID.5.1";
        var results = HL7.QueryMessage(path, ORU).ToList();
        Assert.NotNull(results);
        Assert.IsNotEmpty(results);
        Assert.AreEqual(1, results.Count);
        Assert.AreEqual("AAAAAAAA", results.First());
    }

    [Test]
    public async Task Collects_PID_5_1()
    {
        var results = (await HL7.CollectValues("PID.5.1", new[] { ORU, ORU })).ToList();
        Assert.NotNull(results);
        Assert.IsNotEmpty(results);
        Assert.AreEqual(2, results.Count);
        Assert.AreEqual("AAAAAAAA", results.First());
    }
    
    [Test]
    public async Task Collects_PID_5_1_From_Disk()
    {
        var results = (await HL7.CollectValues("PID.5.1", "test_files")).ToList();
        Assert.NotNull(results);
        Assert.IsNotEmpty(results);
        Assert.AreEqual(1, results.Count);
        Assert.AreEqual("AAAAAAAA", results.First());
    }

    private const string ORU = @"MSH|^~\&|LinkLogic-2149|2149001^BMGPED|CHIRPS-Out|BMGPED|20060915210000||ORU^R01 |1473973200100600|P|2.3|||NE|NE
PID|1||00000-0000000|000000|AAAAAAAA^AAAAAA^A||00000000|M||U|00000 A AA AA AAA^^AAAAAA^AA^00000 ||(000)000-0000|||S|||000-00-0000
PV1|1|O|^^^BMGPED||||dszczepaniak
OBR|1|||5^Preload|||20060915095920|||||||||donaldduck||ZZ
OBX|1|ST|CPT-90707.2^MMR #2||given||||||R|||20040506095950
OBX|2|ST|CPT-90737.4^HEMINFB#4||given||||||R|||19931103100050
OBX|3|ST|CPT-90707.1^MMR #1||given||||||R|||19931103095950
OBX|4|ST|CPT-90731.3^HEPBVAX#3||given||||||R|||19930712100120
OBX|5|ST|CPT-90731.2^HEPBVAX#2||given||||||R|||19930112100120
OBX|6|ST|CPT-90737.3^HEMINFB#3||given||||||R|||19930112100050
OBX|7|ST|CPT-90731.1^HEPBVAX#1||given||||||R|||19921027100120
OBX|8|ST|CPT-90737.2^HEMINFB#2||given||||||R|||19921027100050
OBX|9|ST|CPT-90737.1^HEMINFB#1||given||||||R|||19920826100050";
}
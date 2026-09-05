namespace Sonic4Episode1.Tests;

// readAMAFile resolves node and act offsets through two static dictionaries, so two classes
// parsing archives at once corrupt each other's tables
[CollectionDefinition(Name)]
public class AmaReaderCollection
{
    public const string Name = "ama readers";
}

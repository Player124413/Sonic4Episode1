namespace Sonic4Episode1.Tests;

// amFsLoadVariants swaps a process-wide static, so the classes that call it must not run in
// parallel with each other
[CollectionDefinition(Name)]
public class FsVariantCollection
{
    public const string Name = "fs variants";
}

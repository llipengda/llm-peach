var pid = Environment.ProcessId;

Console.Error.WriteLine($"=={pid}==ERROR: AddressSanitizer: heap-use-after-free on address 0x61400000fe44 at pc 0x4008b9 bp 0x7fffffffd000 sp 0x7fffffffcff8");
Console.Error.WriteLine("READ of size 4 at 0x61400000fe44 thread T0");
Console.Error.WriteLine("    #0 0x4008b8 in main UseAfterFree.c:18");
Console.Error.WriteLine("Shadow bytes around the buggy address:");
Console.Error.WriteLine("  0x0c287fff9f70: fa fa fa fa fd fd fd fd fd fd fd fd fd fd fd fd");
Console.Error.WriteLine($"=={pid}==ABORTING");

return 1;

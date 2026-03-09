using GBX.NET.Tool.CLI;
#if (EnableZlib)
using GBX.NET.ZLib;
#endif
using MyGbxToolApp;

#if (EnableZlib)
Gbx.ZLib = new ZLib();
#endif

await ToolConsole<MyGbxToolAppTool>.RunAsync(args, new()
{
    // .. JsonContext etc.
});
using EternalFS.Library.Filesystem;
using EternalFS.Library.Terminal;

VirtualEternalFileSystem fs = new("MyEternalFS", 36000);
fs.Mount();

TerminalRunner runner = new(fs);
runner.Run();

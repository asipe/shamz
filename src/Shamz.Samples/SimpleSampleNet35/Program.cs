using System;
using System.Diagnostics;
using Shamz.Core;

namespace SimpleSampleNet35 {
  internal class Program {
    private static void Main() {
      var exe = ShamzFactory.CreateShamzExe("sample.exe");

      exe
        .Setup(invocation => invocation
                               .WhenCommandLine("abc")
                               .ThenReturn(1000))
        .Setup(invocation => invocation
                               .WhenCommandLine("def")
                               .ThenReturn(0))
        .Initialize();

      using (var process = Process.Start("sample.exe", "abc")) {
        process.WaitForExit();
        Console.WriteLine("'sample.exe abc': ExitCode = {0}", process.ExitCode);
      }

      using (var process = Process.Start("sample.exe", "def")) {
        process.WaitForExit();
        Console.WriteLine("'sample.exe def': ExitCode = {0}", process.ExitCode);
      }

      exe.CleanUp();
    }
  }
}
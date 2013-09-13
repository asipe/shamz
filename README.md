# Shamz

Shamz is a simple micro framework for mocking command line executables

### Usage

```csharp
using System;
using System.Diagnostics;
using Shamz.Core;

namespace SimpleSampleNet35 {
  internal class Program {
    private static void Main() {
      try {
        Execute();
      } catch (Exception e) {
        Console.WriteLine(e.Message);
      }
    }

    private static void Execute() {
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
```

src/Shamz.Samples/ contains a solution with some sample projects

### License

shamz is licensed under the MIT License

     The MIT License (MIT)
     
     Copyright (c) 2013 Andy Sipe
     
     Permission is hereby granted, free of charge, to any person obtaining a copy of
     this software and associated documentation files (the "Software"), to deal in
     the Software without restriction, including without limitation the rights to
     use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
     the Software, and to permit persons to whom the Software is furnished to do so,
     subject to the following conditions:
     
     The above copyright notice and this permission notice shall be included in all
     copies or substantial portions of the Software.
     
     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
     FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
     COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
     IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
     CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

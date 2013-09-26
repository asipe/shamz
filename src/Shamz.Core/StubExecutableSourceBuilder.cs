namespace Shamz.Core {
  public class StubExecutableSourceBuilder {
    public string[] Build(StubSpec spec) {
      return new[] {
                     _ProgramTemplate,
                     new StubSourceBuilder().Build(spec)
                   };
    }

    private const string _ProgramTemplate = @"
using System;

namespace ShamzStub {
  public class Program {
    public static int Main(string[] args) {
      try {
        return new Stub().Execute(args);
      } catch(Exception e) {
        Console.Error.WriteLine(e);
        return -999;
      }
    }
  }
}
";
  }
}
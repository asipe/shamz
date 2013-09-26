namespace Shamz.Core {
  public class StubExecutableSourceBuilder {
    public string[] Build(StubSpec spec) {
      return new[] {
                     _ProgramTemplate,
                     new StubSourceBuilder().Build(spec)
                   };
    }

    private const string _ProgramTemplate = @"
namespace ShamzStub {
  public class Program {
    public static int Main(string[] args) {
      try {
        return new Stub().Execute(args);
      } catch {
        return -999;
      }
    }
  }
}
";
  }
}
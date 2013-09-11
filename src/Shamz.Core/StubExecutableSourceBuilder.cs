namespace Shamz.Core {
  public class StubExecutableSourceBuilder {
    public string[] Build(params Invocation[] invocations) {
      return new[] {
                     _ProgramTemplate,
                     new StubSourceBuilder().Build(invocations)
                   };
    }

    private const string _ProgramTemplate = @"
namespace ShamzStub {
  public class Program {
    public static int Main(string[] args) {
      return new Stub().Execute(args);
    }
  }
}
";
  }
}
namespace Shamz.Core {
  public class StubExecutableBuilder : IStubExecutableBuilder {
    public void Build(string outputPath, params Invocation[] invocations) {
      CompileExecutable(outputPath, BuildSources(invocations));
    }

    private static void CompileExecutable(string outputPath, string[] sources) {
      new ShamzSourceCompiler().CompileExecutable(outputPath, sources);
    }

    private static string[] BuildSources(Invocation[] invocations) {
      return new StubExecutableSourceBuilder().Build(invocations);
    }
  }
}
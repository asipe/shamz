namespace Shamz.Core {
  public class StubExecutableBuilder : IStubExecutableBuilder {
    public void Build(StubSpec spec) {
      CompileExecutable(spec.ExePath, BuildSources(spec));
    }

    private static void CompileExecutable(string outputPath, string[] sources) {
      new ShamzSourceCompiler().CompileExecutable(outputPath, sources);
    }

    private static string[] BuildSources(StubSpec spec) {
      return new StubExecutableSourceBuilder().Build(spec);
    }
  }
}
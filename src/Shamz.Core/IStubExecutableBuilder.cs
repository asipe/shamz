namespace Shamz.Core {
  public interface IStubExecutableBuilder {
    void Build(string outputPath, params Invocation[] invocations);
  }
}
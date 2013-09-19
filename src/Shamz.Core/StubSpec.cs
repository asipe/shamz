namespace Shamz.Core {
  public class StubSpec {
    public StubSpec(string exePath, int defaultExitCode, params Invocation[] invocations) {
      DefaultExitCode = defaultExitCode;
      ExePath = exePath;
      Invocations = invocations;
    }

    public Invocation[] Invocations{get;private set;}
    public string ExePath{get;private set;}
    public int DefaultExitCode{get;private set;}
  }
}
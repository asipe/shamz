namespace Shamz.Core {
  public class Invocation {
    public string[] CommandLine{get;set;}
    public int ExitCode{get;set;}
    public int ExecutionDelay{get;set;}

    public Invocation WhenCommandLine(params string[] commandLine) {
      CommandLine = commandLine;
      return this;
    }

    public Invocation ThenReturn(int exitCode) {
      ExitCode = exitCode;
      return this;
    }

    public Invocation Delay(int delay) {
      ExecutionDelay = delay;
      return this;
    }
  }
}
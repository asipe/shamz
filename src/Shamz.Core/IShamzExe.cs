using System;

namespace Shamz.Core {
  public interface IShamzExe {
    ShamzExe Setup(Action<Invocation> action);
    ShamzExe Initialize();
    ShamzExe WithDefaultExitCode(int code);
    void CleanUp();
  }
}
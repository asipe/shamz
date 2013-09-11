using System;

namespace Shamz.Core {
  public interface IShamzExe {
    ShamzExe Setup(Action<Invocation> action);
    ShamzExe Initialize();
    void CleanUp();
  }
}
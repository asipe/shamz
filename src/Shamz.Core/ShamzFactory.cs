using SupaCharge.Core.IOAbstractions;

namespace Shamz.Core {
  public static class ShamzFactory {
    public static IShamzExe CreateShamzExe(string executablePath) {
      return new ShamzExe(new StubExecutableBuilder(), new DotNetFile(), executablePath);
    }
  }
}
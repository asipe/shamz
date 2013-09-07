using SupaCharge.Core.IOAbstractions;

namespace Shamz.Core {
  public class ShamzExe {
    public ShamzExe(IFile file, string exePath) {
      mFile = file;
      mExePath = exePath;
    }

    public void Initialize() {
      mFile.WriteAllText(mExePath, "yeah, not an exe");
    }

    public void CleanUp() {
      mFile.Delete(mExePath);
    }

    private readonly string mExePath;
    private readonly IFile mFile;
  }
}
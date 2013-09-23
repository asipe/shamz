using System;
using System.Collections.Generic;
using SupaCharge.Core.IOAbstractions;
using SupaCharge.Core.ThreadingAbstractions;

namespace Shamz.Core {
  public class ShamzExe : IShamzExe {
    public ShamzExe(IStubExecutableBuilder builder, IFile file, string exePath) {
      mBuilder = builder;
      mFile = file;
      mExePath = exePath;
    }

    public ShamzExe Setup(Action<Invocation> action) {
      var invocation = new Invocation();
      action(invocation);
      mInvocations.Add(invocation);
      return this;
    }

    public ShamzExe WithDefaultExitCode(int code) {
      mExitCode = code;
      return this;
    }

    public ShamzExe Initialize() {
      ValidateInvocations();
      mBuilder.Build(new StubSpec(mExePath, mExitCode, mInvocations.ToArray()));
      return this;
    }

    public void CleanUp() {
      new Retry(100, 15)
        .WithWork(x => DeleteExe())
        .Start();
    }

    private void DeleteExe() {
      if (mFile.Exists(mExePath))
        mFile.Delete(mExePath);
      if (mFile.Exists(mExePath))
        throw new Exception("Executable Still Exists");
    }

    private void ValidateInvocations() {
      Array.ForEach(mInvocations.ToArray(), i => i.Validate());
    }

    private readonly IStubExecutableBuilder mBuilder;
    private readonly string mExePath;
    private readonly IFile mFile;
    private readonly List<Invocation> mInvocations = new List<Invocation>();
    private int mExitCode;
  }
}
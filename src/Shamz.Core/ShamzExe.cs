using System;
using System.Collections.Generic;
using SupaCharge.Core.IOAbstractions;

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

    public ShamzExe Initialize() {
      mBuilder.Build(mExePath, mInvocations.ToArray());
      return this;
    }

    public void CleanUp() {
      mFile.Delete(mExePath);
    }

    private readonly string mExePath;
    private readonly IStubExecutableBuilder mBuilder;
    private readonly IFile mFile;
    private readonly List<Invocation> mInvocations = new List<Invocation>();
  }
}
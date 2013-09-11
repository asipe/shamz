using System;
using System.CodeDom.Compiler;

namespace Shamz.Core {
  public class StubExecutableBuilder : IStubExecutableBuilder {
    public void Build(string outputPath, params Invocation[] invocations) {
      CompileExecutable(outputPath, BuildSources(invocations));
    }

    private static void CompileExecutable(string outputPath, string[] sources) {
      using (var provider = CodeDomProvider.CreateProvider("CSharp")) {
        var parms = new CompilerParameters {
                                             GenerateExecutable = true,
                                             GenerateInMemory = false,
                                             TreatWarningsAsErrors = true,
                                             IncludeDebugInformation = false,
                                             CompilerOptions = "/optimize",
                                             OutputAssembly = outputPath
                                           };
        parms.ReferencedAssemblies.Add("System.Core.dll");
        var results = provider.CompileAssemblyFromSource(parms, sources);

        ValidateNoErrors(results);
      }
    }

    private static void ValidateNoErrors(CompilerResults results) {
      if (results.Errors.Count != 0)
        throw new Exception("Error Compiling Stub Executable");
    }

    private static string[] BuildSources(Invocation[] invocations) {
      return new StubExecutableSourceBuilder().Build(invocations);
    }
  }
}
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;

namespace Shamz.Core {
  public class ShamzSourceCompiler {
    public void CompileExecutable(string outputPath, params string[] sources) {
      using (var provider = new CSharpCodeProvider(GetProviderConfiguration())) {
        var parms = new CompilerParameters {
                                             GenerateExecutable = true,
                                             GenerateInMemory = false,
                                             TreatWarningsAsErrors = true,
                                             IncludeDebugInformation = false,
                                             CompilerOptions = "/optimize",
                                             OutputAssembly = outputPath
                                           };
        AddCommonReferences(parms);
        var results = provider.CompileAssemblyFromSource(parms, sources);
        ValidateNoErrors(results);
      }
    }

    public Assembly CompileAssembly(params string[] sources) {
      using (var provider = new CSharpCodeProvider(GetProviderConfiguration())) {
        var parms = new CompilerParameters {
                                             GenerateExecutable = false,
                                             GenerateInMemory = true,
                                             TreatWarningsAsErrors = true,
                                             IncludeDebugInformation = false,
                                             CompilerOptions = "/optimize",
                                           };
        AddCommonReferences(parms);
        var results = provider.CompileAssemblyFromSource(parms, sources);
        ValidateNoErrors(results);
        return results.CompiledAssembly;
      }
    }

    private static void AddCommonReferences(CompilerParameters parms) {
      parms.ReferencedAssemblies.Add("System.dll");
      parms.ReferencedAssemblies.Add("System.Core.dll");
    }

    private static void ValidateNoErrors(CompilerResults results) {
      if (results.Errors.Count != 0)
        throw new Exception("Error Compiling Stub Executable: " + FormatErrors(results));
    }

    private static string FormatErrors(CompilerResults results) {
      var errors = new List<string> {Environment.NewLine};
      errors.AddRange(results.Errors.Cast<object>().Select(error => error.ToString()));
      return string.Join(Environment.NewLine, errors.ToArray());
    }

    private static Dictionary<string, string> GetProviderConfiguration() {
      var config = new Dictionary<string, string>();
#if (NET35) 
      config.Add("CompilerVersion", "v3.5");
#endif
      return config;
    }
  }
}
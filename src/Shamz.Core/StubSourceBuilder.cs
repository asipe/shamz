using System.Linq;
using System.Text;

namespace Shamz.Core {
  public class StubSourceBuilder {
    public string Build(StubSpec spec) {
      return string.Format(_ClassTemplate, BuildLogic(spec), spec.DefaultExitCode);
    }

    private static string BuildLogic(StubSpec spec) {
      var buf = new StringBuilder();
      foreach (var invocation in spec.Invocations)
        buf.AppendFormat(_IsMatchCallTemplate,
                         ForgeUseRegex(invocation),
                         FormatCommandLine(invocation),
                         invocation.ExecutionDelay,
                         invocation.ExitCode);
      return buf.ToString();
    }

    private static string ForgeUseRegex(Invocation invocation) {
      return invocation.MatchMode == MatchMode.Regex ? "true" : "false";
    }

    private static string FormatCommandLine(Invocation invocation) {
      var wrappedLine = invocation
        .CommandLine
        .Select(a => string.Format("\"{0}\"", a))
        .ToArray();
      return string.Format(_ArrayTemplate, string.Join(",", wrappedLine));
    }

    private const string _ClassTemplate = @"
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace ShamzStub {{
  public class Stub {{
    public int Execute(string[] args) {{
      {0}
      return {1};
    }}

    private static bool IsMatch(string[] args, bool useRegex, string[] candidates) {{
      if (args.Length != candidates.Length)
        return false;
      
      return args
        .Select((a,x) => new {{Arg = a, Candidate = candidates[x]}})
        .All(i => useRegex ? Regex.IsMatch(i.Arg, i.Candidate) : i.Arg == i.Candidate);
    }}
  }}
}}
";
    private const string _IsMatchCallTemplate = @"
if (IsMatch(args, {0}, {1})) {{
  Thread.Sleep({2});
  return {3};
}}
";
    private const string _ArrayTemplate = "new [] {{ {0} }}";
  }
}
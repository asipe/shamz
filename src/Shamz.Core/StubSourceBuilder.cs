using System.Linq;
using System.Text;

namespace Shamz.Core {
  public class StubSourceBuilder {
    public string Build(params Invocation[] invocations) {
      var buf = new StringBuilder();
      foreach (var invocation in invocations)
        buf.AppendFormat(_IsMatchCallTemplate, FormatCommandLine(invocation), invocation.ExitCode);
      return string.Format(_ClassTemplate, buf);
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

namespace ShamzStub {{
  public class Stub {{
    public int Execute(string[] args) {{
      {0}
      return 0;
    }}

    private static bool IsMatch(string[] args, string[] candidates) {{
      if (args.Length != candidates.Length)
        return false;
      
      return args
        .Select((a,x) => new {{Arg = a, Candidate = candidates[x]}})
        .All(i => Regex.IsMatch(i.Arg, i.Candidate));
    }}
  }}
}}
";
    private const string _IsMatchCallTemplate = @"
if (IsMatch(args, {0}))
  return {1};
";
    
    private const string _ArrayTemplate = "new [] {{ {0} }}";
  }
}
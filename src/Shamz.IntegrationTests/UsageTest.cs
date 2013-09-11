using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using Shamz.Core;
using SupaCharge.Testing;

namespace Shamz.IntegrationTests {
  [TestFixture]
  public class UsageTest : BaseTestCase {
    [Test]
    public void TestBasicUsage() {
      var shamz = ShamzFactory.CreateShamzExe(mWorkingExePath);
      shamz
        .Setup(invocation => invocation
                               .WhenCommandLine("a1", "b1", "c1")
                               .ThenReturn(0))
        .Setup(invocation => invocation
                               .WhenCommandLine("a2", "b2", "c2")
                               .ThenReturn(1))
        .Initialize();
      ExecuteProcess("a1 b1 c1", 0);
      ExecuteProcess("a2 b2 c2", 1);
      ExecuteProcess("", 0);
      ExecuteProcess("a1b1c1", 0);
      shamz.CleanUp();
      Assert.That(File.Exists(mWorkingExePath), Is.False);
    }

    [Test]
    public void TestRegexUsage() {
      var shamz = ShamzFactory.CreateShamzExe(mWorkingExePath);
      shamz
        .Setup(invocation => invocation
                               .WhenCommandLine("^a[0-9]$", "b[0-9]", "c1")
                               .ThenReturn(1000))
        .Setup(invocation => invocation
                               .WhenCommandLine(".+", ".+", ".+")
                               .ThenReturn(2000))
        .Initialize();
      ExecuteProcess("a1 b1 c1", 1000);
      ExecuteProcess("a2 b2 c2", 2000);
      ExecuteProcess("a2", 0);
      shamz.CleanUp();
      Assert.That(File.Exists(mWorkingExePath), Is.False);
    }

    [SetUp]
    public void DoSetup() {
      mWorkingDir = CreateTempDir();
      mWorkingExePath = Path.Combine(mWorkingDir, "sample.exe");
    }

    private void ExecuteProcess(string arguments, int expectedExitCode) {
      using (var process = new Process {
                                         StartInfo = new ProcessStartInfo {
                                                                            WorkingDirectory = mWorkingDir,
                                                                            FileName = "sample.exe",
                                                                            Arguments = arguments,
                                                                            WindowStyle = ProcessWindowStyle.Hidden
                                                                          }
                                       }) {
        process.Start();
        process.WaitForExit();
        Assert.That(process.ExitCode, Is.EqualTo(expectedExitCode));
      }
    }

    private string mWorkingDir;
    private string mWorkingExePath;
  }
}
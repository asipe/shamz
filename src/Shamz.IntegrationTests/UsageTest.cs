using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using Shamz.Core;
using SupaCharge.Core.ThreadingAbstractions;
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

      new WorkQueueBatch(BuildWorkQueue())
        .Add(() => Measure(0, 500, () => ExecuteProcess("a1 b1 c1", 0)),
             () => Measure(0, 500, () => ExecuteProcess("a2 b2 c2", 1)),
             () => Measure(0, 500, () => ExecuteProcess("", 0)),
             () => Measure(0, 500, () => ExecuteProcess("a1b1c1", 0)))
        .Wait(100000);

      shamz.CleanUp();
      VerifyShamzExecutableRemoved();
    }

    [Test]
    public void TestDefaultExitCodeUsage() {
      var shamz = ShamzFactory.CreateShamzExe(mWorkingExePath);
      shamz
        .Setup(invocation => invocation
                               .WhenCommandLine("a1", "b1", "c1")
                               .ThenReturn(0))
        .Setup(invocation => invocation
                               .WhenCommandLine("a2", "b2", "c2")
                               .ThenReturn(1))
        .WithDefaultExitCode(100)
        .Initialize();
      new WorkQueueBatch(BuildWorkQueue())
        .Add(() => Measure(0, 500, () => ExecuteProcess("a1 b1 c1", 0)),
             () => Measure(0, 500, () => ExecuteProcess("a2 b2 c2", 1)),
             () => Measure(0, 500, () => ExecuteProcess("", 100)),
             () => Measure(0, 500, () => ExecuteProcess("a1b1c1", 100)),
             () => Measure(0, 500, () => ExecuteProcess(null, 100)))
        .Wait(100000);
      shamz.CleanUp();
      VerifyShamzExecutableRemoved();
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
      new WorkQueueBatch(BuildWorkQueue())
        .Add(() => Measure(0, 500, () => ExecuteProcess("a1 b1 c1", 1000)),
             () => Measure(0, 500, () => ExecuteProcess("a2 b2 c2", 2000)),
             () => Measure(0, 500, () => ExecuteProcess("a2", 0)))
        .Wait(100000);
      shamz.CleanUp();
      VerifyShamzExecutableRemoved();
    }

    [Test]
    public void TestDelayUsage() {
      var shamz = ShamzFactory.CreateShamzExe(mWorkingExePath);
      shamz
        .Setup(invocation => invocation
                               .WhenCommandLine("arg1", "arg2", "arg3")
                               .Delay(1000)
                               .ThenReturn(1000))
        .Setup(invocation => invocation
                               .WhenCommandLine("arg4", "arg5", "arg6")
                               .ThenReturn(2000))
        .Setup(invocation => invocation
                               .WhenCommandLine("arg7", "arg8", "arg9")
                               .Delay(1500)
                               .ThenReturn(3000))
        .Initialize();
      new WorkQueueBatch(BuildWorkQueue())
        .Add(() => Measure(1000, 2500, () => ExecuteProcess("arg1 arg2 arg3", 1000)),
             () => Measure(0, 2000, () => ExecuteProcess("arg4 arg5 arg6", 2000)),
             () => Measure(1500, 3000, () => ExecuteProcess("arg7 arg8 arg9", 3000)))
        .Wait(100000);
      shamz.CleanUp();
      VerifyShamzExecutableRemoved();
    }

    [SetUp]
    public void DoSetup() {
      mWorkingDir = CreateTempDir();
      mWorkingExePath = Path.Combine(mWorkingDir, "sample.exe");
    }

    [TearDown]
    public void DoTearDown() {
      Directory.Delete(TempDir, true);
    }

    private static IWorkQueue BuildWorkQueue() {
      return new ThreadPoolWorkQueue();
    }

    private void VerifyShamzExecutableRemoved() {
      Assert.That(File.Exists(mWorkingExePath), Is.False);
    }

    private void ExecuteProcess(string arguments, int expectedExitCode) {
      Assert.That(File.Exists(mWorkingExePath), Is.True);
      using (var process = new Process {
                                         StartInfo = new ProcessStartInfo {
                                                                            WorkingDirectory = mWorkingDir,
                                                                            FileName = mWorkingExePath,
                                                                            Arguments = arguments,
                                                                            WindowStyle = ProcessWindowStyle.Hidden,
                                                                            CreateNoWindow = true,
                                                                            RedirectStandardOutput = true,
                                                                            RedirectStandardError = true,
                                                                            UseShellExecute = false
                                                                          }
                                       }) {
        Assert.That(process.Start(), Is.True);
        Assert.That(process.StandardError.ReadToEnd(), Is.Empty);
        Assert.That(process.StandardOutput.ReadToEnd(), Is.Empty);
        process.WaitForExit(60000);
        Assert.That(process.ExitCode, Is.EqualTo(expectedExitCode));
        process.Close();
      }
    }

    private static void Measure(int min, int max, Action work) {
      var sw = Stopwatch.StartNew();
      work();
      Assert.That(sw.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(min).And.LessThanOrEqualTo(max));
    }

    private string mWorkingDir;
    private string mWorkingExePath;
  }
}
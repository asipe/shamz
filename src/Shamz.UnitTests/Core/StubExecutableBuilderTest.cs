using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using Shamz.Core;
using SupaCharge.Testing;

namespace Shamz.UnitTests.Core {
  [TestFixture]
  public class StubExecutableBuilderTest : BaseTestCase {
    [Test]
    public void TestBuildWithNoInvocationsGivesDefault() {
      mBuilder.Build(mExePath);
      Check(0);
    }

    [Test]
    public void TestWithSingleInvocationThatMatchesArgsReturnsInvocationsCode() {
      mBuilder.Build(mExePath, new Invocation().WhenCommandLine("a", "b", "c").ThenReturn(100));
      Check(100, "a", "b", "c");
    }

    [Test]
    public void TestWithSingleInvocationThatDoesNotMatchArgsReturnsDefault() {
      mBuilder.Build(mExePath, new Invocation().WhenCommandLine("a", "b", "c").ThenReturn(100));
      Check(0, "a", "b", "e");
    }

    [Test]
    public void TestWithMultipleInvocations() {
      mBuilder.Build(mExePath,
                     new Invocation().WhenCommandLine("a1", "b1", "c1").ThenReturn(100),
                     new Invocation().WhenCommandLine("a2", "c2").ThenReturn(200),
                     new Invocation().WhenCommandLine("a3").ThenReturn(300));
      Check(100, "a1", "b1", "c1");
      Check(200, "a2", "c2");
      Check(300, "a3");
      Check(100, "a1", "b1", "c1");
      Check(0);
    }

    [Test]
    public void TestFirstInvocationWinsWhenThereAreDuplicateRegistrations() {
      mBuilder.Build(mExePath,
                     new Invocation().WhenCommandLine("a1", "b1", "c1").ThenReturn(100),
                     new Invocation().WhenCommandLine("a1", "b1", "c1").ThenReturn(200));
      Check(100, "a1", "b1", "c1");
    }

    [SetUp]
    public void DoSetup() {
      CreateTempDir();
      mExePath = Path.Combine(TempDir, "sample.exe");
      mBuilder = new StubExecutableBuilder();
    }

    private void Check(int expected, params string[] args) {
      Assert.That(File.Exists(mExePath), Is.True);
      using (var process = new Process()) {
        process.StartInfo = new ProcessStartInfo {
                                                   WorkingDirectory = TempDir,
                                                   FileName = "sample.exe",
                                                   Arguments = string.Join(" ", args),
                                                   WindowStyle = ProcessWindowStyle.Hidden
                                                 };
        process.Start();
        process.WaitForExit();
        Assert.That(process.ExitCode, Is.EqualTo(expected));
        process.Close();
      }
    }

    private StubExecutableBuilder mBuilder;
    private string mExePath;
  }
}
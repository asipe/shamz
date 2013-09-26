using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using Shamz.Core;
using SupaCharge.Testing;

namespace Shamz.UnitTests.Core {
  [TestFixture]
  public class StubExecutableSourceBuilderTest : BaseTestCase {
    [Test]
    public void TestBuildWithNoInvocationsGivesDefault() {
      Compile(mBuilder.Build(new StubSpec("", 0)));
      Check(0, "^$", "^$");
    }

    [Test]
    public void TestWithSingleInvocationThatMatchesArgsReturnsInvocationsCode() {
      Compile(mBuilder.Build(new StubSpec("", 0, new Invocation().WhenCommandLine("a", "b", "c").ThenReturn(100))));
      Check(100, "^$", "^$", "a", "b", "c");
    }

    [Test]
    public void TestWithSingleInvocationThatDoesNotMatchArgsReturnsDefault() {
      Compile(mBuilder.Build(new StubSpec("", 0, new Invocation().WhenCommandLine("a", "b", "c").ThenReturn(100))));
      Check(0, "^$", "^$", "a", "b", "e");
    }

    [Test]
    public void TestWithMultipleInvocations() {
      Compile(mBuilder.Build(new StubSpec("",
                                          0,
                                          new Invocation().WhenCommandLine("a1", "b1", "c1").ThenReturn(100),
                                          new Invocation().WhenCommandLine("a2", "c2").ThenReturn(200),
                                          new Invocation().WhenCommandLine("a3").ThenReturn(300))));
      Check(100, "^$", "^$", "a1", "b1", "c1");
      Check(200, "^$", "^$", "a2", "c2");
      Check(300, "^$", "^$", "a3");
      Check(100, "^$", "^$", "a1", "b1", "c1");
      Check(0, "^$", "^$");
    }

    [Test]
    public void TestFirstInvocationWinsWhenThereAreDuplicateRegistrations() {
      Compile(mBuilder.Build(new StubSpec("",
                                          0,
                                          new Invocation().WhenCommandLine("a1", "b1", "c1").ThenReturn(100),
                                          new Invocation().WhenCommandLine("a1", "b1", "c1").ThenReturn(200))));
      Check(100, "^$", "^$", "a1", "b1", "c1");
    }

    [Test]
    public void TestWithInvocationWhichCausesARunTimeErrorReturnsNegative999() {
      Compile(mBuilder.Build(new StubSpec("",
                                          0,
                                          new Invocation().WhenCommandLine("a1", "b1", "c[1").ThenReturn(100))));
      Check(-999, "^System.ArgumentException", "^$", "a1", "b1", "c1");
    }

    [SetUp]
    public void DoSetup() {
      CreateTempDir();
      mExePath = Path.Combine(TempDir, "sample.exe");
      mBuilder = new StubExecutableSourceBuilder();
    }

    private void Check(int expected, string expectedStandardError, string expectedStandardOutput, params string[] args) {
      Assert.That(File.Exists(mExePath), Is.True);
      using (var process = new Process()) {
        process.StartInfo = new ProcessStartInfo {
                                                   WorkingDirectory = TempDir,
                                                   FileName = mExePath,
                                                   Arguments = string.Join(" ", args),
                                                   UseShellExecute = false,
                                                   CreateNoWindow = true,
                                                   RedirectStandardError = true,
                                                   RedirectStandardOutput = true
                                                 };
        process.Start();
        Assert.That(process.StandardError.ReadToEnd(), Is.StringMatching(expectedStandardError));
        Assert.That(process.StandardOutput.ReadToEnd(), Is.StringMatching(expectedStandardOutput));
        process.WaitForExit();
        Assert.That(process.ExitCode, Is.EqualTo(expected));
        process.Close();
      }
    }

    private void Compile(string[] sources) {
      new ShamzSourceCompiler().CompileExecutable(mExePath, sources);
    }

    private StubExecutableSourceBuilder mBuilder;
    private string mExePath;
  }
}
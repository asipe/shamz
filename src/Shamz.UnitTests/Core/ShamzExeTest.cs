using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Shamz.Core;
using SupaCharge.Core.IOAbstractions;
using SupaCharge.Testing;

namespace Shamz.UnitTests.Core {
  [TestFixture]
  public class ShamzExeTest : BaseTestCase {
    [Test]
    public void TestInitializeWithNoSetups() {
      mBuilder.Setup(b => b.Build(It.Is<StubSpec>(spec => SpecMatches(spec, _ExePath, 0))));
      mShamz.Initialize();
    }
    
    [Test]
    public void TestInitializeWithInvalidInvocationThrows() {
      mShamz.Setup(inv => mExpectedInvocations.Add(inv));
      var ex = Assert.Throws(typeof(ArgumentException), () => mShamz.Initialize());
      Assert.That(ex.Message, Is.EqualTo("Invalid CommandLine Argument"));
    }

    [TestCase(0)]
    [TestCase(100)]
    public void TestInitializeWithCustomExitCodeOnly(int exitCode) {
      mBuilder.Setup(b => b.Build(It.Is<StubSpec>(spec => SpecMatches(spec, _ExePath, exitCode))));
      mShamz
        .WithDefaultExitCode(exitCode)
        .Initialize();
    }

    [Test]
    public void TestInitializeWithSingleSetup() {
      mBuilder.Setup(b => b.Build(It.IsAny<StubSpec>()));
      mShamz
        .Setup(inv => {
                 inv
                   .WhenCommandLine("a", "b", "c")
                   .ThenReturn(33);
                 mExpectedInvocations.Add(inv);
               })
        .Initialize();
      mBuilder.Verify(b => b.Build(It.Is<StubSpec>(spec => SpecMatches(spec, _ExePath, 0, mExpectedInvocations.ToArray()))));
    }

    [Test]
    public void TestInitializeWithMultipleSetups() {
      mBuilder.Setup(b => b.Build(It.IsAny<StubSpec>()));
      mShamz
        .Setup(inv => {
                 inv
                   .WhenCommandLine("a1", "b1", "c1")
                   .ThenReturn(33);
                 mExpectedInvocations.Add(inv);
               })
        .Setup(inv => {
                 inv
                   .WhenCommandLine("a2", "b2", "c2")
                   .ThenReturn(44);
                 mExpectedInvocations.Add(inv);
               })
        .Setup(inv => {
                 inv
                   .WhenCommandLine("a3", "b3", "c3")
                   .ThenReturn(55);
                 mExpectedInvocations.Add(inv);
               })
        .Initialize();
      mBuilder.Verify(b => b.Build(It.Is<StubSpec>(spec => SpecMatches(spec, _ExePath, 0, mExpectedInvocations.ToArray()))));
    }

    [Test]
    public void TestCleanUpRemovesExeFile() {
      mFile.Setup(f => f.Delete(_ExePath));
      mShamz.CleanUp();
    }

    [SetUp]
    public void DoSetup() {
      mExpectedInvocations.Clear();
      mFile = Mok<IFile>();
      mBuilder = Mok<IStubExecutableBuilder>();
      mShamz = new ShamzExe(mBuilder.Object, mFile.Object, _ExePath);
    }

    private static bool SpecMatches(StubSpec actual, string outputPath, int exitCode, params Invocation[] invocations) {
      return Equals(actual.ExePath, outputPath) &&
             Equals(actual.DefaultExitCode, exitCode) &&
             actual.Invocations.SequenceEqual(invocations);
    }

    private readonly List<Invocation> mExpectedInvocations = new List<Invocation>();
    private Mock<IFile> mFile;
    private ShamzExe mShamz;
    private Mock<IStubExecutableBuilder> mBuilder;
    private const string _ExePath = @"c:\app1\app.exe";
  }
}
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
      mBuilder.Setup(b => b.Build(_ExePath));
      mShamz.Initialize();
    }

    [Test]
    public void TestInitializeWithSingleSetup() {
      mBuilder.Setup(b => b.Build(_ExePath, It.IsAny<Invocation[]>()));
      mShamz
        .Setup(inv => {
                 inv
                   .WhenCommandLine("a", "b", "c")
                   .ThenReturn(33);
                 mExpectedInvocations.Add(inv);
               })
        .Initialize();
      mBuilder.Verify(b => b.Build(_ExePath, It.Is<Invocation[]>(a => a.SequenceEqual(mExpectedInvocations))));
    }

    [Test]
    public void TestInitializeWithMultipleSetups() {
      mBuilder.Setup(b => b.Build(_ExePath, It.IsAny<Invocation[]>()));
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
      mBuilder.Verify(b => b.Build(_ExePath, It.Is<Invocation[]>(a => a.SequenceEqual(mExpectedInvocations))));
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

    private readonly List<Invocation> mExpectedInvocations = new List<Invocation>();
    private Mock<IFile> mFile;
    private ShamzExe mShamz;
    private Mock<IStubExecutableBuilder> mBuilder;
    private const string _ExePath = @"c:\app1\app.exe";
  }
}
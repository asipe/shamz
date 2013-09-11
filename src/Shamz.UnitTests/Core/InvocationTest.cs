using NUnit.Framework;
using Shamz.Core;
using SupaCharge.Testing;

namespace Shamz.UnitTests.Core {
  [TestFixture]
  public class InvocationTest : BaseTestCase {
    [Test]
    public void TestDefaults() {
      Assert.That(mInvocation.CommandLine, Is.Null);
      Assert.That(mInvocation.ExitCode, Is.EqualTo(0));
    }

    [Test]
    public void TestSetCommandLine() {
      mInvocation.WhenCommandLine("a", "b", "c");
      Assert.That(mInvocation.CommandLine, Is.EqualTo(BA("a", "b", "c")));
    }

    [Test]
    public void TestSetReturn() {
      mInvocation.ThenReturn(3);
      Assert.That(mInvocation.ExitCode, Is.EqualTo(3));
    }

    [Test]
    public void TestSetAllProperties() {
      mInvocation
        .WhenCommandLine("abc", "def")
        .ThenReturn(44);
      Assert.That(mInvocation.CommandLine, Is.EqualTo(BA("abc", "def")));
      Assert.That(mInvocation.ExitCode, Is.EqualTo(44));
    }

    [SetUp]
    public void DoSetup() {
      mInvocation = new Invocation();
    }

    private Invocation mInvocation;
  }
}
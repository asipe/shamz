using System;
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
      Assert.That(mInvocation.ExecutionDelay, Is.EqualTo(0));
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
    public void TestSetDelay() {
      mInvocation.Delay(1000);
      Assert.That(mInvocation.ExecutionDelay, Is.EqualTo(1000));
    }

    [Test]
    public void TestSetAllProperties() {
      mInvocation
        .WhenCommandLine("abc", "def")
        .Delay(100)
        .ThenReturn(44);
      Assert.That(mInvocation.CommandLine, Is.EqualTo(BA("abc", "def")));
      Assert.That(mInvocation.ExecutionDelay, Is.EqualTo(100));
      Assert.That(mInvocation.ExitCode, Is.EqualTo(44));
    }

    [Test]
    public void TestValidateThrowsWhenNoCommandLineIsDefined() {
      var ex = Assert.Throws(typeof(ArgumentException), () => mInvocation.Validate());
      Assert.That(ex.Message, Is.EqualTo("Invalid CommandLine Argument"));
    }

    [SetUp]
    public void DoSetup() {
      mInvocation = new Invocation();
    }

    private Invocation mInvocation;
  }
}
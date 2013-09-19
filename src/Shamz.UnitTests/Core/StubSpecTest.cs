using NUnit.Framework;
using Shamz.Core;
using SupaCharge.Testing;

namespace Shamz.UnitTests.Core {
  [TestFixture]
  public class StubSpecTest : BaseTestCase {
    [Test]
    public void TestDefaults() {
      var invocations = CM<Invocation>();
      var spec = new StubSpec("anoutputpath", 33, invocations);
      Assert.That(spec.DefaultExitCode, Is.EqualTo(33));
      Assert.That(spec.ExePath, Is.EqualTo("anoutputpath"));
      Assert.That(spec.Invocations, Is.EqualTo(invocations));
    }
  }
}
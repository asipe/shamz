using NUnit.Framework;
using SupaCharge.Testing;

namespace Shamz.UnitTests {
  [TestFixture]
  public class SampleTest : BaseTestCase {
    [Test]
    public void TestSomething() {
      Assert.That(true, Is.True);
    }
  }
}
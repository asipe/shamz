using System.IO;
using NUnit.Framework;
using Shamz.Core;
using SupaCharge.Core.IOAbstractions;
using SupaCharge.Testing;

namespace Shamz.IntegrationTests {
  [TestFixture]
  public class UsageTest : BaseTestCase {
    [Test]
    public void TestBasicUsage() {
      var shamz = new ShamzExe(new DotNetFile(), mWorkingExePath);
      Assert.That(File.Exists(mWorkingExePath), Is.False);
      shamz.Initialize();
      Assert.That(File.Exists(mWorkingExePath), Is.True);
      shamz.CleanUp();
      Assert.That(File.Exists(mWorkingExePath), Is.False);
    }

    [SetUp]
    public void DoSetup() {
      mWorkingDir = CreateTempDir();
      mWorkingExePath = Path.Combine(mWorkingDir, "sample.exe");
    }

    private string mWorkingDir;
    private string mWorkingExePath;
  }
}
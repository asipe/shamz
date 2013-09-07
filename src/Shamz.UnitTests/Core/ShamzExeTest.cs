using Moq;
using NUnit.Framework;
using Shamz.Core;
using SupaCharge.Core.IOAbstractions;
using SupaCharge.Testing;

namespace Shamz.UnitTests.Core {
  [TestFixture]
  public class ShamzExeTest : BaseTestCase {
    [Test]
    public void TestInitializeCreatesExeFile() {
      mFile.Setup(f => f.WriteAllText(@"c:\app1\app.exe", "yeah, not an exe"));
      mShamz.Initialize();
    }

    [Test]
    public void TestCleanUpRemovesExeFile() {
      mFile.Setup(f => f.Delete(@"c:\app1\app.exe"));
      mShamz.CleanUp();
    }

    [SetUp]
    public void DoSetup() {
      mFile = Mok<IFile>();
      mShamz = new ShamzExe(mFile.Object, @"c:\app1\app.exe");
    }

    private Mock<IFile> mFile;
    private ShamzExe mShamz;
  }
}
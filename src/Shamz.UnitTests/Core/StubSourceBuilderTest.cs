using System.Reflection;
using NUnit.Framework;
using Shamz.Core;
using SupaCharge.Testing;

namespace Shamz.UnitTests.Core {
  [TestFixture]
  public class StubSourceBuilderTest : BaseTestCase {
    [Test]
    public void TestBuildWithNoInvocationsGivesDefault() {
      var source = mBuilder.Build();
      var instance = CreateInstance(Compile(source));
      Check(instance, 0, CM<string>());
    }

    [Test]
    public void TestWithSingleInvocationThatMatchesArgsReturnsInvocationsCode() {
      var source = mBuilder.Build(new Invocation().WhenCommandLine("a", "b", "c").ThenReturn(100));
      var instance = CreateInstance(Compile(source));
      Check(instance, 100, "a", "b", "c");
    }

    [Test]
    public void TestWithSingleInvocationThatDoesNotMatchArgsReturnsDefault() {
      var source = mBuilder.Build(new Invocation().WhenCommandLine("a", "b", "c").ThenReturn(100));
      var instance = CreateInstance(Compile(source));
      Check(instance, 0, "a", "b", "e");
    }

    [Test]
    public void TestWithMultipleInvocations() {
      var source = mBuilder.Build(new Invocation().WhenCommandLine("a1", "b1", "c1").ThenReturn(100),
                                  new Invocation().WhenCommandLine("a2", "c2").ThenReturn(200),
                                  new Invocation().WhenCommandLine("a3").ThenReturn(300));
      var instance = CreateInstance(Compile(source));
      Check(instance, 100, "a1", "b1", "c1");
      Check(instance, 200, "a2", "c2");
      Check(instance, 300, "a3");
      Check(instance, 100, "a1", "b1", "c1");
      Check(instance, 0);
    }

    [Test]
    public void TestFirstInvocationWinsWhenThereAreDuplicateRegistrations() {
      var source = mBuilder.Build(new Invocation().WhenCommandLine("a1", "b1", "c1").ThenReturn(100),
                                  new Invocation().WhenCommandLine("a1", "b1", "c1").ThenReturn(200));
      var instance = CreateInstance(Compile(source));
      Check(instance, 100, "a1", "b1", "c1");
    }

    [Test]
    public void TestWithSingleInvocationThatMatchesArgsWithRegexReturnsInvocationsCode() {
      var source = mBuilder.Build(new Invocation().WhenCommandLine("a[0-9]", "b", "c").ThenReturn(100));
      var instance = CreateInstance(Compile(source));
      Check(instance, 100, "a1", "b", "c");
    }

    [Test]
    public void TestWithMultipleInvocationsThatMatchesArgsWithRegexReturnsInvocationsCode() {
      var source = mBuilder.Build(new Invocation().WhenCommandLine("^a[0-9]$", "b", "c").ThenReturn(100),
                                  new Invocation().WhenCommandLine("a[0-9][0-9]", "^b$", "c").ThenReturn(200),
                                  new Invocation().WhenCommandLine(".+", "b+", "c").ThenReturn(300));
      var instance = CreateInstance(Compile(source));
      Check(instance, 100, "a1", "b", "c");
      Check(instance, 100, "a9", "b", "c");
      Check(instance, 200, "a10", "b", "c");
      Check(instance, 300, "a10", "bbbbbbb", "c");
    }

    [SetUp]
    public void DoSetup() {
      mBuilder = new StubSourceBuilder();
    }

    private static void Check(object instance, int expected, params string[] args) {
      var method = instance.GetType().GetMethod("Execute");
      Assert.That(method.Invoke(instance, new object[] {args}), Is.EqualTo(expected));
    }

    private static object CreateInstance(Assembly assembly) {
      return assembly.CreateInstance("ShamzStub.Stub");
    }

    private static Assembly Compile(string source) {
      return new ShamzSourceCompiler().CompileAssembly(source);
    }

    private StubSourceBuilder mBuilder;
  }
}
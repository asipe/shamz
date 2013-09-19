using System.Diagnostics;
using System.Reflection;
using NUnit.Framework;
using Shamz.Core;
using SupaCharge.Testing;

namespace Shamz.UnitTests.Core {
  [TestFixture]
  public class StubSourceBuilderTest : BaseTestCase {
    [Test]
    public void TestBuildWithNoInvocationsGivesDefault() {
      var source = mBuilder.Build(new StubSpec("", 50));
      var instance = CreateInstance(Compile(source));
      Check(instance, 50, 0, 500, CM<string>());
    }

    [Test]
    public void TestWithSingleInvocationThatMatchesArgsReturnsInvocationsCode() {
      var source = mBuilder.Build(new StubSpec("", 0, new Invocation().WhenCommandLine("a", "b", "c").ThenReturn(100)));
      var instance = CreateInstance(Compile(source));
      Check(instance, 100, 0, 500, "a", "b", "c");
    }

    [Test]
    public void TestWithSingleInvocationThatDoesNotMatchArgsReturnsDefault() {
      var source = mBuilder.Build(new StubSpec("", 0, new Invocation().WhenCommandLine("a", "b", "c").ThenReturn(100)));
      var instance = CreateInstance(Compile(source));
      Check(instance, 0, 0, 500, "a", "b", "e");
    }

    [Test]
    public void TestWithMultipleInvocations() {
      var source = mBuilder.Build(new StubSpec("",
                                               0,
                                               new Invocation().WhenCommandLine("a1", "b1", "c1").ThenReturn(100),
                                               new Invocation().WhenCommandLine("a2", "c2").ThenReturn(200),
                                               new Invocation().WhenCommandLine("a3").ThenReturn(300)));
      var instance = CreateInstance(Compile(source));
      Check(instance, 100, 0, 500, "a1", "b1", "c1");
      Check(instance, 200, 0, 500, "a2", "c2");
      Check(instance, 300, 0, 500, "a3");
      Check(instance, 100, 0, 500, "a1", "b1", "c1");
      Check(instance, 0, 0, 500);
    }

    [Test]
    public void TestFirstInvocationWinsWhenThereAreDuplicateRegistrations() {
      var source = mBuilder.Build(new StubSpec("",
                                               0,
                                               new Invocation().WhenCommandLine("a1", "b1", "c1").ThenReturn(100),
                                               new Invocation().WhenCommandLine("a1", "b1", "c1").ThenReturn(200)));
      var instance = CreateInstance(Compile(source));
      Check(instance, 100, 0, 500, "a1", "b1", "c1");
    }

    [Test]
    public void TestWithSingleInvocationThatMatchesArgsWithRegexReturnsInvocationsCode() {
      var source = mBuilder.Build(new StubSpec("",
                                               0,
                                               new Invocation().WhenCommandLine("a[0-9]", "b", "c").ThenReturn(100)));
      var instance = CreateInstance(Compile(source));
      Check(instance, 100, 0, 500, "a1", "b", "c");
    }

    [Test]
    public void TestWithMultipleInvocationsThatMatchesArgsWithRegexReturnsInvocationsCode() {
      var source = mBuilder.Build(new StubSpec("",
                                               0,
                                               new Invocation().WhenCommandLine("^a[0-9]$", "b", "c").ThenReturn(100),
                                               new Invocation().WhenCommandLine("a[0-9][0-9]", "^b$", "c").ThenReturn(200),
                                               new Invocation().WhenCommandLine(".+", "b+", "c").ThenReturn(300)));
      var instance = CreateInstance(Compile(source));
      Check(instance, 100, 0, 500, "a1", "b", "c");
      Check(instance, 100, 0, 500, "a9", "b", "c");
      Check(instance, 200, 0, 500, "a10", "b", "c");
      Check(instance, 300, 0, 500, "a10", "bbbbbbb", "c");
    }

    [Test]
    public void TestWithSingleInvocationWithDelay() {
      var source = mBuilder.Build(new StubSpec("", 0, new Invocation().WhenCommandLine("a", "b", "c").Delay(1000).ThenReturn(100)));
      var instance = CreateInstance(Compile(source));
      Check(instance, 100, 1000, 1500, "a", "b", "c");
    }

    [SetUp]
    public void DoSetup() {
      mBuilder = new StubSourceBuilder();
    }

    private static void Check(object instance,
                              int expectedResult,
                              int minExecutionTime,
                              int maxExecutionTime,
                              params string[] args) {
      var sw = Stopwatch.StartNew();
      var method = instance.GetType().GetMethod("Execute");
      Assert.That(method.Invoke(instance, new object[] {args}), Is.EqualTo(expectedResult));
      Assert.That(sw.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(minExecutionTime).And.LessThanOrEqualTo(maxExecutionTime));
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
using System.CodeDom.Compiler;
using NUnit.Framework;
using Shamz.Core;
using SupaCharge.Testing;

namespace Shamz.UnitTests.Core {
  [TestFixture]
  public class StubSourceBuilderTest : BaseTestCase {
    [Test]
    public void TestBuildWithNoInvocationsGivesDefault() {
      var source = mBuilder.Build();
      var results = Compile(source);
      var instance = CreateInstance(results);
      Check(instance, 0, CM<string>());
    }

    [Test]
    public void TestWithSingleInvocationThatMatchesArgsReturnsInvocationsCode() {
      var source = mBuilder.Build(new Invocation().WhenCommandLine("a", "b", "c").ThenReturn(100));
      var results = Compile(source);
      var instance = CreateInstance(results);
      Check(instance, 100, "a", "b", "c");
    }

    [Test]
    public void TestWithSingleInvocationThatDoesNotMatchArgsReturnsDefault() {
      var source = mBuilder.Build(new Invocation().WhenCommandLine("a", "b", "c").ThenReturn(100));
      var results = Compile(source);
      var instance = CreateInstance(results);
      Check(instance, 0, "a", "b", "e");
    }

    [Test]
    public void TestWithMultipleInvocations() {
      var source = mBuilder.Build(new Invocation().WhenCommandLine("a1", "b1", "c1").ThenReturn(100),
                                  new Invocation().WhenCommandLine("a2", "c2").ThenReturn(200),
                                  new Invocation().WhenCommandLine("a3").ThenReturn(300));
      var results = Compile(source);
      var instance = CreateInstance(results);
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
      var results = Compile(source);
      var instance = CreateInstance(results);
      Check(instance, 100, "a1", "b1", "c1");
    }

    [SetUp]
    public void DoSetup() {
      mBuilder = new StubSourceBuilder();
    }

    private static void Check(object instance, int expected, params string[] args) {
      var method = instance.GetType().GetMethod("Execute");
      Assert.That(method.Invoke(instance, new object[] {args}), Is.EqualTo(expected));
    }

    private static object CreateInstance(CompilerResults results) {
      return results.CompiledAssembly.CreateInstance("ShamzStub.Stub");
    }

    private static CompilerResults Compile(string source) {
      using (var provider = CodeDomProvider.CreateProvider("CSharp")) {
        var parms = new CompilerParameters {
                                             GenerateExecutable = false,
                                             GenerateInMemory = true,
                                             TreatWarningsAsErrors = true
                                           };
        parms.ReferencedAssemblies.Add("System.Core.dll");
        var results = provider.CompileAssemblyFromSource(parms, source);
        Assert.That(results.Errors, Is.Empty);
        return results;
      }
    }

    private StubSourceBuilder mBuilder;
  }
}
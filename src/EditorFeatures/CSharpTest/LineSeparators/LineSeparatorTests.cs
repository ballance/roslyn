﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Extensions;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editor.CSharp.LineSeparator;
using Microsoft.CodeAnalysis.Editor.UnitTests.Workspaces;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.LineSeparators
{
    public class LineSeparatorTests
    {
        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void TestEmptyFile()
        {
            AssertTagsOnBracesOrSemicolons(contents: string.Empty);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void TestEmptyClass()
        {
            var file = @"class C
{
}";
            AssertTagsOnBracesOrSemicolons(file, 0);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void TestClassWithOneMethod()
        {
            var file = @"class C
{
    void M()
    {
    }
}";
            AssertTagsOnBracesOrSemicolons(file, 1);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void TestClassWithTwoMethods()
        {
            var file = @"class C
{
    void M()
    {
    }

    void N()
    {
    }
}";
            AssertTagsOnBracesOrSemicolons(file, 0, 2);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void TestClassWithTwoNonEmptyMethods()
        {
            var file = @"class C
{
    void M()
    {
        N();
    }

    void N()
    {
        M();
    }
}";
            AssertTagsOnBracesOrSemicolons(file, 1, 4);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void TestClassWithMethodAndField()
        {
            var file = @"class C
{
    void M()
    {
    }

    int field;
}";
            AssertTagsOnBracesOrSemicolons(file, 0, 2);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void TestEmptyNamespace()
        {
            var file = @"namespace N
{
}";
            AssertTagsOnBracesOrSemicolons(file, 0);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void TestNamespaceAndClass()
        {
            var file = @"namespace N
{
    class C
    {
    }
}";
            AssertTagsOnBracesOrSemicolons(file, 1);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void TestNamespaceAndTwoClasses()
        {
            var file = @"namespace N
{
    class C
    {
    }

    class D
    {
    }
}";
            AssertTagsOnBracesOrSemicolons(file, 0, 2);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void TestNamespaceAndTwoClassesAndDelegate()
        {
            var file = @"namespace N
{
    class C
    {
    }

    class D
    {
    }

    delegate void Del();
}";
            AssertTagsOnBracesOrSemicolons(file, 0, 1, 3);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void TestNestedClass()
        {
            var file = @"class C
{
    class N
    {
    }
}";
            AssertTagsOnBracesOrSemicolons(file, 1);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void TestTwoNestedClasses()
        {
            var file = @"class C
{
    class N
    {
    }

    class N2
    {
    }
}";
            AssertTagsOnBracesOrSemicolons(file, 0, 2);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void TestStruct()
        {
            var file = @"struct S
{
}";
            AssertTagsOnBracesOrSemicolons(file, 0);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void TestInterface()
        {
            var file = @"interface I
{
}";
            AssertTagsOnBracesOrSemicolons(file, 0);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void TestEnum()
        {
            var file = @"enum E
{
}";
            AssertTagsOnBracesOrSemicolons(file, 0);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void TestProperty()
        {
            var file = @"class C
{
    int Prop
    {
        get
        {
            return 0;
        }
        set
        {
        }
    }
}";
            AssertTagsOnBracesOrSemicolons(file, 4);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void TestPropertyAndField()
        {
            var file = @"class C
{
    int Prop
    {
        get
        {
            return 0;
        }
        set
        {
        }
    }

    int field;
}";
            AssertTagsOnBracesOrSemicolons(file, 3, 5);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void TestClassWithFieldAndMethod()
        {
            var file = @"class C
{
    int field;

    void M()
    {
    }
}";
            AssertTagsOnBracesOrSemicolons(file, 0, 2);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void UsingDirective()
        {
            var file = @"using System;

class C
{
}";
            AssertTagsOnBracesOrSemicolons(file, 0, 1);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void UsingDirectiveInNamespace()
        {
            var file = @"namespace N
{
    using System;

    class C
    {
    }
}";
            AssertTagsOnBracesOrSemicolons(file, 0, 2);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void PropertyStyleEventDeclaration()
        {
            var file = @"class C
{
    event EventHandler E
    {
        add { }
        remove { }
    }

    int i;
}";
            AssertTagsOnBracesOrSemicolons(file, 2, 4);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void IndexerDeclaration()
        {
            var file = @"class C
{
    int this[int i]
    {
        get { return i; }
        set { }
    }

    int i;
}";
            AssertTagsOnBracesOrSemicolons(file, 3, 5);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void Constructor()
        {
            var file = @"class C
{
    C()
    {
    }

    int i;
}";
            AssertTagsOnBracesOrSemicolons(file, 0, 2);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void Destructor()
        {
            var file = @"class C
{
    ~C()
    {
    }

    int i;
}";
            AssertTagsOnBracesOrSemicolons(file, 0, 2);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void Operator()
        {
            var file = @"class C
{
    static C operator +(C lhs, C rhs)
    {
    }

    int i;
}";
            AssertTagsOnBracesOrSemicolons(file, 0, 2);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void ConversionOperator()
        {
            var file = @"class C
{
    static implicit operator C(int i)
    {
    }

    int i;
}";
            AssertTagsOnBracesOrSemicolons(file, 0, 2);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void Bug930292()
        {
            var file = @"class Program
{
void A() { }
void B() { }
void C() { }
void D() { }
}
";
            AssertTagsOnBracesOrSemicolons(file, 4);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void Bug930289()
        {
            var file = @"namespace Roslyn.Compilers.CSharp
{
internal struct ArrayElement<T>
{
internal T Value;
internal ArrayElement(T value) { this.Value = value; }
public static implicit operator ArrayElement<T>(T value) { return new ArrayElement<T>(value); }
}
}
";
            AssertTagsOnBracesOrSemicolons(file, 6);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void TestConsoleApp()
        {
            var file = @"using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
    }
}";
            AssertTagsOnBracesOrSemicolons(file, 2, 4);
        }

        #region Negative (incomplete) tests

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void IncompleteClass()
        {
            AssertTagsOnBracesOrSemicolons(@"class C");
            AssertTagsOnBracesOrSemicolons(@"class C {");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void IncompleteEnum()
        {
            AssertTagsOnBracesOrSemicolons(@"enum E");
            AssertTagsOnBracesOrSemicolons(@"enum E {");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void IncompleteMethod()
        {
            AssertTagsOnBracesOrSemicolons(@"void foo() {");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void IncompleteProperty()
        {
            AssertTagsOnBracesOrSemicolons(@"class C { int P { get; set; void");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void IncompleteEvent()
        {
            AssertTagsOnBracesOrSemicolons(@"public event EventHandler");
            AssertTagsOnBracesOrSemicolons(@"public event EventHandler {");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void IncompleteIndexer()
        {
            AssertTagsOnBracesOrSemicolons(@"int this[int i]");
            AssertTagsOnBracesOrSemicolons(@"int this[int i] {");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void IncompleteOperator()
        {
            // top level operators not supported in script code
            AssertTagsOnBracesOrSemicolonsTokens(@"C operator +(C lhs, C rhs) {", new int[0], Options.Regular);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void IncompleteConversionOperator()
        {
            AssertTagsOnBracesOrSemicolons(@"implicit operator C(int i) {");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.LineSeparators)]
        public void IncompleteMember()
        {
            AssertTagsOnBracesOrSemicolons(@"class C { private !C(");
        }

        #endregion

        private void AssertTagsOnBracesOrSemicolons(string contents, params int[] tokenIndices)
        {
            AssertTagsOnBracesOrSemicolonsTokens(contents, tokenIndices);
            AssertTagsOnBracesOrSemicolonsTokens(contents, tokenIndices, Options.Script);
        }

        private void AssertTagsOnBracesOrSemicolonsTokens(string contents, int[] tokenIndices, CSharpParseOptions options = null)
        {
            using (var workspace = CSharpWorkspaceFactory.CreateWorkspaceFromFile(contents, options))
            {
                var document = workspace.CurrentSolution.GetDocument(workspace.Documents.First().Id);
                var spans = new CSharpLineSeparatorService().GetLineSeparatorsAsync(document, document.GetSyntaxTreeAsync().Result.GetRoot().FullSpan, CancellationToken.None).Result;
                var tokens = document.GetCSharpSyntaxRootAsync(CancellationToken.None).Result.DescendantTokens().Where(t => t.Kind() == SyntaxKind.CloseBraceToken || t.Kind() == SyntaxKind.SemicolonToken);

                Assert.Equal(tokenIndices.Length, spans.Count());

                int i = 0;
                foreach (var span in spans.OrderBy(t => t.Start))
                {
                    var expectedToken = tokens.ElementAt(tokenIndices[i]);

                    var expectedSpan = expectedToken.Span;

                    var message = string.Format("Expected to match curly {0} at span {1}.  Actual span {2}",
                                                tokenIndices[i],
                                                expectedSpan,
                                                span);
                    Assert.True(expectedSpan == span, message);
                    ++i;
                }
            }
        }

        private static SyntaxToken GetOpenBrace(SyntaxTree syntaxTree, SyntaxToken token)
        {
            return token.Parent.ChildTokens().Where(n => n.Kind() == SyntaxKind.OpenBraceToken).Single();
        }
    }
}

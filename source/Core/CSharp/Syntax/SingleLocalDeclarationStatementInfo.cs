﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    public struct SingleLocalDeclarationStatementInfo : IEquatable<SingleLocalDeclarationStatementInfo>
    {
        private SingleLocalDeclarationStatementInfo(
            LocalDeclarationStatementSyntax statement,
            VariableDeclarationSyntax declaration,
            VariableDeclaratorSyntax declarator)
        {
            Statement = statement;
            Declaration = declaration;
            Declarator = declarator;
        }

        private static SingleLocalDeclarationStatementInfo Default { get; } = new SingleLocalDeclarationStatementInfo();

        public LocalDeclarationStatementSyntax Statement { get; }

        public VariableDeclarationSyntax Declaration { get; }

        public VariableDeclaratorSyntax Declarator { get; }

        public EqualsValueClauseSyntax Initializer
        {
            get { return Declarator?.Initializer; }
        }

        public SyntaxTokenList Modifiers
        {
            get { return Statement?.Modifiers ?? default(SyntaxTokenList); }
        }

        public TypeSyntax Type
        {
            get { return Declaration?.Type; }
        }

        public SyntaxToken Identifier
        {
            get { return Declarator?.Identifier ?? default(SyntaxToken); }
        }

        public string IdentifierText
        {
            get { return Identifier.ValueText; }
        }

        public SyntaxToken EqualsToken
        {
            get { return Initializer?.EqualsToken ?? default(SyntaxToken); }
        }

        public SyntaxToken SemicolonToken
        {
            get { return Statement?.SemicolonToken ?? default(SyntaxToken); }
        }

        public bool Success
        {
            get { return Declaration != null; }
        }

        internal static SingleLocalDeclarationStatementInfo Create(
            LocalDeclarationStatementSyntax localDeclarationStatement,
            bool allowMissing = false)
        {
            VariableDeclarationSyntax variableDeclaration = localDeclarationStatement?.Declaration;

            if (!Check(variableDeclaration, allowMissing))
                return Default;

            VariableDeclaratorSyntax variableDeclarator = variableDeclaration.Variables.SingleOrDefault(shouldthrow: false);

            if (!Check(variableDeclarator, allowMissing))
                return Default;

            return new SingleLocalDeclarationStatementInfo(localDeclarationStatement, variableDeclaration, variableDeclarator);
        }

        internal static SingleLocalDeclarationStatementInfo Create(
            VariableDeclarationSyntax variableDeclaration,
            bool allowMissing = false)
        {
            if (!Check(variableDeclaration, allowMissing))
                return Default;

            if (!(variableDeclaration.Parent is LocalDeclarationStatementSyntax localDeclarationStatement))
                return Default;

            VariableDeclaratorSyntax variableDeclarator = variableDeclaration.Variables.SingleOrDefault(shouldthrow: false);

            if (!Check(variableDeclarator, allowMissing))
                return Default;

            return new SingleLocalDeclarationStatementInfo(localDeclarationStatement, variableDeclaration, variableDeclarator);
        }

        internal static SingleLocalDeclarationStatementInfo Create(ExpressionSyntax value)
        {
            SyntaxNode node = value?.WalkUpParentheses().Parent;

            if (node?.Kind() != SyntaxKind.EqualsValueClause)
                return Default;

            if (!(node.Parent is VariableDeclaratorSyntax declarator))
                return Default;

            if (!(declarator.Parent is VariableDeclarationSyntax declaration))
                return Default;

            if (declaration.Variables.Count != 1)
                return Default;

            if (!(declaration.Parent is LocalDeclarationStatementSyntax localDeclarationStatement))
                return Default;

            return new SingleLocalDeclarationStatementInfo(localDeclarationStatement, declaration, declarator);
        }

        public override string ToString()
        {
            return Statement?.ToString() ?? base.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is SingleLocalDeclarationStatementInfo other && Equals(other);
        }

        public bool Equals(SingleLocalDeclarationStatementInfo other)
        {
            return EqualityComparer<LocalDeclarationStatementSyntax>.Default.Equals(Statement, other.Statement);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<LocalDeclarationStatementSyntax>.Default.GetHashCode(Statement);
        }

        public static bool operator ==(SingleLocalDeclarationStatementInfo info1, SingleLocalDeclarationStatementInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(SingleLocalDeclarationStatementInfo info1, SingleLocalDeclarationStatementInfo info2)
        {
            return !(info1 == info2);
        }
    }
}

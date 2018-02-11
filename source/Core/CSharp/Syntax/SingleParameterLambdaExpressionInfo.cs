﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    public struct SingleParameterLambdaExpressionInfo : IEquatable<SingleParameterLambdaExpressionInfo>
    {
        private SingleParameterLambdaExpressionInfo(
            LambdaExpressionSyntax lambdaExpression,
            ParameterSyntax parameter,
            CSharpSyntaxNode body)
        {
            LambdaExpression = lambdaExpression;
            Parameter = parameter;
            Body = body;
        }

        private static SingleParameterLambdaExpressionInfo Default { get; } = new SingleParameterLambdaExpressionInfo();

        public LambdaExpressionSyntax LambdaExpression { get; }

        public ParameterSyntax Parameter { get; }

        public CSharpSyntaxNode Body { get; }

        public ParameterListSyntax ParameterList
        {
            get { return (IsParenthesizedLambda) ? (ParameterListSyntax)Parameter.Parent : null; }
        }

        public bool IsSimpleLambda
        {
            get { return LambdaExpression?.Kind() == SyntaxKind.SimpleLambdaExpression; }
        }

        public bool IsParenthesizedLambda
        {
            get { return LambdaExpression?.Kind() == SyntaxKind.ParenthesizedLambdaExpression; }
        }

        public bool Success
        {
            get { return LambdaExpression != null; }
        }

        internal static SingleParameterLambdaExpressionInfo Create(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return CreateCore(Walk(node, walkDownParentheses) as LambdaExpressionSyntax, allowMissing);
        }

        internal static SingleParameterLambdaExpressionInfo Create(
            LambdaExpressionSyntax lambdaExpression,
            bool allowMissing = false)
        {
            return CreateCore(lambdaExpression, allowMissing);
        }

        internal static SingleParameterLambdaExpressionInfo CreateCore(
            LambdaExpressionSyntax lambdaExpression,
            bool allowMissing = false)
        {
            switch (lambdaExpression?.Kind())
            {
                case SyntaxKind.SimpleLambdaExpression:
                    {
                        var simpleLambda = (SimpleLambdaExpressionSyntax)lambdaExpression;

                        ParameterSyntax parameter = simpleLambda.Parameter;

                        if (!Check(parameter, allowMissing))
                            break;

                        CSharpSyntaxNode body = simpleLambda.Body;

                        if (!Check(body, allowMissing))
                            break;

                        return new SingleParameterLambdaExpressionInfo(simpleLambda, parameter, body);
                    }
                case SyntaxKind.ParenthesizedLambdaExpression:
                    {
                        var parenthesizedLambda = (ParenthesizedLambdaExpressionSyntax)lambdaExpression;

                        ParameterSyntax parameter = parenthesizedLambda
                            .ParameterList?
                            .Parameters
                            .SingleOrDefault(shouldthrow: false);

                        if (!Check(parameter, allowMissing))
                            break;

                        CSharpSyntaxNode body = parenthesizedLambda.Body;

                        if (!Check(body, allowMissing))
                            break;

                        return new SingleParameterLambdaExpressionInfo(parenthesizedLambda, parameter, body);
                    }
            }

            return Default;
        }

        public override string ToString()
        {
            return LambdaExpression?.ToString() ?? base.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is SingleParameterLambdaExpressionInfo other && Equals(other);
        }

        public bool Equals(SingleParameterLambdaExpressionInfo other)
        {
            return EqualityComparer<LambdaExpressionSyntax>.Default.Equals(LambdaExpression, other.LambdaExpression);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<LambdaExpressionSyntax>.Default.GetHashCode(LambdaExpression);
        }

        public static bool operator ==(SingleParameterLambdaExpressionInfo info1, SingleParameterLambdaExpressionInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(SingleParameterLambdaExpressionInfo info1, SingleParameterLambdaExpressionInfo info2)
        {
            return !(info1 == info2);
        }
    }
}

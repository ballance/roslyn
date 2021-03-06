﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGeneration;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.Shared.Extensions
{
    internal static class IParameterSymbolExtensions
    {
        public static bool IsRefOrOut(this IParameterSymbol symbol)
        {
            return symbol.RefKind != RefKind.None;
        }

        public static IParameterSymbol RenameParameter(this IParameterSymbol parameter, string parameterName)
        {
            return parameter.Name == parameterName
                ? parameter
                : CodeGenerationSymbolFactory.CreateParameterSymbol(
                        parameter.GetAttributes(),
                        parameter.RefKind,
                        parameter.IsParams,
                        parameter.Type,
                        parameterName,
                        parameter.IsOptional,
                        parameter.HasExplicitDefaultValue,
                        parameter.HasExplicitDefaultValue ? parameter.ExplicitDefaultValue : null);
        }

        public static IList<IParameterSymbol> RenameParameters(this IList<IParameterSymbol> parameters, IList<string> parameterNames)
        {
            var result = new List<IParameterSymbol>();
            for (int i = 0; i < parameterNames.Count; i++)
            {
                result.Add(parameters[i].RenameParameter(parameterNames[i]));
            }

            return result;
        }
    }
}

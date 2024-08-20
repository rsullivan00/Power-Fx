// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.PowerFx.Core.App.ErrorContainers;
using Microsoft.PowerFx.Core.Binding;
using Microsoft.PowerFx.Core.Errors;
using Microsoft.PowerFx.Core.Functions;
using Microsoft.PowerFx.Core.Localization;
using Microsoft.PowerFx.Core.Types;
using Microsoft.PowerFx.Core.Utils;
using Microsoft.PowerFx.Syntax;

namespace Microsoft.PowerFx.Core.Texl.Builtins
{
    // UntypedTable(expr, expr, ...)
    internal class UntypedTableFunction : BuiltinFunction
    {
        public override bool IsSelfContained => true;

        public override bool SupportsParamCoercion => false;

        public UntypedTableFunction()
            : base("UntypedTable", TexlStrings.AboutUntypedTable, FunctionCategories.Table, DType.UntypedObject, 0, 0, int.MaxValue)
        {
        }

        public override IEnumerable<TexlStrings.StringGetter[]> GetSignatures()
        {
            yield return new[] { TexlStrings.UntypedTableArg1 };
            yield return new[] { TexlStrings.UntypedTableArg1, TexlStrings.UntypedTableArg1 };
            yield return new[] { TexlStrings.UntypedTableArg1, TexlStrings.UntypedTableArg1, TexlStrings.UntypedTableArg1 };
        }

        public override IEnumerable<TexlStrings.StringGetter[]> GetSignatures(int arity)
        {
            if (arity > 2)
            {
                return GetGenericSignatures(arity, TexlStrings.UntypedTableArg1);
            }

            return base.GetSignatures(arity);
        }

        public override bool CheckTypes(CheckTypesContext context, TexlNode[] args, DType[] argTypes, IErrorContainer errors, out DType returnType, out Dictionary<TexlNode, DType> nodeToCoercedTypeMap)
        {
            Contracts.AssertValue(args);
            Contracts.AssertAllValues(args);
            Contracts.AssertValue(argTypes);
            Contracts.Assert(args.Length == argTypes.Length);
            Contracts.AssertValue(errors);
            Contracts.Assert(MinArity <= args.Length && args.Length <= MaxArity);

            var isValid = base.CheckTypes(context, args, argTypes, errors, out _, out nodeToCoercedTypeMap);

            returnType = new DType(DKind.Table, new TypeTree());
            returnType = returnType.ToTable();
            return isValid;
        }

        public override void CheckSemantics(TexlBinding binding, TexlNode[] args, DType[] argTypes, IErrorContainer errors)
        {
            base.CheckSemantics(binding, args, argTypes, errors);

            for (var i = 0; i < argTypes.Length; i++)
            {
                // show warning when the node is pageable as data could be truncated at this point
                if (argTypes[i].IsTableNonObjNull && binding.IsPageable(args[i]))
                {
                    errors.EnsureError(DocumentErrorSeverity.Warning, args[i], TexlStrings.ErrTruncatedArgWarning, args[i].ToString(), Name);
                    continue;
                }
            }
        }
    }
}

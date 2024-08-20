// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.PowerFx.Types;
using Xunit;

namespace Microsoft.PowerFx.Interpreter.Tests
{
    public class UntypedTableTests
    {
        [Fact]
        public async Task UntypedTableReturnsUntypedObject()
        {
            var pfxConfig = new PowerFxConfig();
            var recalcEngine = new RecalcEngine(pfxConfig);

            var table = await recalcEngine.EvalAsync("UntypedTable({}, {}, {})", CancellationToken.None);

            Assert.NotNull(table);

            var t = table.ToObject();

            Assert.NotNull(t);
            Assert.Equal(3, ((object[])t).Length);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using Microsoft.DotNet.Arcade.Sdk.Tests.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.DotNet.Arcade.Sdk.Tests
{
    public class GenerateResxSourceTests
    {
        private readonly ITestOutputHelper _output;

        public GenerateResxSourceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(true, false, false, "TestStrings.EmitFormatMethods.cs.txt")]
        [InlineData(false, false, false, "TestStrings.Default.cs.txt")]
        [InlineData(false, true, true, "TestStrings.AsConstants.cs.txt")]
        [InlineData(false, false, true, "TestStrings.OmitGetResourceString.cs.txt")]
        public void GeneratesCSharp(bool emitFormatMethods, bool asConstants, bool omitGetResourceString, string expectedFileName)
        {
            var resx = Path.Combine(AppContext.BaseDirectory, "testassets", "Resources", "TestStrings.resx");
            var actualFile = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName());

            var engine = new MockEngine(_output);
            var task = new GenerateResxSource
            {
                BuildEngine = engine,
                ResourceFile = resx,
                ResourceName = "Microsoft.DotNet.TestStrings",
                ResourceClassName = "Microsoft.DotNet.TestStrings",
                EmitFormatMethods = emitFormatMethods,
                AsConstants = asConstants,
                OmitGetResourceString = omitGetResourceString,
                Language = "C#",
                OutputPath = actualFile,
            };

            var expectedFile = Path.Combine(AppContext.BaseDirectory, "testassets", "Resources", expectedFileName);

            if (File.Exists(actualFile))
            {
                File.Delete(actualFile);
            }

            Assert.True(task.Execute(), "Task failed");

            Assert.Empty(engine.Warnings);

            Assert.True(File.Exists(actualFile), "Actual file does not exist");
            var actualFileContents = File.ReadAllText(actualFile);
            _output.WriteLine(actualFileContents);
            Assert.Equal(File.ReadAllText(expectedFile), actualFileContents, ignoreLineEndingDifferences: true);
        }
    }
}

﻿/**** Git Credential Manager for Windows ****
 *
 * Copyright (c) Microsoft Corporation
 * All rights reserved.
 *
 * MIT License
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the """"Software""""), to deal
 * in the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
 * the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN
 * AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE."
**/

using System;
using System.Collections.Generic;
using Moq;
using Xunit;

namespace Microsoft.Alm.Cli.Test
{
    public class ProgramTests
    {
        [Fact]
        public void LoadOperationArgumentsTest()
        {
            Program program = new Program();

            program._dieException = (Program caller, Exception e, string path, int line, string name) => Assert.False(true, $"Error: {e.ToString()}");
            program._dieMessage = (Program caller, string m, string path, int line, string name) => Assert.False(true, $"Error: {m}");
            program._exit = (Program caller, int e, string m, string path, int line, string name) => Assert.False(true, $"Error: {e} {m}");

            var envvars = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "HOME", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) }
            };
            var gitconfig = new Git.Configuration.Impl();
            var targetUri = new Authentication.TargetUri("https://example.visualstudio.com/");

            Mock<OperationArguments> opargsMock = new Mock<OperationArguments>();
            opargsMock.Setup(r => r.EnvironmentVariables)
                      .Returns(envvars);
            opargsMock.Setup(r => r.GitConfiguration)
                      .Returns(gitconfig);
            opargsMock.Setup(r => r.LoadConfiguration());
            opargsMock.Setup(r => r.TargetUri)
                      .Returns(targetUri);
            opargsMock.Setup(r => r.QueryUri)
                      .Returns(targetUri);

            var opargs = opargsMock.Object;

            program.LoadOperationArguments(opargs);

            Assert.NotNull(opargs);
        }

        [Fact]
        public void TryReadBooleanTest()
        {
            bool? yesno;

            var envvars = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "HOME", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) },
                { Program.EnvironPreserveCredentialsKey, "no" },
            };
            var gitconfig = new Git.Configuration.Impl();
            var targetUri = new Authentication.TargetUri("https://example.visualstudio.com/");

            Mock<OperationArguments> opargsMock = new Mock<OperationArguments>();
            opargsMock.Setup(r => r.EnvironmentVariables)
                      .Returns(envvars);
            opargsMock.Setup(r => r.GitConfiguration)
                      .Returns(gitconfig);
            opargsMock.Setup(r => r.LoadConfiguration());
            opargsMock.Setup(r => r.TargetUri)
                      .Returns(targetUri);
            opargsMock.Setup(r => r.QueryUri)
                      .Returns(targetUri);

            Program program = new Program();

            Assert.False(CommonFunctions.TryReadBoolean(program, opargsMock.Object, "notFound", "notFound", out yesno));
            Assert.False(yesno.HasValue);

            Assert.True(CommonFunctions.TryReadBoolean(program, opargsMock.Object, Program.ConfigPreserveCredentialsKey, Program.EnvironPreserveCredentialsKey, out yesno));
            Assert.True(yesno.HasValue);
            Assert.False(yesno.Value);

            envvars = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "HOME", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) },
                { Program.EnvironPreserveCredentialsKey, "yes" },
            };
            opargsMock.Setup(r => r.EnvironmentVariables)
                      .Returns(envvars);

            Assert.True(CommonFunctions.TryReadBoolean(program, opargsMock.Object, Program.ConfigPreserveCredentialsKey, Program.EnvironPreserveCredentialsKey, out yesno));
            Assert.True(yesno.HasValue);
            Assert.True(yesno.Value);

            envvars = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "HOME", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) },
                { Program.EnvironPreserveCredentialsKey, string.Empty },
            };
            opargsMock.Setup(r => r.EnvironmentVariables)
                      .Returns(envvars);

            Assert.False(CommonFunctions.TryReadBoolean(program, opargsMock.Object, Program.ConfigPreserveCredentialsKey, Program.EnvironPreserveCredentialsKey, out yesno));
            Assert.False(yesno.HasValue);
        }
    }
}

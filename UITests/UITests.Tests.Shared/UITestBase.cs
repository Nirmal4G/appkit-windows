// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

#if USING_TAEF
using WEX.Logging.Interop;
using WEX.TestExecution;
using WEX.TestExecution.Markup;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

using Windows.UI.Xaml.Tests.MUXControls.InteractionTests.Common;
using Windows.UI.Xaml.Tests.MUXControls.InteractionTests.Infra;

namespace UITests.Tests
{
    public abstract class UITestBase
    {
        private TestSetupHelper helper;

        internal static TestApplicationInfo WinUICSharpUWPSampleApp
        {
            get
            {
                try
                {
                    string assemblyDir = Path.GetDirectoryName(typeof(TestApplicationInfo).Assembly.Location);
                    var baseDir = Directory.GetParent(assemblyDir);

                    for (int i = 0; i < 10; i++)
                    {
                        if (baseDir.EnumerateDirectories().Any(d => d.Name.Equals("~build", StringComparison.OrdinalIgnoreCase)))
                        {
                            break;
                        }

                        baseDir = baseDir.Parent;
                    }

                    string publishDir = Path.Combine(baseDir.FullName, "~publish");

                    Log.Comment($"Base Package Search Directory = \"{publishDir}\"");

                    var exclude = new[] { "Microsoft.NET.CoreRuntime", "Microsoft.VCLibs", "Microsoft.UI.Xaml", "Microsoft.NET.CoreFramework.Debug" };
                    var files = Directory.EnumerateFiles(publishDir, "*.msix", SearchOption.AllDirectories).Where(f => !exclude.Any(Path.GetFileNameWithoutExtension(f).Contains));

                    if (!files.Any())
                    {
                        throw new Exception($"Failed to find '*.msix' in {publishDir}'!");
                    }

                    string mostRecentlyBuiltPackage = string.Empty;
                    DateTime timeMostRecentlyBuilt = DateTime.MinValue;

                    foreach (string file in files)
                    {
                        DateTime fileWriteTime = File.GetLastWriteTime(file);

                        if (fileWriteTime > timeMostRecentlyBuilt)
                        {
                            timeMostRecentlyBuilt = fileWriteTime;
                            mostRecentlyBuiltPackage = file;
                        }
                    }

                    return new TestApplicationInfo(
                        testAppPackageName: "UITests.App",
                        testAppName: "3568ebdf-5b6b-4ddd-bb17-462d614ba50f_yeyc6z1eztrme!App",
                        testAppPackageFamilyName: "3568ebdf-5b6b-4ddd-bb17-462d614ba50f_yeyc6z1eztrme",
                        testAppMainWindowTitle: "UITests.App",
                        processName: "UITests.App.exe",
                        installerName: mostRecentlyBuiltPackage.Replace(".msix", string.Empty),
                        certSerialNumber: "24d62f3b13b8b9514ead9c4de48cc30f7cc6151d",
                        baseAppxDir: publishDir);
                }
                catch (Exception e)
                {
                    throw new AggregateException("Can't get Test Application info from the published MSIX packages. Check Output paths.", e);
                }
            }
        }

        private static TestSetupHelper.TestSetupHelperOptions TestSetupHelperOptions
            => new()
            {
                AutomationIdOfSafeItemToClick = string.Empty
            };

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public async Task TestInitialize()
        {
            // This will reset the test for each run (as from original WinUI https://github.com/microsoft/microsoft-ui-xaml/blob/master/test/testinfra/MUXTestInfra/Infra/TestHelpers.cs)
            // We construct it so it doesn't try to run any tests since we use the AppService Bridge to complete our loading.
            helper = new TestSetupHelper(new string[] { }, TestSetupHelperOptions);

            var pageName = GetPageForTest(TestContext);

            var rez = await TestAssembly.OpenPage(pageName);

            if (!rez)
            {
                // Error case, we didn't get confirmation of test starting.
                throw new InvalidOperationException("Test host didn't confirm test ready to execute page: " + pageName);
            }

            Log.Comment("[Harness] Received Host Ready with Page: {0}", pageName);
            Wait.ForIdle();
            Log.Comment("[Harness] Starting Test for {0}...", pageName);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            helper.Dispose();
        }

        private static string GetPageForTest(TestContext testContext)
        {
#if USING_TAEF
            var fullTestName = testContext.TestName;
            var lastDotIndex = fullTestName.LastIndexOf('.');
            var testName = fullTestName.Substring(lastDotIndex + 1);
            var theClassName = fullTestName.Substring(0, lastDotIndex);
#else
            var testName = testContext.TestName;
            var theClassName = testContext.FullyQualifiedTestClassName;
#endif
            var testClassString = $"test class \"{theClassName}\"";
            if (Type.GetType(theClassName) is not Type type)
            {
                throw new Exception($"Could not find {testClassString}.");
            }

            Log.Comment($"Found {testClassString}.");

            var testMethodString = $"test method \"{testName}\" in {testClassString}";
            if (type.GetMethod(testName) is not MethodInfo method)
            {
                throw new Exception($"Could not find {testMethodString}.");
            }

            Log.Comment($"Found {testMethodString}.");

            var testPageAttributeString = $"\"{typeof(TestPageAttribute)}\" on {testMethodString}";
            if (method.GetCustomAttribute(typeof(TestPageAttribute), true) is not TestPageAttribute attribute)
            {
                throw new Exception($"Could not find {testPageAttributeString}.");
            }

            Log.Comment($"Found {testPageAttributeString}. {nameof(TestPageAttribute.XamlFile)}: {attribute.XamlFile}.");

            return attribute.XamlFile;
        }
    }
}
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Microsoft.VisualStudio.TestPlatform.Extension.Exercism.TestLogger
{
    using System;
    using System.Collections.Generic;
    using ObjectModel;
    using ObjectModel.Client;

    [FriendlyName(FriendlyName)]
    [ExtensionUri(ExtensionUri)]
    public class ExercismTestLogger : ITestLoggerWithParameters
    {
        private const string ExtensionUri = "logger://Microsoft/TestPlatform/ExercismTestLogger/v1";
        private const string FriendlyName = "exercism";

        private string _testRunDirectory;

        public void Initialize(TestLoggerEvents events, string testRunDirectory)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            if (string.IsNullOrEmpty(testRunDirectory))
                throw new ArgumentNullException(nameof(testRunDirectory));

            events.TestRunMessage += TestMessageHandler;
            events.TestResult += TestResultHandler;
            events.TestRunComplete += TestRunCompleteHandler;

            _testRunDirectory = testRunDirectory;
        }

        private void TestMessageHandler(object sender, TestRunMessageEventArgs e)
        {
            Console.WriteLine(_testRunDirectory);
//            throw new NotImplementedException();
        }

        private void TestResultHandler(object sender, TestResultEventArgs e)
        {
//            throw new NotImplementedException();
        }

        private void TestRunCompleteHandler(object sender, TestRunCompleteEventArgs e)
        {
            // dotnet test --logger:exercism --test-adapter-path:C:\Programmeren\exercism\ExercismLogger\src\Exercism.Logger\bin\Debug\netstandard1.5
            
            var json = new StringBuilder();
            json.Append("[");
            json.Append("{");
            json.Append("name");
            json.Append(":");
            json.Append("\"Test that the thing works\"");
            json.Append("}");
            json.Append("]");

            var testResultsFilePath = Path.Combine(_testRunDirectory, "test-results.json");

            if (!Directory.Exists(_testRunDirectory))
                Directory.CreateDirectory(_testRunDirectory);
            
            File.WriteAllText(testResultsFilePath, json.ToString());
            
//           [ {
//                name: "Test that the thing works" ,
//                passed: false,
//                expected: "42",
//                output: "123123",
//                error: nil
//            } ]
        }

        public void Initialize(TestLoggerEvents events, Dictionary<string, string> parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if (parameters.Count == 0)
                throw new ArgumentException("No default parameters added", nameof(parameters));

            Initialize(events, parameters[DefaultLoggerParameterNames.TestRunDirectory]);
        }
    }
}
using System.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Microsoft.VisualStudio.TestPlatform.Extension.Exercism.TestLogger
{
    using System;
    using System.Collections.Generic;
    using ObjectModel;
    using ObjectModel.Client;

    internal class ExercismTestResult
    {
        public string Name { get; }
        public string Error { get; }

        public bool Passed => Error == null;

        public ExercismTestResult(string name, string error)
        {
            Name = name;
            Error = error;
        }   
    }
    
    [FriendlyName(FriendlyName)]
    [ExtensionUri(ExtensionUri)]
    public class ExercismTestLogger : ITestLoggerWithParameters
    {
        private const string ExtensionUri = "logger://Microsoft/TestPlatform/ExercismTestLogger/v1";
        private const string FriendlyName = "exercism";

        private string _testRunDirectory;
        private List<ExercismTestResult> _testResults;

        public void Initialize(TestLoggerEvents events, string testRunDirectory)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            if (string.IsNullOrEmpty(testRunDirectory))
                throw new ArgumentNullException(nameof(testRunDirectory));

            _testRunDirectory = testRunDirectory;
            _testResults = new List<ExercismTestResult>();
            
            events.TestResult += TestResultHandler;
            events.TestRunComplete += TestRunCompleteHandler;
        }

        private void TestResultHandler(object sender, TestResultEventArgs e)
        {
            if (e.Result.Outcome == TestOutcome.Passed ||
                e.Result.Outcome == TestOutcome.Failed)
                _testResults.Add(new ExercismTestResult(e.Result.DisplayName, e.Result.ErrorMessage));
        }

        private void TestRunCompleteHandler(object sender, TestRunCompleteEventArgs e)
        {
            // dotnet test --logger:exercism --test-adapter-path:C:\Programmeren\exercism\ExercismLogger\src\Exercism.Logger\bin\Debug\netstandard1.5
            
            if (!Directory.Exists(_testRunDirectory))
                Directory.CreateDirectory(_testRunDirectory);

            using (var streamWriter = File.CreateText(Path.Combine(_testRunDirectory, "test-results.json")))
            {
                streamWriter.Write("[");

                for (var i = 0; i < _testResults.Count; i++)
                {
                    var name = $"\"{_testResults[i].Name}\"";
                    var error = _testResults[i].Error == null ? "null" : $"\"{_testResults[i].Error}\"";
                    var passed = _testResults[i].Passed.ToString().ToLower();
                    
                    streamWriter.Write("{{\"name\": {0}, \"error\": {1}, \"passed\": {2}}}", name, error, passed);
                
                    if (i < _testResults.Count - 1)
                        streamWriter.Write("," );
                }
            
                streamWriter.Write("]");
                streamWriter.Flush();
            }
            
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
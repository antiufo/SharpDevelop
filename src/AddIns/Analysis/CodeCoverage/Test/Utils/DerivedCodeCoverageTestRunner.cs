﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

//using System;
//using System.Diagnostics;
//using ICSharpCode.CodeCoverage;
//using ICSharpCode.UnitTesting;
//
//namespace ICSharpCode.CodeCoverage.Tests.Utils
//{
//	public class DerivedCodeCoverageTestRunner : CodeCoverageTestRunner
//	{
//		public string ParseStringReturnValue;
//		public string ParseStringParameter;
//		
//		public DerivedCodeCoverageTestRunner(IUnitTestProcessRunner processRunner,
//			ITestResultsMonitor testResultsMonitor,
//			UnitTestingOptions options,
//			IFileSystem fileSystem,
//			IUnitTestMessageService messageService)
//			: base(new CodeCoverageTestRunnerContext(processRunner, testResultsMonitor, fileSystem, messageService, options))
//		{
//		}
//		
//		public ProcessStartInfo CallGetProcessStartInfo(SelectedTests selectedTests)
//		{
//			return base.GetProcessStartInfo(selectedTests);
//		}
//		
//		public TestResult CallCreateTestResultForTestFramework(TestResult testResult)
//		{
//			return base.CreateTestResultForTestFramework(testResult);
//		}
//		
//		protected override string ParseString(string text)
//		{
//			ParseStringParameter = text;
//			return ParseStringReturnValue;
//		}
//	}
//}

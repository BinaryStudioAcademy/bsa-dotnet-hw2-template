using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using CoolParking.BL.Interfaces;
using CoolParking.BL.Services;
using Xunit;

namespace CoolParking.BL.Tests
{
    public class LogServiceTests : IDisposable
    {
        private readonly string _logFilePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\Transactions.log";
        private readonly ILogService _logService;

        public LogServiceTests()
        {
            _logService = new LogService(_logFilePath);
        }

        public void Dispose()
        {
            File.Delete(_logFilePath);
        }

        [Fact]
        public void Write_WhenWriteTwice_ThenReadTwoLines()
        {
            const string testStr = "Test string.";
            _logService.Write(testStr);
            _logService.Write(testStr);

            string actual;
            using (var file = new StreamReader(_logFilePath))
            {
                actual = file.ReadToEnd();
            }

            Assert.Equal(2, Regex.Matches(actual, testStr).Count);
            Assert.Equal(2, Regex.Matches(actual, "\n").Count);
        }

        [Fact]
        public void Read_WhenExistFile_ThenReadFileContains()
        {
            const string testStr = "Test string.";
            using (var file = new StreamWriter(_logFilePath, true))
            {
                file.WriteLine(testStr);
                file.WriteLine(testStr);
            }

            var actual = _logService.Read();

            Assert.Equal(2, Regex.Matches(actual, testStr).Count);
            Assert.Equal(2, Regex.Matches(actual, "\n").Count);
        }
    }
}
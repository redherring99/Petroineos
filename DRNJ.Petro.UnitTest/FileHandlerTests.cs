using DRNJ.Petro.Components.IO;
using DRNJ.Petro.UnitTest.Builders;
using FluentAssertions;
using Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DRNJ.Petro.UnitTest
{
    /// <summary>
    /// Unit Tests for FileHandler Class
    /// 
    /// </summary>

    public class FileHandlerTests
    {

        /// <summary>
        /// Check What's written to the file
        /// </summary>
        [Fact]
        public void Test_CSV_Write_File_Contents()
        {
            // *************************************************************************************************
            // Arrange
            // *************************************************************************************************
            string callbackfileName = string.Empty;
            List<double> callbackFileContents = null;
            List<string> fileContentsCallBack = new List<string>();
            var fakeLogger = new FakeLoggerBuilder().Build<FileHandler>();
            var fakeStream = new FakeStreamWrapperBuilder()
                                .WithWritelineCallBack(a =>fileContentsCallBack.Add(a))
                                .Build();
            var fh = new FileHandler(fakeLogger,fakeStream);

            //-----------------------------------------
            // Create dummy data to write to the file |
            // Could use NBuilder for this            |
            //-----------------------------------------

            List<PowerPeriod> dummyData = new List<PowerPeriod>();

            for (int i = 1; i <= 24; i++)
            {
                dummyData.Add(new PowerPeriod { Period = i, Volume = i * 10 });
            }

            // *************************************************************************************************
            // Act
            // *************************************************************************************************

            fh.WriteCsv("abcd", dummyData);

            // *************************************************************************************************
            // Assert
            // *************************************************************************************************

            //---------------------------
            // Header + 24 rows written |
            //---------------------------
            fileContentsCallBack.Count.Should().Be(25);

            for (int period = 1; period <= 24; period ++)
            {
                fileContentsCallBack[period].Should().Be(string.Format("{0},{1}",
                    period == 1 ? "23:00" : string.Format("{0:00}:00", period - 2), 
                    period * 10));
            }
 

        }
    }
}

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
                                .WithWritelineCallBack(a => fileContentsCallBack.Add(a))
                                .Build();
            var fh = new FileHandler(fakeLogger, fakeStream);

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

            for (int period = 1; period <= 24; period++)
            {
                fileContentsCallBack[period].Should().Be(string.Format("{0},{1}",
                    period == 1 ? "23:00" : string.Format("{0:00}:00", period - 2),
                    period * 10));
            }


        }

        /// <summary>
        /// Check What's written to the file as per pdf specification
        /// Pdf show data and expected file contents - test that here
        /// </summary>
        [Fact]
        public void Test_CSV_Write_File_Contents_As_Per_Spec()
        {
            // *************************************************************************************************
            // Arrange
            // *************************************************************************************************
            string callbackfileName = string.Empty;
            List<double> callbackFileContents = null;
            List<string> fileContentsCallBack = new List<string>();
            var fakeLogger = new FakeLoggerBuilder().Build<FileHandler>();
            var fakeStream = new FakeStreamWrapperBuilder()
                                .WithWritelineCallBack(a => fileContentsCallBack.Add(a))
                                .Build();
            var fh = new FileHandler(fakeLogger, fakeStream);

            //-----------------------------------------
            // Create dummy data to write to the file |
            // Could use NBuilder for this            |
            //-----------------------------------------

            List<PowerPeriod> dummyData = new List<PowerPeriod>();

            #region As per PDF Spec
            dummyData.Add(new PowerPeriod { Period = 1, Volume = 150 });
            dummyData.Add(new PowerPeriod { Period = 2, Volume = 150 });
            dummyData.Add(new PowerPeriod { Period = 3, Volume = 150 });
            dummyData.Add(new PowerPeriod { Period = 4, Volume = 150 });
            dummyData.Add(new PowerPeriod { Period = 5, Volume = 150 });
            dummyData.Add(new PowerPeriod { Period = 6, Volume = 150 });
            dummyData.Add(new PowerPeriod { Period = 7, Volume = 150 });
            dummyData.Add(new PowerPeriod { Period = 8, Volume = 150 });
            dummyData.Add(new PowerPeriod { Period = 9, Volume = 150 });
            dummyData.Add(new PowerPeriod { Period = 10, Volume = 150 });
            dummyData.Add(new PowerPeriod { Period = 11, Volume = 150 });
            dummyData.Add(new PowerPeriod { Period = 12, Volume = 80 });
            dummyData.Add(new PowerPeriod { Period = 13, Volume = 80 });
            dummyData.Add(new PowerPeriod { Period = 14, Volume = 80 });
            dummyData.Add(new PowerPeriod { Period = 15, Volume = 80 });
            dummyData.Add(new PowerPeriod { Period = 16, Volume = 80 });
            dummyData.Add(new PowerPeriod { Period = 17, Volume = 80 });
            dummyData.Add(new PowerPeriod { Period = 18, Volume = 80 });
            dummyData.Add(new PowerPeriod { Period = 19, Volume = 80 });
            dummyData.Add(new PowerPeriod { Period = 20, Volume = 80 });
            dummyData.Add(new PowerPeriod { Period = 21, Volume = 80 });
            dummyData.Add(new PowerPeriod { Period = 22, Volume = 80 });
            dummyData.Add(new PowerPeriod { Period = 23, Volume = 80 });
            dummyData.Add(new PowerPeriod { Period = 24, Volume = 80 });
            #endregion

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

            fileContentsCallBack[1].Should().Be("23:00,150");

            for (int period = 1; period <= 24; period++)
            {
                fileContentsCallBack[period].Should().Be(string.Format("{0},{1}",
                    period == 1 ? "23:00" : string.Format("{0:00}:00", period - 2),
                    dummyData[period-1].Volume));
            }



        }

    }
}

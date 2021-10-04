using DRNJ.Petro.Components.Aggregator;
using DRNJ.Petro.UnitTest.Accessor;
using DRNJ.Petro.UnitTest.Builders;
using FluentAssertions;
using NSubstitute;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DRNJ.Petro.UnitTest
{
    /// <summary>
    /// Tests for Aggregator
    /// </summary>
    public class AggregatorTests
    {
        /// <summary>
        /// Used to try to work out clean LINQ to do aggregate summation !
        /// 
        /// I'm sure I can do something simpler and perhaps use .Aggregate LINQ method
        /// 
        /// </summary>
        [Fact]
        public void Test_Aggregator_Aggregation_Totals()
        {
            // *************************************************************************************************
            // Arrange
            // *************************************************************************************************

            List<PowerTrade> p = new List<PowerTrade>();

            //-----------------------------------
            // Create fake PowerTrade structure |
            //-----------------------------------
            var pt = PowerTrade.Create(DateTime.Now, 24);
            for (int i = 0; i < 24; i++)
            {
                pt.Periods[i].Period = i + 1;
                pt.Periods[i].Volume = i * 10;
            }
            p.Add(pt);

            pt = PowerTrade.Create(DateTime.Now, 24);
            for (int i = 0; i < 24; i++)
            {
                pt.Periods[i].Period = i + 1;
                pt.Periods[i].Volume = i * 10;
            }
            p.Add(pt);

            var fakeLogger = new FakeLoggerBuilder().Build<Aggregator>();
            var fakeFileHandler = new FakeFileHandlerBuilder().Build();
            var fakeService = new FakePowerServiceBuilder().Build();

            var aggregator = new Aggregator(fakeLogger, fakeService, fakeFileHandler, "abcd");

            //----------------------------------------------------------------------------------
            // A bit of magic so we can call a protected method rather than use a facade class |
            //----------------------------------------------------------------------------------
            dynamic target = new DynamicAccessor(aggregator, typeof(Aggregator));

            // *************************************************************************************************
            // Act
            // *************************************************************************************************

            var volume = (List<double>)target.AggregateResults(p);

            // *************************************************************************************************
            // Assert
            // *************************************************************************************************

            volume.Count().Should().Be(24);
            for (int period = 1; period <= 24; period++)
            {
                volume[period - 1].Should().Be((double)((period - 1) * 10 * 2));
            }

        }

        /// <summary>
        /// Check filename and path created correctly
        /// </summary>
        [Fact]
        public void Test_CSV_Write_FileName_and_Path()
        {
            // *************************************************************************************************
            // Arrange
            // *************************************************************************************************
            string callbackfileName = string.Empty;
            List<double> callbackFileContents = null;

            var fakeLogger = new FakeLoggerBuilder().Build<Aggregator>();
            var fakeFileHandler = new FakeFileHandlerBuilder()
                .WithFileSaveCallback((x,y) =>
                {
                    callbackfileName = x;
                    callbackFileContents = y;
                })
                .Build();


           var fakeService = new FakePowerServiceBuilder().Build();

            var aggregator = new Aggregator(fakeLogger, fakeService, fakeFileHandler, "abcd");

            //----------------------------------------------------------------------------------
            // A bit of magic so we can call a protected method rather than use a facade class |
            //----------------------------------------------------------------------------------
            dynamic target = new DynamicAccessor(aggregator, typeof(Aggregator));

            // *************************************************************************************************
            // Act
            // *************************************************************************************************

            target.WriteDataToFile(DateTime.Parse("01-Feb-2021 18:37"),new List<double>());

            // *************************************************************************************************
            // Assert
            // *************************************************************************************************

            (callbackFileContents != null).Should().BeTrue();
            callbackfileName.Should().Be("abcd\\20210201_1837.csv");

        }

    }



}

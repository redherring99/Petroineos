using DRNJ.Petro.Components.Aggregate;
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

            List<PowerTrade> FakePowerTrades = new List<PowerTrade>();

            #region Build Fake Power Trades
            //-----------------------------------
            // Create fake PowerTrade structure |
            //-----------------------------------
            var pt = PowerTrade.Create(DateTime.Now, 24);
            for (int i = 0; i <= 23; i++)
            {
                pt.Periods[i].Volume = (i + 1) * 10;
            }
            FakePowerTrades.Add(pt);

            pt = PowerTrade.Create(DateTime.Now, 24);
            for (int i = 0; i <= 23; i++)
            {
                pt.Periods[i].Volume = (i + 1) * 20;
            }
            FakePowerTrades.Add(pt);
            #endregion

            //--------------------------------------------
            // Use Builder pattern to build fake objects |
            //--------------------------------------------
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

            var res = (List<PowerPeriod>)target.AggregateResults(FakePowerTrades);

            // *************************************************************************************************
            // Assert
            // *************************************************************************************************

            res.Count().Should().Be(24);
            for (int period = 0; period <= 23; period++)
            {
                res[period].Period.Should().Be(period + 1);
                res[period].Volume.Should().Be((double)((period + 1) * 30));
            }

        }

        [Fact]
        public void Test_Aggregator_Aggregation_Totals_As_Per_Spec()
        {
            // *************************************************************************************************
            // Arrange
            // *************************************************************************************************

            List<PowerTrade> FakePowerTrades = new List<PowerTrade>();

            #region Build Fake Power Trades - Data as per spec
            //-----------------------------------
            // Create fake PowerTrade structure |
            //-----------------------------------
            var pt = PowerTrade.Create(DateTime.Now, 24);

            #region First Example
            // First example is all 100
            for (int i = 0; i <= 23; i++)
            {
                pt.Periods[i].Volume = 100;
            }
            #endregion
            FakePowerTrades.Add(pt);
            #region Second Example
            pt = PowerTrade.Create(DateTime.Now, 24);
            pt.Periods[0].Volume = 50;
            pt.Periods[1].Volume = 50;
            pt.Periods[2].Volume = 50;
            pt.Periods[3].Volume = 50;
            pt.Periods[4].Volume = 50;
            pt.Periods[5].Volume = 50;
            pt.Periods[6].Volume = 50;
            pt.Periods[7].Volume = 50;
            pt.Periods[8].Volume = 50;
            pt.Periods[9].Volume = 50;
            pt.Periods[10].Volume = 50;
            pt.Periods[11].Volume = -20;
            pt.Periods[12].Volume = -20;
            pt.Periods[13].Volume = -20;
            pt.Periods[14].Volume = -20;
            pt.Periods[15].Volume = -20;
            pt.Periods[16].Volume = -20;
            pt.Periods[17].Volume = -20;
            pt.Periods[18].Volume = -20;
            pt.Periods[19].Volume = -20;
            pt.Periods[20].Volume = -20;
            pt.Periods[21].Volume = -20;
            pt.Periods[22].Volume = -20;
            pt.Periods[23].Volume = -20;
            #endregion
            FakePowerTrades.Add(pt);

            #endregion

            //--------------------------------------------
            // Use Builder pattern to build fake objects |
            //--------------------------------------------
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

            var res = (List<PowerPeriod>)target.AggregateResults(FakePowerTrades);

            // *************************************************************************************************
            // Assert
            // *************************************************************************************************

            res.Count().Should().Be(24);

            res[0].Volume.Should().Be(150);
            res[1].Volume.Should().Be(150);
            res[2].Volume.Should().Be(150);
            res[3].Volume.Should().Be(150);
            res[4].Volume.Should().Be(150);
            res[5].Volume.Should().Be(150);
            res[6].Volume.Should().Be(150);
            res[7].Volume.Should().Be(150);
            res[8].Volume.Should().Be(150);
            res[9].Volume.Should().Be(150);
            res[10].Volume.Should().Be(150);
            res[11].Volume.Should().Be(80);
            res[12].Volume.Should().Be(80);
            res[13].Volume.Should().Be(80);
            res[14].Volume.Should().Be(80);
            res[15].Volume.Should().Be(80);
            res[16].Volume.Should().Be(80);
            res[17].Volume.Should().Be(80);
            res[18].Volume.Should().Be(80);
            res[19].Volume.Should().Be(80);
            res[20].Volume.Should().Be(80);
            res[21].Volume.Should().Be(80);
            res[22].Volume.Should().Be(80);
            res[23].Volume.Should().Be(80);


        }


        /// <summary>
        /// Check filename and path created correctly
        /// Use Example data and expected totals from original sspec
        /// </summary>
        [Fact]
        public void Test_CSV_Write_FileName_and_Path()
        {
            // *************************************************************************************************
            // Arrange
            // *************************************************************************************************
            string callbackfileName = string.Empty;
            IList<PowerPeriod> callbackFileContents = null;

            var fakeLogger = new FakeLoggerBuilder().Build<Aggregator>();
            var fakeFileHandler = new FakeFileHandlerBuilder()
                .WithFileSaveCallback((x, y) =>
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

            target.WriteDataToFile(DateTime.Parse("01-Feb-2021 18:07"), new List<PowerPeriod>());

            // *************************************************************************************************
            // Assert
            // *************************************************************************************************

            (callbackFileContents != null).Should().BeTrue();
            callbackfileName.Should().Be("abcd\\PowerPosition_20210201_1807.csv");

        }

    }



}

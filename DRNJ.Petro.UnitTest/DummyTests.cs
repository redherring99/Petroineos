using FluentAssertions;
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
    public class DummyTests
    {
        /// <summary>
        /// Dummy test to show the layout of A/A/A tests
        /// </summary>
        [Fact]
        public void Test_Aggregator_dummy()
        {
            // *************************************************************************************************
            // Arrange
            // *************************************************************************************************

            bool b;

            // *************************************************************************************************
            // Act
            // *************************************************************************************************

            b = true;

            // *************************************************************************************************
            // Assert
            // *************************************************************************************************

            b.Should().Be(true);
        }


        /// <summary>
        /// Used to try to work out clean LINQ to do aggregate summation !
        /// 
        /// I'm sure I can do something simpler and perhaps use .Aggregate LINQ method
        /// 
        /// </summary>
        [Fact]
        public void Test_Aggregation_Totals_LINQ_Calc()
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

            // *************************************************************************************************
            // Act
            // *************************************************************************************************

            List<double> volume = new List<double>();


            for (int period = 1; period <= 24; period++)
            {
                var totalVol = p.Sum(a => a.Periods.Where(c => c.Period == period).First().Volume);
                volume.Add(totalVol);
            }


            // *************************************************************************************************
            // Assert
            // *************************************************************************************************

            volume.Count().Should().Be(24);
            for (int period = 1; period <= 24; period++)
            {
                volume[period - 1].Should().Be((double)((period - 1) * 10 * 2));
            }
        }

        [Fact]
        public void Test_Filename_Creation()
        {
            // *************************************************************************************************
            // Arrange
            // *************************************************************************************************

            DateTime dt = DateTime.Parse("01-Feb-2021 18:37");

            // *************************************************************************************************
            // Act
            // *************************************************************************************************

            string s = String.Format("{0:yyyyMMdd_HHm}", dt);

            // *************************************************************************************************
            // Assert
            // *************************************************************************************************

            s.Should().Be("20210201_1837");
        }
    }
}

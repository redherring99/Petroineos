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
    public class DevelopmentTests
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
               // x.GroupBy(c=>c.)
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
        public void Test_Aggregation_Totals_LINQ_Calc2()
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

            var res =  (from g in p.SelectMany(a => a.Periods).GroupBy(b => b.Period)
                        select new PowerPeriod { Period = g.Key, Volume = g.Sum(a => a.Volume) }).ToList();


            // *************************************************************************************************
            // Assert
            // *************************************************************************************************

            res.Count().Should().Be(24);
            for (int period = 1; period <= 24; period++)
            {
                res.Where(a => a.Period == period).First().Volume.Should().Be((double)((period - 1) * 10 * 2));
            }
        }


        public class PT
        {

            public PT(int p, int v)
            {
                this.Period = p;
                this.Volume = v;
            }
            public int Period { get; set; }
            public int Volume { get; set; }
        }


        [Fact]
        public void Test_Aggregation_Totals_LINQ_Calc3()
        {

            List<Tuple<int, int>> t = new List<Tuple<int, int>>();

            t.Add(new Tuple<int, int>(1, 10));
            t.Add(new Tuple<int, int>(1, 10));
            t.Add(new Tuple<int, int>(2, 10));

            var r = t.GroupBy(x => x.Item1);


            var zz = (from z in t group z by z.Item1 into g select new { Id = g.Key, S = g.Sum(c => c.Item2)}).ToList();

            List<PT> p1 = new List<PT>();
            p1.Add(new PT(1, 10));
            p1.Add(new PT(2, 10));

            List<PT> p2 = new List<PT>();
            p2.Add(new PT(1, 20));
            p2.Add(new PT(2, 30));

            List<List<PT>> p3 = new List<List<PT>>();

            p3.Add(p1);
            p3.Add(p2);


            var p7 = (from g in p3.SelectMany(a => a).GroupBy(b => b.Period)
                     select new PT(g.Key, g.Sum(a => a.Volume))).ToList();

            var p4 = p3.SelectMany(a => a.GroupBy(b => b.Period)).ToList();

            var p6 = p3.SelectMany(a => a).ToList();



            var p5 = p3.GroupBy(a => a.Select(b => b.Period)).ToList();



           // *************************************************************************************************
           // Arrange
           // *************************************************************************************************

           List < PowerTrade > p = new List<PowerTrade>();

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

            //var zz = (from z in t group z by z.Item1 into g select new { Id = g.Key, S = g.Sum(c => c.Item2) }).ToList();

            // var xx = from w in p group p by p.
            var s = p.Select(a => a.Periods);

 
            for (int period = 1; period <= 24; period++)
            {
                //var x = p.Select(a => a.Periods.Where(p => p.Period == period)).
                // x.GroupBy(c=>c.)
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

            DateTime dt = DateTime.Parse("01-Feb-2021 18:07");

            // *************************************************************************************************
            // Act
            // *************************************************************************************************

            string s = String.Format("{0:yyyyMMdd_HHmm}", dt);

            // *************************************************************************************************
            // Assert
            // *************************************************************************************************

            s.Should().Be("20210201_1807");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DRNJ.Petro.UnitTest.Builders
{
    /// <summary>
    /// TDD Builder pattern
    ///
    /// http://www.natpryce.com/articles/000714.html
    /// </summary>
    public class FakeLoggerBuilder
    {
        public FakeLogger<T> Build<T>()
        {
            return new FakeLogger<T>();
        }
        public FakeLogger Build()
        {
            return new FakeLogger();
        }
    }
}

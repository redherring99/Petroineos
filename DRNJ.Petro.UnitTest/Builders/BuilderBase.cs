using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRNJ.Petro.UnitTest.Builders
{
    /// <summary>
    /// Builder base
    /// Builder TDD pattern
    /// See http://www.natpryce.com/articles/000714.html
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BuilderBase<T> where T : class
    {
        protected T Fake { get; set; }
        public BuilderBase()
        {
            this.Fake = Substitute.For<T>();
        }


        public virtual T Build()
        {
            return this.Fake;
        }
    }
}

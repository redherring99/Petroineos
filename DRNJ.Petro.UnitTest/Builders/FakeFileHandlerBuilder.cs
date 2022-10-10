using DRNJ.Petro.Components.IO;
using NSubstitute;
using Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRNJ.Petro.UnitTest.Builders
{
    public class FakeFileHandlerBuilder : BuilderBase<IFileHandler>
    {
        private Action<string, IList<PowerPeriod>> saveCallback;
        public FakeFileHandlerBuilder WithFileSaveCallback(Action<string,IList<PowerPeriod>> callback)
        {
            this.saveCallback = callback;
            return this;
        }

        public override IFileHandler Build()
        {
            this.Fake.When(x => x.WriteCsv(Arg.Any<string>(), Arg.Any<IList<PowerPeriod>>()))
               .Do(x => this.saveCallback(x.Arg<string>(), x.Arg<IList<PowerPeriod>>()));

            return this.Fake;
        }
    }
}

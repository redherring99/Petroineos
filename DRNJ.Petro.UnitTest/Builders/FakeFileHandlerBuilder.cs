using DRNJ.Petro.Components.IO;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRNJ.Petro.UnitTest.Builders
{
    public class FakeFileHandlerBuilder : BuilderBase<IFileHandler>
    {
        private Action<string, List<double>> saveCallback;
        public FakeFileHandlerBuilder WithFileSaveCallback(Action<string,List<double>> callback)
        {
            this.saveCallback = callback;
            return this;
        }

        public override IFileHandler Build()
        {
            this.Fake.When(x => x.WriteCsv(Arg.Any<string>(), Arg.Any<List<double>>()))
               .Do(x => this.saveCallback(x.Arg<string>(), x.Arg<List<double>>()));

            return this.Fake;
        }
    }
}

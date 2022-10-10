using DRNJ.Petro.Components.IO;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRNJ.Petro.UnitTest.Builders
{
    public class FakeStreamWrapperBuilder : BuilderBase<IStreamWrapper>
    {

        public FakeStreamWrapperBuilder WithWritelineCallBack(Action<string> a)
        {
            this.Fake.WriteLine(Arg.Do<string>(x => a(x)));
            return this;
        }
    }
}

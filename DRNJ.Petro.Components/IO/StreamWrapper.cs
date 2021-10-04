﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DRNJ.Petro.Components.IO
{
    public interface IStreamWrapper
    {
        void OpenFile(string fileName);
        void CloseFile();
        void WriteLine(string s);
    }

    public class StreamWrapper : IStreamWrapper
    {
        private StreamWriter Writer;


        public void OpenFile(string fileName)
        {
            Writer = new StreamWriter(fileName);
        }
        public void CloseFile()
        {
            if (this.Writer != null) this.Writer.Dispose();
        }

        public void WriteLine(string s)
        {
            Writer.WriteLine(s);
        }
    }
}

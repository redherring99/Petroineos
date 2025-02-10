using System.Threading.Tasks;

namespace DRNJ.Petro.Components.IO
{
    public interface IStreamWrapper
    {
        void OpenFile(string fileName);
        void CloseFile();
        void WriteLine(string s);
        Task WriteLineAsync(string s);
    }
}

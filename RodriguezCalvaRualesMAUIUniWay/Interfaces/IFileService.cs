using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RodriguezCalvaRualesMAUIUniWay.Interfaces
{
    public interface IFileService
    {
        Task<string> ReadTextAsync(string fileName);
        Task WriteTextAsync(string fileName, string content);
        Task AppendTextAsync(string fileName, string content);
        Task<bool> ExistsAsync(string fileName);
        Task DeleteAsync(string fileName);
        Task<List<string>> ReadLinesAsync(string fileName);
        Task WriteLinesAsync(string fileName, List<string> lines);
        string GetLocalFilePath(string fileName);
    }
}
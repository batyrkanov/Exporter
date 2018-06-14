using System.Collections.Generic;

namespace Exporter.Models.Interfaces
{
    public interface IExecutor
    {
        List<Dictionary<string, object>> Result { get; }
        void Execute();
    }
}

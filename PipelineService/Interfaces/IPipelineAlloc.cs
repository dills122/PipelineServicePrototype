using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineService.Interfaces
{
    public interface IPipelineAlloc<T> where T : class
    {
        Task<T> RetrievePipeline();

        Task DisposePipeline(T t);
    }
}

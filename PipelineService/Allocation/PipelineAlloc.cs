using PipelineService.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineService.Allocation
{
    public class PipelineAlloc<T> : IPipelineAlloc<T> where T : class
    {
        public Task DisposePipeline(T t)
        {
            IDisposable disposable = t as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
                return Task.CompletedTask;
            }
            else
            {
                return Task.FromException(new Exception("Instance not able to be disposed"));
            }
        }

        public Task<T> RetrievePipeline()
        {
            //_activation = Activator.CreateInstance<T>();
            return Task.FromResult(Activator.CreateInstance<T>());
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineService.Interfaces
{
    public interface IPipeline<T> where T: class
    {
        Task FillPipeline(T t);
        Task WaitForResults();
        Task<List<T>> GetResults();
        void Complete();
        Task<List<T>> ProcessWaitForResults(List<T> ts);
        void ProcessAndForget(List<T> ts);
    }

    public interface IPipeline<T, E> 
        where T : class
        where E : class
    {
        Task FillPipeline(T t);
        Task WaitForResults();
        Task<List<E>> GetResults();
        void Complete();
        Task<List<E>> ProcessWaitForResults(List<T> ts);
        void ProcessAndForget(List<T> ts);
    }
}

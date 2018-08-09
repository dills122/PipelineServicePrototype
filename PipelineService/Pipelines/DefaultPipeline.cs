using Gridsum.DataflowEx;
using PipelineService.Interfaces;
using PipelineService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace PipelineService.Pipelines
{
    public class DefaultPipeline : Dataflow<Default>, IPipeline<Default>
    {
        public override ITargetBlock<Default> InputBlock { get { return _inputBlock; } }

        private TransformBlock<Default, Default> _inputBlock;
        private ActionBlock<Default> _resultsBlock;

        private List<Default> _results { get; set; }

        public DefaultPipeline() : base(DataflowOptions.Default)
        {
            _results = new List<Default>();

            _inputBlock = new TransformBlock<Default, Default>(obj => obj);
            _resultsBlock = new ActionBlock<Default>(obj => _results.Add(obj));

            _inputBlock.LinkTo(_resultsBlock, new DataflowLinkOptions() { PropagateCompletion = true });

            RegisterChild(_inputBlock);
            RegisterChild(_resultsBlock);
        }

        public Task FillPipeline(Default input)
        {
            InputBlock.Post(input);
            return Task.CompletedTask;
        }

        public Task<List<Default>> GetResults()
        {
            return Task.FromResult(_results);
        }

        public async Task WaitForResults()
        {
            await this.CompletionTask;
        }

        public override void Complete()
        {
            InputBlock.Complete();
            base.Complete();
        }

        public async Task<List<Default>> ProcessWaitForResults(List<Default> inputs)
        {
            foreach(Default def in inputs)
            {
                await FillPipeline(def);
            }
            Complete();
            await WaitForResults();
            var results = await GetResults();
            return results;
        }

        public void ProcessAndForget(List<Default> ts)
        {
            //Fire and forget
            Task.Factory.StartNew(() => ProcessWaitForResults(ts));
        }
    }
}

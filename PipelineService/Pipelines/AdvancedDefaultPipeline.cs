using Gridsum.DataflowEx;
using PipelineService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace PipelineService.Pipelines
{
    public class AdvancedDefaultPipeline : Dataflow<string>, IPipeline<string, List<string>>
    {
        public override ITargetBlock<string> InputBlock => _inputBlock;

        private TransformBlock<string, string> _inputBlock;
        private TransformBlock<string, string> _transformBlock;
        private ActionBlock<string> _outputBlock;

        private List<List<string>> _results;

        public AdvancedDefaultPipeline() :base(DataflowOptions.Default)
        {
            _results = new List<List<string>>();

            _inputBlock = new TransformBlock<string, string>(str =>
            {
                str = str.Replace("a", string.Empty);
                str = str.Replace("ab", "tes");
                str = str.Replace("n", "trestd");
                return str;
            });

            _transformBlock = new TransformBlock<string, string>(str =>
            {
                str += "testingtwo";
                str = str.Insert(0, "anotyher");
                return str;
            });

            _outputBlock = new ActionBlock<string>(str =>
            {
                var initialSplit = str.Split('a').ToList();
                List<string> outputList = new List<string>();
                foreach(string strng in initialSplit)
                {
                    outputList.Add(strng);
                    outputList.AddRange(strng.Split('t').ToList());
                }
                _results.Add(outputList);
            });

            DataflowLinkOptions options = new DataflowLinkOptions
            {
                PropagateCompletion = true
            };

            _inputBlock.LinkTo(_transformBlock, options);
            _transformBlock.LinkTo(_outputBlock, options);

            RegisterChild(_inputBlock);
            RegisterChild(_transformBlock);
            RegisterChild(_outputBlock);
        }

        public Task FillPipeline(string t)
        {
            InputBlock.Post(t);
            return Task.CompletedTask;
        }

        public Task<List<List<string>>> GetResults()
        {
            return Task.FromResult(_results);
        }

        public void ProcessAndForget(List<string> ts)
        {
            Task.Factory.StartNew(() => ProcessWaitForResults(ts));
        }

        public async Task<List<List<string>>> ProcessWaitForResults(List<string> ts)
        {
            foreach(string str in ts)
            {
                InputBlock.Post(str);
            }
            Complete();
            await WaitForResults();
            var results = await GetResults();
            return results;
        }

        public override void Complete()
        {
            InputBlock.Complete();
            base.Complete();
        }

        public async Task WaitForResults()
        {
            await this.CompletionTask;
        }
    }
}

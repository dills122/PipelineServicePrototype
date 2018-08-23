using PipelineService.Allocation;
using PipelineService.Interfaces;
using PipelineService.Models;
using PipelineService.Pipelines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace PipelineTests
{
    public class DefaultPipelineTests
    {
        [Fact]
        public void DefaultAllocationTest()
        {
            IPipelineAlloc<DefaultPipeline> pipelineAlloc = new PipelineAlloc<DefaultPipeline>();

            Assert.NotNull(pipelineAlloc.RetrievePipeline());
        }

        [Theory]
        [InlineData("Test", 1220)]
        [InlineData("Message", 120)]
        [InlineData("Testing Message", 990)]
        [InlineData("Another Message", 40521)]
        public void TestDefaultPipeline(string message, int number)
        {
            IPipelineAlloc<DefaultPipeline> pipelineAlloc = new PipelineAlloc<DefaultPipeline>();
            IPipeline<Default> pipeline = pipelineAlloc.RetrievePipeline().Result;

            pipeline.FillPipeline(new Default(message, number));
            pipeline.Complete();
            pipeline.WaitForResults().Wait();
            var result = pipeline.GetResults().Result;
            var item = result.FirstOrDefault();
            Assert.True(result.Count == 1);
            Assert.Equal(item.message, message);
            Assert.Equal(item.randomNumber, number);
        }

        [Fact]
        public async void TestBulkDefaultPipeline()
        {
            IPipelineAlloc<DefaultPipeline> pipelineAlloc = new PipelineAlloc<DefaultPipeline>();
            IPipeline<Default> pipeline = pipelineAlloc.RetrievePipeline().Result;

            List<Default> defaults = new List<Default>();
            Random rnd = new Random();

            defaults.Add(new Default(Path.GetRandomFileName().Replace(".", string.Empty), rnd.Next(10, 2000)));
            defaults.Add(new Default(Path.GetRandomFileName().Replace(".", string.Empty), rnd.Next(10, 2000)));
            defaults.Add(new Default(Path.GetRandomFileName().Replace(".", string.Empty), rnd.Next(10, 2000)));

            var results = await pipeline.ProcessWaitForResults(defaults);
            Assert.True(results.Count == 3);
            foreach(Default def in results)
            {
                Assert.NotNull(def);
            }
        }
    }
}

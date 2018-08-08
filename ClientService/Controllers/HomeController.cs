using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ClientService.Models;
using PipelineService.Interfaces;
using PipelineService.Pipelines;
using PipelineService.Models;

namespace ClientService.Controllers
{
    public class HomeController : Controller
    {
        private IPipelineAlloc<DefaultPipeline> _pipelineAlloc;
        private IPipeline<Default> _pipeline;
        public HomeController(IPipelineAlloc<DefaultPipeline> pipelineAlloc)
        {
            _pipelineAlloc = pipelineAlloc;
            _pipeline = _pipelineAlloc.RetrievePipeline().Result;

        }
        public async Task<IActionResult> Index()
        {
            _pipeline = await _pipelineAlloc.RetrievePipeline();
            await _pipeline.FillPipeline(new Default());
            await _pipeline.FillPipeline(new Default());
            _pipeline.Complete();
            await _pipeline.WaitForResults();
            var results = await _pipeline.GetResults();
            ViewBag.Results = results;
            return View();
        }

        public async Task<IActionResult> About()
        {
            ViewData["Message"] = "Your application description page.";
            List<Default> inputs = new List<Default>();
            inputs.Add(new Default("About Pipeline", 300));
            inputs.Add(new Default("About Pipeline Two", 300));
            _pipeline = await _pipelineAlloc.RetrievePipeline();
            var results = await _pipeline.ProcessWaitForResults(inputs);
            ViewBag.Results = results;
            return View();
        }

        public async Task<IActionResult> Contact()
        {
            IPipeline<Default> localPipeline = _pipelineAlloc.RetrievePipeline().Result;
            await localPipeline.FillPipeline(new Default("Contact Pipeline", 200));
            await localPipeline.FillPipeline(new Default("Contact Pipeline Two", 200));
            localPipeline.Complete();
            await localPipeline.WaitForResults();
            var results = await localPipeline.GetResults();
            ViewBag.Results = results;

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

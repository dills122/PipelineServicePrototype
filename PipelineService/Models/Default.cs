using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineService.Models
{
    public class Default
    {
        public Default()
        {
            message = "Hello World";
            randomNumber = 122;
        }
        public Default(string message, int number)
        {
            this.message = message;
            this.randomNumber = number;
        }
        public string message { get; set; }
        public int randomNumber { get; set; }
    }
}

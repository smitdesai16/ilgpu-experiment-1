using ILGPU.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPUvsGPU.Models
{
    internal class ProcessingUnit
    {
        public string Name { get; set; }
        public AcceleratorType Type { get; set; }
        public int Index { get; set; }
    }
}

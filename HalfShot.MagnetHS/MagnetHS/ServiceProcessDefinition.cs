using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.Core
{
    class ServiceProcessDefinition
    {
        public string Name { get; private set; }
        public string Dll { get; private set; }
        public List<string> Arguments { get; private set; }
        public int Count { get; private set; }

        public ServiceProcessDefinition(string dll, string name = null, int count = 1, params string[] arguments)
        {
            Dll = dll;
            Name = name ?? dll;
            Count = count;
            Arguments = new List<string>(arguments);
        }
    }
}

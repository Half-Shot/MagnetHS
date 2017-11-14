using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.ClientServerAPIService
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class RestPath : Attribute
    {
        readonly string path;
        public string Path
        {
            get { return path; }
        }

        public RestPath(string path)
        {
            this.path = path;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace HalfShot.MagnetHS.CommonStructures.Meta
{
    public abstract class ServiceConfiguration
    {
        public static T FromYAMLFile<T>(string file) where T : ServiceConfiguration
        {
           return FromYAMLFile<T>(File.OpenText(file));
        }

        public static T FromYAMLFile<T>(StreamReader file) where T : ServiceConfiguration
        {
            return new DataTransformer.YAMLDataTransformer().FromStream<T>(file);
        }
    }
}

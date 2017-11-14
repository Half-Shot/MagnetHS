using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.DatastoreService
{
    [AttributeUsage(AttributeTargets.Method)]
    class DataStoreAttribute : Attribute
    {
        public DataStoreOperation Operation { get; set; }
        public DataStoreAttribute(DataStoreOperation operation) {
            Operation = operation;
        }
    }
}

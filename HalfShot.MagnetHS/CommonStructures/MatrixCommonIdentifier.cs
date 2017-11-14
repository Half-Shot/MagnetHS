using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures
{
    [Serializable]
    public abstract class MatrixCommonId : IEquatable<MatrixCommonId>
    {
        protected char Identifier { get; private set; }
        public string Localpart { get; private set; }
        public string Domain { get; private set; }

        public MatrixCommonId(char identifier, string commonid)
        {
            Identifier = identifier;
            if (!commonid.StartsWith(identifier))
            {
                throw new FormatException($"must start with ${identifier}");
            }
            commonid = commonid.Remove(0, 1);
            var splitPair = commonid.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (splitPair.Length < 2)
            {
                throw new FormatException("UserIds must contain a homeserver");
            }
            Localpart = splitPair[0];
            var domain = new List<string>(splitPair);
            domain.RemoveAt(0);
            Domain = String.Join(':', domain);
        }

        public override string ToString()
        {
            return $"{Identifier}{Localpart}:{Domain}";
        }

        public static bool operator ==(MatrixCommonId x, MatrixCommonId y)
        {
            return x.ToString() == y.ToString();
        }

        public static bool operator !=(MatrixCommonId x, MatrixCommonId y)
        {
            return x.ToString() != y.ToString();
        }

        public bool Equals(MatrixCommonId other)
        {
            return this.ToString() == other.ToString();
        }
    }
}

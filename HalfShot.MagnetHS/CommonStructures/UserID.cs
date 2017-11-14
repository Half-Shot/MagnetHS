using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures
{
    /* Note: See https://matrix.org/docs/spec/appendices.html#user-identifiers for acceptable UserIDs */
    [Serializable]
    public class UserID : MatrixCommonId
    {
        public UserID(string userId) : base('@', userId)
        {

        }
    }
}

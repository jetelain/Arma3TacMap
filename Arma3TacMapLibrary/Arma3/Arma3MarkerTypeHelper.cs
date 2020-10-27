using System;
using System.Collections.Generic;
using System.Linq;

namespace Arma3TacMapLibrary.Arma3
{
    public static class Arma3MarkerTypeHelper
    {
        public static IEnumerable<Arma3MarkerType> GetAll()
        {
            return Enum.GetValues(typeof(Arma3MarkerType)).Cast<Arma3MarkerType>();
        }
    }
}

using System.Collections.Generic;

namespace Eindwerk.Models
{
    public class DtoList<T> : List<T>, IDtoModel
    {
        public bool IsFilled()
        {
            return Count > 0;
        }
    }
}
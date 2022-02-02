using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dto.Base
{
    public class PagedListDto<T>
    {
        public T[] Data { get; set; }
        public int TotalElements { get; set; }
    }
}

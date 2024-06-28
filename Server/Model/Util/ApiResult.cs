using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Util
{
    public class ApiResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Result { get; set; }
        public string? Msg { get; set; }
    }
}

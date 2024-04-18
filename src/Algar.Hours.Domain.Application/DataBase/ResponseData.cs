using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase
{
    public class ResponseData<T>
    {
        public T? Data { get; set; }
        public bool Error { get; set; }
        public string Message { get; set; }
    }
}

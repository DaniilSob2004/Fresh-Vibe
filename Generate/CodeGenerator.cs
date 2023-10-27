using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreExam.Generate
{
    public static class CodeGenerator
    {
        public static string GetCode(int len = 6)
        {
            return Guid.NewGuid().ToString()[..len].ToUpper();
        }
    }
}

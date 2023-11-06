using System;

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

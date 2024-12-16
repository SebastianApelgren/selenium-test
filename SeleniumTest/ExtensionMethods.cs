using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTest
{
    public static class ExtensionMethods
    {
        public static int TokenCounter(this string text)
        { 
            int tokenCounter = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (i%4 == 0)
                {
                    tokenCounter++;
                }
            }

            return tokenCounter;
        }
    }
}

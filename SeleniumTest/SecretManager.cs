using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumTest
{
    public class SecretManager
    {
        //This class is used to retrieve the OpenAI API key from the secret file

        public static string GetOpenAiApiKey()
        {
            string secret = ReadSecretFile("OpenAiApiKey.txt").Result;
            return secret;
        }

        private static async Task<string> ReadSecretFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                File.Create(fileName);
                throw new FileNotFoundException($"{fileName} not found");
            }
            string secret = await File.ReadAllTextAsync(fileName);
            return secret;
        }
    }
}

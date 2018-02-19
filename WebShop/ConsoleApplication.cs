using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebShop
{
    public class ConsoleApplication
    {
        public static void WriteDefaultValues(string fileName, byte[] data)
        {
            if (File.Exists(fileName)) {
                return;
            }
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                writer.Write(data);
            }
        }

        
    }
}

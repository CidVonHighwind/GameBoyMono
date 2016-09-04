using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entschlüssler
{
    class Program
    {
        static void Main(string[] args)
        {

            byte[] chi = File.ReadAllBytes("C:/input");

            byte[] outPut = new byte[chi.Length / 2];


            for (int i = 0; i < chi.Length / 2; i++)
            {
                outPut[i] = Math.Min(chi[i * 2], chi[i * 2 + 1]);
            }

            File.WriteAllBytes("C:/Users/Patri/Desktop/Neuer Ordner/Level 1/output.png", outPut);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Buffers;
using System.Buffers.Text;
using System.Text;
using System.Text.Encodings;
using System.IO;
using Microsoft.AspNetCore.Cryptography;

namespace FibonatixQueue.Services
{
    public class SymAlgo
    {
        private byte[] Input { get; set; }

        private ICryptoTransform Encryptor { get; set; }

        private ICryptoTransform Decryptor { get; set; }

        public SymAlgo(string algName, string key = null, string iv = null)
        {
            SymmetricAlgorithm algorithm = SymmetricAlgorithm.Create(algName);
            algorithm.Padding = PaddingMode.Zeros;

            if (key == null)
                algorithm.GenerateKey();
            else
                algorithm.Key = Encoding.UTF8.GetBytes(key);
            if (iv == null)
                algorithm.GenerateIV();
            else
                algorithm.IV = Encoding.UTF8.GetBytes(iv);

            Encryptor = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV);
            Decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV);
        }

        public byte[] Encrypt(string newInput)
        {
            byte[] newBInput = Encoding.UTF8.GetBytes(newInput);
            Input = Encryptor.TransformFinalBlock(newBInput, 0, newBInput.Length);

            return Input;
        }

        public string Decrypt(string newInput)
        {
            byte[] newBInput = Convert.FromBase64String(newInput);
            Input = Decryptor.TransformFinalBlock(newBInput, 0, newBInput.Length);

            return Encoding.UTF8.GetString(Input);
        }
    }
}

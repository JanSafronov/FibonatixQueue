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
        private byte[] input { get; set; }

        private ICryptoTransform encrypt { get; set; }

        private ICryptoTransform decrypt { get; set; }

        public SymAlgo(string algName, string key = null, string iv = null)
        {
            SymmetricAlgorithm algorithm = SymmetricAlgorithm.Create(algName);
            if (key == null)
                algorithm.GenerateKey();
            else
                algorithm.Key = Encoding.UTF8.GetBytes(key);
            if (iv == null)
                algorithm.GenerateIV();
            else
                algorithm.IV = Encoding.UTF8.GetBytes(iv);

            this.encrypt = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV);
            this.decrypt = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV);

            this.input = Encoding.UTF8.GetBytes("");
        }

        public string Encrypt(string newInput)
        {
            input = Encoding.UTF8.GetBytes(newInput);
            //input = encrypt.TransformFinalBlock(input, 0, input.Length);
            MemoryStream stream = new MemoryStream();

            var cryp = new CryptoStream(stream, encrypt, CryptoStreamMode.Write);
            cryp.Write(input, 0, input.Length);

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        public string Decrypt(string newInput)
        {
            input = Encoding.UTF8.GetBytes(newInput);
            //input = decrypt.TransformFinalBlock(input, 0, input.Length);

            MemoryStream stream = new MemoryStream();

            var cryp = new CryptoStream(stream, decrypt, CryptoStreamMode.Write);
            cryp.Write(input, 0, input.Length);

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}

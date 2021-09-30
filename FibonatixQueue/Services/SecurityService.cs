using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Cryptography;

namespace FibonatixQueue.Services
{
    public class SymAlgo<E> where E : SymmetricAlgorithm
    {
        private SymmetricAlgorithm algo = SymmetricAlgorithm.Create(nameof(Aes));

        private string input { get; }

        private byte[] key { get; }

        private byte[] iv { get; }

        public SymAlgo(string input, string key, string iv)
        {
            this.input = input;
            algo.GenerateKey();
            algo.GenerateIV();
            this.key = algo.Key;
            this.iv = algo.IV;
        }

        public void Encrypt()
        {
        }
    }
}

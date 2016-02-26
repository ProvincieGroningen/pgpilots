using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ExpressionToCodeLib;
using NUnit.Framework;

namespace WindMeter
{
    public static class Decryptor
    {
        // Thanks to https://github.com/willem4ever/node-red-ttn/blob/master/90-TTN.js
        // and http://forum.thethingsnetwork.org/t/parse-data/789/7
        // 
        public class DecryptedLoraMessage
        {
            public string Node;
            public string Payload;
        }

        [Test]
        public static void TestDecription()
        {
            const string dataRaw = "gAF0AQIAAAABTf5C11bgiNx9ZPm5klWCxl4EzEpDG6udqcozu6zWy9w="; // the data encrypted
            var key = new byte[]
            {0x2B, 0x7e, 0x15, 0x16, 0x28, 0xae, 0xd2, 0xa6, 0xab, 0xf7, 0x15, 0x88, 0x09, 0xcf, 0x4f, 0x3b};
            var iv = Enumerable.Repeat((byte) 0, 16).ToArray();
            var dataBytes = Convert.FromBase64String(dataRaw);

            var decryptedLoraMessage = Decrypt(dataBytes, key, iv);
            PAssert.That(() => decryptedLoraMessage.Node == "2017401");
            PAssert.That(() => decryptedLoraMessage.Payload == "{ \"I\": 1, \"S\": 7.5, \"R\": 6 }");
        }

        [Test]
        public static void TestSpeed()
        {
            const string dataRaw = "gAF0AQIAAAABTf5C11bgiNx9ZPm5klWCxl4EzEpDG6udqcozu6zWy9w="; // the data encrypted
            var key = new byte[]
            {0x2B, 0x7e, 0x15, 0x16, 0x28, 0xae, 0xd2, 0xa6, 0xab, 0xf7, 0x15, 0x88, 0x09, 0xcf, 0x4f, 0x3b};
            var iv = Enumerable.Repeat((byte) 0, 16).ToArray();
            var dataBytes = Convert.FromBase64String(dataRaw);
            var t = new Stopwatch();
            t.Start();
            for (var i = 0; i < 5000; i++)
            {
                Decrypt(dataBytes, key, iv);
            }
            t.Stop();
            PAssert.That(() => t.ElapsedMilliseconds < 5000);
        }

        public static DecryptedLoraMessage Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            // int8 -> sbyte
            // uint8 -> byte
            // word -> ushort
            using (var aes = new AesManaged
            {
                Mode = CipherMode.ECB,
                Padding = PaddingMode.None
            })
            {
                var encryptor = aes.CreateEncryptor(key, iv);

                var dataLengte = data.Length - 13;
                var blockA = new byte[16];
                var work = new byte[dataLengte];

                for (var i = 0; i < dataLengte; i++)
                {
                    work[i] = data[i + 9];
                }

                for (var i = 0; i < dataLengte; i += 16)
                {
                    blockA[0] = 0x01;
                    blockA[1] = 0x00;
                    blockA[2] = 0x00;
                    blockA[3] = 0x00;
                    blockA[4] = 0x00;
                    blockA[5] = 0; // 0 for uplink frames 1 for downlink frames;
                    blockA[6] = data[1]; // LSB devAddr 4 bytes
                    blockA[7] = data[2]; // ..
                    blockA[8] = data[3]; // ..
                    blockA[9] = data[4]; // MSB
                    blockA[10] = data[6]; // LSB framecounter
                    blockA[11] = data[7]; // MSB framecounter
                    blockA[12] = 0x00; // Frame counter upper Bytes}
                    blockA[13] = 0x00;
                    blockA[14] = 0x00;
                    blockA[15] = Convert.ToByte((i >> 4) + 1); // block sequence counter 1..
                    var ox = Encrypt(encryptor, blockA);
                    var xor = ox.Select(o => (ushort) o).ToArray();
                    var k = Math.Min(dataLengte - i, 16);
                    for (var j = 0; j < k; j++)
                    {
                        work[i + j] = (byte) (work[i + j] ^ xor[j]);
                    }
                }
                var node = data[1] + (data[2] << 8) + (data[3] << 16) + (data[4] << 24);
                var decryptedLoraMessage = new DecryptedLoraMessage
                {
                    Node = node.ToString("X"),
                    Payload = Encoding.ASCII.GetString(work)
                };
                return decryptedLoraMessage;
            }
        }

        private static IEnumerable<byte> Encrypt(ICryptoTransform decryptor, byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                }
                return ms.ToArray();
            }
        }
    }
}
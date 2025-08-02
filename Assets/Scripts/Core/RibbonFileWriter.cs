using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class RibbonFileWriter
{


    public static readonly byte[] Key = Encoding.UTF8.GetBytes("whatifitoldyouthattheworldwaspoo");
    public static readonly byte[] IV = Encoding.UTF8.GetBytes("gmtk2025ssfaapoo");
    public static bool EncryptionOn = true;
    public static string ReadFile(string path)
    {
        string dirPath = Path.GetDirectoryName(path);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        if (File.Exists(path))
        {
            string file = File.ReadAllText(path);
            file = AutoDecrypt(file, Key, IV);
            return file;
        }
        return "";
    }



    public static void WriteFile(string path, string content)
    {
        string dirPath = Path.GetDirectoryName(path);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        if (EncryptionOn)
        {
            content = EncryptStringAES(content, Key, IV);
        }

        File.WriteAllText(path, content);

    }
    public static string EncryptStringAES(string plainText, byte[] key, byte[] iv)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            using (var ms = new MemoryStream())
            using (var cryptoStream = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cryptoStream))
            {
                sw.Write(plainText);
                sw.Flush();
                cryptoStream.FlushFinalBlock();
                byte[] encryptedBytes = ms.ToArray();
                string base64 = Convert.ToBase64String(encryptedBytes);
                return "ENC:" + base64;
            }
        }
    }
    public static string DecryptStringAES(string encryptedBase64, byte[] key, byte[] iv)
    {
        try
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (var ms = new MemoryStream(encryptedBytes))
                using (var cryptoStream = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (var sr = new StreamReader(cryptoStream))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        catch
        {
            return encryptedBase64;
        }
    }
    public static string AutoDecrypt(string input, byte[] key, byte[] iv)
    {
        if (input.StartsWith("ENC:"))
        {
            string base64 = input.Substring(4);
            return DecryptStringAES(base64, key, iv);
        }
        else
        {
            return input; // Plain text
        }
    }


    public static void WriteEncryptedFile(string path, byte[] aesKey, byte[] plainData)
    {
        byte[] magic = new byte[] { 0x45, 0x4E, 0x43 }; // "ENC"
        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            fs.Write(magic, 0, magic.Length);

            using (var aes = Aes.Create())
            {
                aes.Key = aesKey;
                aes.GenerateIV();
                fs.Write(aes.IV, 0, aes.IV.Length);

                using (var encryptor = aes.CreateEncryptor())
                using (var cryptoStream = new CryptoStream(fs, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainData, 0, plainData.Length);
                }
            }
        }
    }

}

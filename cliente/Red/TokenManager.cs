using Godot;
using Client.Juego;
using Shared.Paquetes;
using System;
using System.IO;
using System.Security.Cryptography;
namespace Client.Red
{
    public class TokenManager
    {
        private const string RutaArchivoSesion = "user://ProfileData.enc";
        private static readonly byte[] Clave = Convert.FromHexString(
            "B8E2F1A94C6D7F1035A8CE924D71BF0E9C4A5D88F1E2B3764AC91D5E7F03B6A2"[..64]
        );
        public static void GuardarToken(string token)
        {
            string ruta = ProjectSettings.GlobalizePath(RutaArchivoSesion);

            using Aes aes = Aes.Create();
            aes.Key = SHA256.HashData(Clave); // normaliza a 32 bytes
            aes.GenerateIV();

            using FileStream fs = new(ruta, FileMode.Create, System.IO.FileAccess.Write);
            fs.Write(aes.IV, 0, aes.IV.Length); // guarda el IV al inicio

            using CryptoStream cs = new(fs, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using StreamWriter sw = new(cs);
            sw.Write(token);
        }

        public static string LeerTokenGuardado()
        {
            string ruta = ProjectSettings.GlobalizePath(RutaArchivoSesion);
            if (!System.IO.File.Exists(ruta))
            {
                GD.Print("No hay archivo de sesión guardado.");
                return null;
            }

            try
            {

                using FileStream fs = new(ruta, FileMode.Open, System.IO.FileAccess.Read);

                byte[] iv = new byte[16]; // tamaño de IV para AES
                fs.ReadExactly(iv, 0, iv.Length);

                using Aes aes = Aes.Create();
                aes.Key = SHA256.HashData(Clave);
                aes.IV = iv;

                using CryptoStream cs = new(fs, aes.CreateDecryptor(), CryptoStreamMode.Read);
                using StreamReader sr = new(cs);
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                GD.PrintErr($"No se pudo leer el archivo de sesión: {ex.Message}");
                return null;
            }

        }

    }
}
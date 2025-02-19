using System.Security.Cryptography;
using System.IO;
using Microsoft.VisualBasic;
using System.Transactions;
using System.Text.Json;
using System.Runtime.InteropServices;
class cSharpUtils 
{
    static byte[] encrypted;
    static string decrypted;
    static byte[] masterKey;
    static bool programTerminateFlag = false;
    static string inputIdentificador;
    static string inputSenha;
    static List<passwordTemplate> listOfPasswords = new List<passwordTemplate>();
    static List<byte[]> listofEncryptedPasswords = new List<byte[]>();


    static void Main()
    {
        if (File.Exists("firstTime0.txt"))
        {
            using (Aes myAes = Aes.Create())
            {
            
                (byte[] key, byte[] IV) = masterKeyManager.GetMasterKey();
                if (key != null && IV != null)
                {
                    myAes.Key = key;
                    myAes.IV = IV;

                    ICryptoTransform encrypter = myAes.CreateEncryptor(key, IV);
                    ICryptoTransform decrypter = myAes.CreateDecryptor(key, IV);




                    System.Console.WriteLine("Acesso permitido. \n");
                    while (!programTerminateFlag)
                    {
                        System.Console.WriteLine("Selecione uma ação: \n");
                        System.Console.WriteLine("0. Sair");
                        System.Console.WriteLine("1. Adicionar nova senha");
                        System.Console.WriteLine("2. Mostrar senhas");
                        Int32.TryParse(Console.ReadLine(), out int commandIndex);

                        switch (commandIndex)
                        {
                            case 0:
                                programTerminateFlag = true;
                                break;
                            case 1:
                                System.Console.WriteLine("Insira o identificador: ");
                                inputIdentificador = Console.ReadLine();

                                System.Console.WriteLine("Insira a senha: ");
                                inputSenha = Console.ReadLine();

                                passwordTemplate newPassword = new passwordTemplate();

                                newPassword.Identificador = inputIdentificador;
                                newPassword.Senha = inputSenha;

                                listOfPasswords.Add(newPassword);

                                
                                EncryptToJson(encrypter, listOfPasswords);
                                break;
                            case 2:
                                System.Console.WriteLine("Descriptografando...");
                                GetPasswords(decrypter);
                                System.Console.WriteLine("Sucesso!");
                                break;
                        }
                    }
                }
                else
                {
                    System.Console.WriteLine("Acesso negado.");
                }
            }
        
        }
        else
        {
            masterKeyManager.FirstBootUp();
        }
    }

    static byte[] EncryptToJson(ICryptoTransform encrypter, List<passwordTemplate> input)
    {
        try
        {
            string newPasswordString = JsonSerializer.Serialize(input.Last());
            byte[] encrypted;
            encrypted = Encrypt(encrypter, newPasswordString, "senhas", "json", input.IndexOf(input.Last()));
            listofEncryptedPasswords.Add(encrypted);
            return encrypted;      
        }
        catch (Exception e) 
        {
            System.Console.WriteLine("Chave mestre incorreta: " + e);
            return null;
        }
        
    }

    static public byte[] Encrypt(ICryptoTransform encrypter, string input, string fileName, string fileExtension, int i)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            using (CryptoStream cryptoStream = new CryptoStream(stream, encrypter, CryptoStreamMode.Write))
            {
                using (StreamWriter writer = new StreamWriter(cryptoStream))
                {
                    writer.Write(input);
                    writer.Flush();
                }
            }
            encrypted = stream.ToArray();

            using (FileStream masterKeyWriter = new FileStream($"{fileName}{i}.{fileExtension}", FileMode.Create, FileAccess.Write))
            {
                    masterKeyWriter.Write(encrypted, 0, encrypted.Length);
            }
            }
            
            return encrypted;
    }

    static passwordTemplate DecryptFromJson(ICryptoTransform decrypter, string filePath)
    {
        try 
        {
            string decrypted = Decrypt(decrypter, filePath);
            passwordTemplate decryptedPassword = JsonSerializer.Deserialize<passwordTemplate>(decrypted);
            return decryptedPassword;
        }
        catch (Exception e)
        {
            System.Console.WriteLine("Acesso negado: " + e);
            return null;
        }
        
    }

    static string Decrypt(ICryptoTransform decrypter, string filePath)
    {
        byte[] input = File.ReadAllBytes(filePath);
        using (MemoryStream stream = new MemoryStream(input))
        {
            using (CryptoStream cryptoStream = new CryptoStream(stream, decrypter, CryptoStreamMode.Read))
            {
                using (StreamReader reader = new StreamReader(cryptoStream))
                {
                    decrypted = reader.ReadToEnd();
                }
            }
        }

        return decrypted;
    }

    



    static void GetPasswords(ICryptoTransform decrypter)
    {
        for (int i = 0; i < listofEncryptedPasswords.Count; i++) 
        {
            passwordTemplate DecPassword = new passwordTemplate();
            DecPassword = DecryptFromJson(decrypter, $"senhas{i}.json");
            // Isso é uma merda, depois arruma isso.
            System.Console.WriteLine(DecPassword.Identificador);
            System.Console.WriteLine(DecPassword.Senha);

            System.Console.WriteLine("Indentificador: " + DecPassword.Identificador);
            System.Console.WriteLine("Senha: " + DecPassword.Senha);
        }
        
    }

}
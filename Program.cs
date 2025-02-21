using System.Security.Cryptography;
using System.IO;
using Microsoft.VisualBasic;
using System.Transactions;
using System.Text.Json;
using System.Runtime.InteropServices;
using SQLiteAPI;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
static class cSharpUtils 
{
    static string encryptedIdentifier;
    static string encryptedPassword;
    static string decrypted;
    static byte[] masterKey;
    static bool programTerminateFlag = false;
    static string inputIdentificador;
    static string inputSenha;
    static List<passwordTemplate> listOfPasswords = new List<passwordTemplate>();
    static string decryptedIdentifier;
    static string decryptedPassword;




    static void Main()
    {
        SQLiteAPI.Interface.DatabaseCommand("create", "passwords");
        SQLiteAPI.Interface.DatabaseCommand("select", "passwords");
        SQLiteAPI.Interface.TableCommand("create", "encryptedPasswords", "Id INTEGER PRIMARY KEY AUTOINCREMENT, encIdentifier TEXT NOT NULL, encPassword, TEXT NOT NULL");


        if (File.Exists("firstTime.txt"))
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

                                passwordTemplate newPassword = new passwordTemplate(inputIdentificador, inputSenha);

                                Encrypt(encrypter, newPassword);
                                break;
                            case 2:
                                System.Console.WriteLine("Descriptografando...");
                                DecryptAll(decrypter);
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


    static public void Encrypt(ICryptoTransform encrypter, passwordTemplate input)
    {
        try 
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(stream, encrypter, CryptoStreamMode.Write))
                {
                    using (StreamWriter writer = new StreamWriter(cryptoStream))
                    {
                        writer.Write(input.Identificador);
                        writer.Flush();
                    }
                }
                encryptedIdentifier = stream.ToString();
            }

            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(stream, encrypter, CryptoStreamMode.Write))
                {
                    using (StreamWriter writer = new StreamWriter(cryptoStream))
                    {
                        writer.Write(input.Senha);
                        writer.Flush();
                    }
                }
                encryptedPassword = stream.ToString();
            }
        }
        catch(Exception e)
        {
            System.Console.WriteLine("Erro: " + e);
            return;
        }
    }

    public static void WriteEncryptedPasswordToDatabase (ICryptoTransform encrypter, passwordTemplate input)
    {
        Encrypt(encrypter, input);
        SQLiteAPI.Interface.TableCommand("addvalue", "encryptedPasswords", "encIdentifier, encPassword", $"{input.Identificador}, {input.Senha}");

    }


    static string DecryptAll(ICryptoTransform decrypter)
    {
        for (int i = 1; i <= 1; i++)
        {
            SQLiteAPI.Interface.TableCommand("getvalue", "encryptedPassword", "encIdentifier", i.ToString());
            byte[] inputIdentifier = Encoding.ASCII.GetBytes(SQLiteAPI.Interface.GetValueResult);
            SQLiteAPI.Interface.TableCommand("getvalue", "encryptedPassword", "encPassword", i.ToString());
            byte[] inputPassword = Encoding.ASCII.GetBytes(SQLiteAPI.Interface.GetValueResult);

            using (MemoryStream stream = new MemoryStream(inputIdentifier))
            {
                using (CryptoStream cryptoStream = new CryptoStream(stream, decrypter, CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(cryptoStream))
                    {
                        decryptedIdentifier = reader.ReadToEnd();
                    }
                }
            }

            using (MemoryStream stream = new MemoryStream(inputPassword))
            {
                using (CryptoStream cryptoStream = new CryptoStream(stream, decrypter, CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(cryptoStream))
                    {
                        decryptedPassword = reader.ReadToEnd();
                    }
                }
            }

            SQLiteAPI.Interface.TableCommand("create", "passwords", "Id INTEGER PRIMARY KEY AUTOINCREMENT, Identifier TEXT NOT NULL, Password TEXT NOT NULL");
            SQLiteAPI.Interface.TableCommand("addvalue", "passwords", "Identifier, Password", $"{decryptedIdentifier}, {decryptedPassword}");
        }
        SQLiteAPI.Interface.TableCommand("view", "passwords");
        System.Console.WriteLine("Press any key to return...");
        Console.ReadKey();
        SQLiteAPI.Interface.TableCommand("destroy", "passwords");
        
        
        return decrypted;
    }



}

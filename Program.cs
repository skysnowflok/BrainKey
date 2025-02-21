using System.Security.Cryptography;
using System.IO;
using Microsoft.VisualBasic;
using System.Transactions;
using System.Text.Json;
using System.Runtime.InteropServices;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
static class cSharpUtils 
{
    static string encryptedIdentifier ="";
    static string encryptedPassword ="";
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
        SQLITEAPI.Interface.DatabaseCommand("create", "passwords");
        SQLITEAPI.Interface.DatabaseCommand("select", "passwords");
        SQLITEAPI.Interface.TableCommand("create", "encryptedPasswords", "Id INTEGER PRIMARY KEY AUTOINCREMENT, encIdentifier TEXT NOT NULL, encPassword TEXT NOT NULL");

        if (File.Exists("firstTime.txt"))
        {
            using (Aes myAes = Aes.Create())
            {
            
                (byte[] key, byte[] IV) = masterKeyManager.GetMasterKey();
                if (key != null && IV != null)
                {
                    myAes.Key = key;
                    myAes.IV = IV;
                    myAes.Padding = PaddingMode.PKCS7;

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

                                WriteToDatabase(encrypter, newPassword);
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


    static public passwordTemplate Encrypt(ICryptoTransform encrypter, passwordTemplate input)
    {
        try 
        {
            byte[] encryptedIdentifierByte;
            byte[] encryptedPasswordByte;

            System.Console.WriteLine(input.Identificador);
            System.Console.WriteLine(input.Senha);

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
                encryptedIdentifierByte = stream.ToArray();
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
                encryptedPasswordByte = stream.ToArray();
            }

            encryptedIdentifier = Convert.ToBase64String(encryptedIdentifierByte);
            encryptedPassword = Convert.ToBase64String(encryptedPasswordByte);


            passwordTemplate encryptedPasswordTemplate = new passwordTemplate(encryptedIdentifier, encryptedPassword);
            return encryptedPasswordTemplate;


        }
        catch(Exception e)
        {
            System.Console.WriteLine("Erro: " + e);
            return null;
        }
    }

    public static void WriteToDatabase (ICryptoTransform encrypter, passwordTemplate input)
    {
        passwordTemplate passwordInfo = Encrypt(encrypter, input);
        SQLITEAPI.Interface.TableCommand("addvalue", "encryptedPasswords", "encIdentifier, encPassword", $"{passwordInfo.Identificador}, {passwordInfo.Senha}");

    }


    static string DecryptAll(ICryptoTransform decrypter)
    {
        for (int i = 1; i <= 1; i++)
        {
            SQLITEAPI.Interface.TableCommand("getvalue", "encryptedPasswords", "encIdentifier", i.ToString());
            byte[] inputIdentifier = Convert.FromBase64String(SQLITEAPI.Interface.GetValueResult);
            SQLITEAPI.Interface.TableCommand("getvalue", "encryptedPasswords", "encPassword", i.ToString());
            byte[] inputPassword = Convert.FromBase64String(SQLITEAPI.Interface.GetValueResult);

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

            SQLITEAPI.Interface.TableCommand("create", "passwords", "Id INTEGER PRIMARY KEY AUTOINCREMENT, Identifier TEXT NOT NULL, Password TEXT NOT NULL");
            SQLITEAPI.Interface.TableCommand("addvalue", "passwords", "Identifier, Password", $"'{decryptedIdentifier}', '{decryptedPassword}'");
        }
        SQLITEAPI.Interface.TableCommand("view", "passwords");
        System.Console.WriteLine("Press any key to return...");
        Console.ReadKey();
        SQLITEAPI.Interface.TableCommand("destroy", "passwords");
        
        
        return decrypted;
    }



}

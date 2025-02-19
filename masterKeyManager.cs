using System.Security.Cryptography;
using System.IO;
using System;
using System.Diagnostics;

static public class masterKeyManager 
{
    static private char drive = 'D';
    static private byte[] key;
    static private byte[] IV;
    static private ICryptoTransform encrypter;
    static public void FirstBootUp()
    {
        Console.WriteLine("Instruções de uso: \n");;

        Console.WriteLine("1. O programa irá gerar dois arquivos: key.bin e IV.bin");
        Console.WriteLine("2. Mova esses dois arquivos para um pendrive qualquer.");
        Console.WriteLine("3. Note que o programa irá lhe perguntar se o usuario quer setar uma sigla de drive.");
        Console.WriteLine("4. Isso é para casos quando o seu pendrive é E:\, F:\, etc.");
        Console.WriteLine("5. Estas instruções e as duas proximas perguntas irão aparecer apenas uma vez.");
        Console.WriteLine("6. Execute o programa novamente para usar-lo");
        
        Console.WriteLine("Pressione qualquer tecla para continuar...");
        ConsoleKeyInfo cki = Console.ReadKey();


        System.Console.WriteLine("Gostaria de especificar uma sigla de um drive? (Y/N)");
        string responseDriveYN = Console.ReadLine();
        responseDriveYN = responseDriveYN.ToUpper();

        if (responseDriveYN == "Y") 
        {
            drive = Console.ReadLine().First();
        }
        else
        {
            drive = 'D';
        }


        System.Console.WriteLine("Gostaria de gerar uma chave mestre? (Y/N)");
        string responseMasterKey = Console.ReadLine();
        responseMasterKey = responseMasterKey.ToUpper();
        if (responseMasterKey == "Y")
        {
            using (Aes newAes = Aes.Create())
            {
                key = newAes.Key;
                IV = newAes.IV;

                WriteMasterKey(key, IV);

                encrypter = newAes.CreateEncryptor(key, IV);
            }

            string FirstBootUp = ".";
            cSharpUtils.Encrypt(encrypter, FirstBootUp, "firstTime", "txt", 0);
        }
        else 
        {
            Console.Write("Entendido, desligando...");
            Thread.Sleep(1200);
        }
    }
    static public void WriteMasterKey(byte[] key, byte[] IV)
    {
        using (FileStream masterKeyWriter = new FileStream("key.bin", FileMode.Create, FileAccess.Write))
        {
            masterKeyWriter.Write(key, 0, key.Length);
        }
        
        using (FileStream masterIVWriter = new FileStream("IV.bin", FileMode.Create, FileAccess.Write))
        {
            masterIVWriter.Write(IV, 0, IV.Length);
        }
    }

    static public (byte[], byte[]) GetMasterKey()
    {
        if (File.Exists(@$"{drive}:\key.bin") && File.Exists(@$"{drive}:\IV.bin"))
        {
            System.Console.WriteLine("executing this");
            return (File.ReadAllBytes(@$"{drive}:\key.bin"), File.ReadAllBytes(@$"{drive}:\IV.bin"));
        }
        else
        {
            return (File.ReadAllBytes("key.bin"), File.ReadAllBytes("IV.bin"));
        }
    }
}

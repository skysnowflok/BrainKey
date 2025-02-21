public class passwordTemplate 
{
    public string Identificador {get; set;}
    public string Senha {get; set;}

    public passwordTemplate(string identificador, string senha)
    {
        Identificador = identificador;
        Senha = identificador;
    }
}


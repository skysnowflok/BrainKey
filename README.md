<p align="center">
  <img src="resources/brainKeyLogo.png" style="width:300px;height:300px" alt="Brainkey Logo">
</p>
<h1 align="center">Brainkey</h1>
<p align="center">Um gerenciador de senhas simples que utiliza AES e interface de CLI.</p>

## Sobre

Brainkey é um programa de CLI que é utilizado para guardar senhas em JSONs criptografados utilizando AES (Advanced Encryption System). O AES utiliza dois arquivos como senha que são gerados automaticamente ao executar o programa pela primeira vez. Estes arquivos, por sua vez, podem ser guardados em um pendrive ou no diretório local.

## Como usar

No cmd, execute:

``` 
dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true
```

> [!NOTE]
> O cmd tem que estar executando no diretorio do codigo fonte.

Isso irá gerar um arquivo .exe. Retire-o da pasta e o coloque em qualquer lugar que deseja.

Agora, execute o programa. Na primeira vez usando o programa automaticamente irá gerar uma lista de instruções como tal:

``` 
Instruções de uso: 

1. O programa irá gerar dois arquivos: key.bin e IV.bin
2. Mova esses dois arquivos para um pendrive qualquer.
3. Note que o programa irá lhe perguntar se o usuario quer setar uma sigla de drive.
4. Isso é para casos quando o seu pendrive é E:\, F:\, etc."
5. Estas instruções e as duas proximas perguntas irão aparecer apenas uma vez."
6. Execute o programa novamente para usar-lo"
        
Pressione qualquer tecla para continuar...
```

Seguindo as instruções e executando mais uma vez. Uma lista de ações aparecera *apenas* quando as duas chaves estejam ou no diretorio local ou em um Pendrive com a sigla configurada.

## Detalhes Técnicos

Abaixo está um diagrama baseado em PlantUML que explica o sistema:

<p align="center">
  <img src="resources/Password Manager - Data Flow Diagram.svg" alt="Diagrama de Fluxo de Dados">
</p>

## Planos futuros

- Criar um interface de usuario com WPF ou Blazor
- Integrar SQLite para o armazenamento de senhas criptografadas.
- Filtro de senhas
- Gerador de senhas imbutido
- Analisador da segurança de uma nova senha
- Integração com navegador
- Suporte para API

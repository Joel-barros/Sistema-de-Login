using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using SistemaLogin.Model;

namespace SistemaLogin
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var conexao = new Conexao();
            //conexao.conn!.Open();

            Console.WriteLine("-Pressione Enter-");
            Console.ReadKey();

            Console.Clear();

            while (true)
            {
                Console.WriteLine("---- FACEBOOK ----");

                try
                {
                    await conexao.conn!.OpenAsync();

                    Console.Write("LOGIN: ");
                    string login = Console.ReadLine()!;
                    Console.Write("SENHA: ");
                    string senha = Console.ReadLine()!;

                    bool login_senha = await conexao.VerificarLogin(login, senha);

                    if (login_senha)
                    {
                        DateTime data = DateTime.Now;
                        string sql = $"update usuario set ultimologin = '{data.ToString("yyyy/MM/dd HH:mm:ss")}' where login = '{login}' and senha = '{senha}'";
                        MySqlCommand comando = new MySqlCommand(sql, conexao.conn);
                        await comando.ExecuteNonQueryAsync();
                        break;
                    }
                    else
                    {
                        await conexao.conn!.CloseAsync();
                        Console.Write("Pressione Enter para continuar...");
                        Console.ReadKey();
                        Console.Clear();
                        continue;
                    }
                }
                catch (MySqlException sqlex)
                {
                    Console.WriteLine(sqlex.Message.ToString());
                    Console.ReadKey();
                    continue;
                }
            }
            await conexao.conn!.CloseAsync();
            while (true)
            {
                Console.Clear();
                await conexao.conn!.OpenAsync();

                string usuario = "select login from usuario order by ultimologin desc";
                MySqlCommand comando = new MySqlCommand(usuario, conexao.conn);
                MySqlDataReader reader = await comando.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    Console.WriteLine($"-------- SEJA BEM VINDO(A) {reader.GetString(0).ToUpper()} ----------");
                    Console.WriteLine("- CENTRAL DE GERENCIAMENTO DE CONTAS -");
                    await reader.CloseAsync();
                }

                Console.WriteLine("CADASTRAR REGISTRO -> 1 \nATUALIZAR REGISTRO -> 2 \nPROCURAR REGISTRO --> 3");
                string acao = Console.ReadLine()!;

                if (acao == "1")
                {
                    Console.Clear();
                    Console.WriteLine("- CADASTRO -");

                    Console.Write("DIGITE O NOME DO LOGIN: ");
                    string login = Console.ReadLine()!;
                    Console.Write("CRIE UMA SENHA (NO MÁXIMO 8 CARACTERES): ");
                    string senha = Console.ReadLine()!;

                    DateTime data = new DateTime();

                    string sql = $"insert into usuario value (default, '{login}', '{senha}', '{data.ToString("yyyy/MM/dd HH:mm:ss")}')";

                    MySqlDataReader resultadoDoComando = await conexao.Comando(sql);

                    if (resultadoDoComando == null)
                    {
                        Console.WriteLine("Por favor, tente novamente...");
                        Console.ReadKey();
                        await conexao.conn!.CloseAsync();
                        continue;
                    }
                    else
                    {
                        System.Console.WriteLine("Novo Login cadastrado.");
                        System.Console.WriteLine("Pressione Enter para continuar...");
                        Console.ReadKey();
                        await conexao.conn!.CloseAsync();
                        continue;
                    }
                }
                if (acao == "2")
                {
                    Console.Clear();
                    Console.WriteLine("- ATUALIZE UM REGISTRO -");

                    Console.Write("DIGITE O LOGIN ATUAL:");
                    string login = Console.ReadLine()!;

                    Console.Write("NOVO LOGIN:");
                    string loginNovo = Console.ReadLine()!;

                    Console.Write("DIGITE A SENHA ATUAL:");
                    string senha = Console.ReadLine()!;

                    Console.Write("NOVA SENHA:");
                    string senhaNova = Console.ReadLine()!;

                    string sql = $"update usuario set login = '{loginNovo}', senha = '{senhaNova}' where login = '{login}'";

                    if (!await conexao.VerificarLogin(login, senha))
                    {
                        Console.ReadKey();
                        await conexao.conn.CloseAsync();
                        continue;
                    }

                    MySqlDataReader resultadoDoComando = await conexao.Comando(sql);

                    if (resultadoDoComando == null) { Console.ReadKey(); 
                    await conexao.conn!.CloseAsync(); continue; }
                    
                    else
                    {
                        System.Console.WriteLine("Registro atualizado!");
                        Console.ReadKey();
                        await conexao.conn!.CloseAsync();
                        continue;
                    }
                }
                if (acao == "3")
                {
                    Console.Clear();
                    System.Console.WriteLine("- PROCURE POR LOGIN -");

                    System.Console.Write("LOGIN: ");
                    string login = Console.ReadLine()!;

                    Console.Clear();
                    System.Console.WriteLine("---------------- TABELA ------------------");
                    System.Console.WriteLine("| ID - LOGIN - SENHA - ÚLTIMO LOGIN |");
                    System.Console.WriteLine();
                    var tabela = await conexao.conn.QueryAsync<Usuario>(
                        "select * from usuario " +
                        "where login like @login or senha like @login " +
                        "order by ultimologin desc",
                        new
                        {
                            login = $"%{login}%" // Primeiro login é do @login
                        }
                    );
                    foreach (var item in tabela)
                    {
                        Console.WriteLine($"| {item.id} - {item.login} - {item.senha} - {item.ultimologin} |");
                    }
                    
                    System.Console.WriteLine("- Pressione Enter para voltar -");

                    Console.ReadKey();
                    await conexao.conn.CloseAsync();
                    continue;
                }
                else
                {
                    System.Console.WriteLine("Finalizou");
                    await conexao.conn.CloseAsync();
                    break;
                }
            }
            System.Console.WriteLine("Final");
        }
    }
}
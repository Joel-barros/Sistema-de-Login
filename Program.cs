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

namespace SistemaLogin
{
    public class Program
    {
        public static void Main(string[] args)
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
                    conexao.conn!.Open();

                    Console.Write("LOGIN: ");
                    string login = Console.ReadLine()!;
                    Console.Write("SENHA: ");
                    string senha = Console.ReadLine()!;

                    // if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(senha))
                    // {
                    //     Console.WriteLine("Login e Senha não podem ser vazio! Tente Novamente.");
                    //     conexao.conn!.Close();
                    //     continue;
                    // }

                    bool login_senha = conexao.VerificarLogin(login, senha);

                    if (login_senha)
                    {
                        // MySqlCommand comando = new MySqlCommand();
                        // comando.Connection = conexao.conn;
                        // comando.CommandText = "update usuario set ultimologin = '@ultimologin' where login = '@login' and senha = '@senha'";
                        // comando.Prepare();

                        // DateTime data = DateTime.Now;

                        // comando.Parameters.AddWithValue("@ultimologin", data);
                        // comando.Parameters.AddWithValue("@login", login);
                        // comando.Parameters.AddWithValue("@senha", senha);

                        // comando.ExecuteNonQuery();
                        DateTime data = DateTime.Now;
                        string sql = $"update usuario set ultimologin = '{data.ToString("yyyy/MM/dd HH:mm:ss")}' where login = '{login}' and senha = '{senha}'";
                        MySqlCommand comando = new MySqlCommand(sql, conexao.conn);
                        comando.ExecuteNonQuery();

                        break;
                    }
                    else
                    {
                        conexao.conn!.Close();
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
            conexao.conn!.Close();
            while (true)
            {
                Console.Clear();
                conexao.conn!.Open();
                System.Console.WriteLine($"-------- SEJA BEM VINDO ----------");
                Console.WriteLine("- CENTRAL DE GERENCIAMENTO DE CONTAS -");

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

                    MySqlDataReader resultadoDoComando = conexao.Comando(sql);

                    if (resultadoDoComando == null)
                    {
                        Console.WriteLine("Por favor, tente novamente...");
                        Console.ReadKey();
                        conexao.conn!.Close();
                        continue;
                    }
                    else
                    {
                        System.Console.WriteLine("Novo Login cadastrado.");
                        System.Console.WriteLine("Pressione Enter para continuar...");
                        Console.ReadKey();
                        conexao.conn!.Close();
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

                    if (!conexao.VerificarLogin(login, senha))
                    {
                        Console.ReadKey();
                        conexao.conn.Close();
                        continue;
                    }

                    MySqlDataReader resultadoDoComando = conexao.Comando(sql);

                    if (resultadoDoComando == null) { Console.ReadKey(); conexao.conn!.Close(); continue; }
                    else
                    {
                        System.Console.WriteLine("Registro atualizado!");
                        Console.ReadKey();
                        conexao.conn!.Close();
                        continue;
                    }
                }
                if (acao == "3")
                {
                    Console.Clear();
                    System.Console.WriteLine("- PROCURE POR LOGIN -");

                    System.Console.Write("LOGIN: ");
                    string login = Console.ReadLine()!;

                    // string sql = $"select * from usuario " +
                    //               $"where login like '%{login}%' or senha like '%{login}%'" +
                    //               "order by ultimologin desc";

                    Console.Clear();
                    System.Console.WriteLine("---------------- TABELA ------------------");
                    System.Console.WriteLine("| ID - LOGIN - SENHA - ÚLTIMO LOGIN |");
                    System.Console.WriteLine();
                    //System.Console.WriteLine(conexao.ShowTable(conexao.Comando(sql)));
                    var tabela = conexao.conn.Query<Usuario>(
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
                    conexao.conn.Close();
                    continue;
                }
                else
                {
                    System.Console.WriteLine("Finalizou");
                    conexao.conn.Close();
                    break;
                }
            }
            System.Console.WriteLine("Final");
        }
    }
}
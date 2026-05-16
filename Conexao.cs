using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;

namespace SistemaLogin
{
    public class Usuario
    {
        public int id { get; set; }
        public string? login { get; set; }
        public string? senha { get; set; }
        public DateTime ultimologin { get; set; }
    }
    public class Conexao
    {
        public MySqlConnection? conn { get; private set; }
        public MySqlCommand? command { get; private set; }
        public MySqlDataReader? reader { get; private set; }
        // public MySqlCommand? command_login { get; private set; }
        // public MySqlDataReader? reader_login { get; private set; }
        public int erroLines { get; private set; }

        public Conexao()
        {
            var builder = new MySqlConnectionStringBuilder()
            {
                Server = "localhost",
                UserID = "root",
                Password = "18112003",
                Database = "loginsenha"
            };
            conn = new MySqlConnection(builder.ConnectionString);
        }
        public MySqlDataReader Comando(string sql)
        {
            try
            {
                command = new MySqlCommand(sql, conn);
                using (reader = command.ExecuteReader())
                {
                    return reader;
                }
            }
            catch (MySqlException msqlex) when (msqlex.Number == 3819)
            {
                System.Console.WriteLine("Login e Senha não podem ter espaço em branco!");
                return null!;
            }
            catch (MySqlException msqlex) when (msqlex.Number == 1406)
            {
                System.Console.WriteLine("Login ou Senha maior que o padrão!");
                return null!;
            }
            catch (MySqlException msqlex) when (msqlex.Number == 1062)
            {
                System.Console.WriteLine("Esse nome já está em uso!");
                return null!;
            }
            catch (MySqlException msqlex)
            {
                System.Console.WriteLine(msqlex.Number);
                Console.WriteLine(msqlex.Message.ToString());
                return null!;
            }
        }
        public bool VerificarLogin(string login, string senha)
        {
            try
            {
                string sql = $"select * from usuario where login = '{login}' and senha = '{senha}'";
                command = new MySqlCommand(sql, conn);
                using (reader = command.ExecuteReader())
                {
                    if (reader.Read() && login == (string)reader.GetValue(1) && senha == (string)reader.GetValue(2))
                    {
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Login ou Senha incorreta, por favor tente novamente.");
                        return false;
                    }
                }
            }
            catch (MySqlException ex) when (ex.Number == 1064)
            {
                Console.WriteLine("Esse tipo de caractere não é aceito, por favor tente novamente!");
                return false;
            }
            catch (MySqlException ex)
            {
                System.Console.WriteLine(ex.Number);
                Console.WriteLine(ex.Message.ToString());
                return false;
            }
        }
        // public bool VerificarLogin(string login)
        // {
        //     try
        //     {
        //         string sql = $"select * from usuario where login = '{login}'";
        //         command = new MySqlCommand(sql, conn);
        //         using (reader = command.ExecuteReader())
        //         {
        //             if (reader.Read() && login == (string)reader.GetValue(1))
        //             {
        //                 Console.WriteLine($"Suas informações foram atualizadas!");
        //                 return true;
        //             }
        //             else
        //             {
        //                 Console.WriteLine("Login incorreto ou não existente, por favor tente novamente.");
        //                 return false;
        //             }
        //         }
        //     }
        //     catch (MySqlException ex)
        //     {
        //         Console.WriteLine(ex.Message.ToString());
        //         return false;
        //         //throw;
        //     }
        // }
        public string ShowTable(MySqlDataReader reader)
        {
            string table = "";
            using (reader = command!.ExecuteReader())
            {
                while (reader.Read())
                {
                    table += $"| {reader.GetInt32(0)} - {reader.GetString(1)} | {reader.GetString(2)} | {reader.GetDateTime(3)} | \n";
                }
    
                return table;
            }
        }
    }
}
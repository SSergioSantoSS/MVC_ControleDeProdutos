using Dapper;
using ProjetoMVC01.Entities;
using ProjetoMVC01.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoMVC01.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        //atributo para armazenar a string de conexão do banco de dados
        private readonly string _connectionstring;

        //construtor para receber o valor da connectionstring
        public UsuarioRepository(string connectionstring)
        {
            _connectionstring = connectionstring;
        }

        public void Inserir(Usuario usuario)
        {
            using (var connection = new SqlConnection(_connectionstring))
            {
                connection.Execute("SP_INSERIRUSUARIO",
                    new
                    {
                        @NOME = usuario.Nome,
                        @EMAIL = usuario.Email,
                        @SENHA = usuario.Senha
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public Usuario Consultar(string email, string senha)
        {
            using (var connection = new SqlConnection(_connectionstring))
            {
                return connection.Query<Usuario>("SP_CONSULTARUSUARIO",
                    new
                    {
                        @EMAIL = email,
                        @SENHA = senha
                    },
                    commandType: CommandType.StoredProcedure)
                    .FirstOrDefault();
            }
        }
    }
}



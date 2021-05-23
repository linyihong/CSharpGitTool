using Dapper;
using GitTools.Models;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace GitTools
{
    public class SqlLiteGitTool : GitToolBase
    {

        public SqlLiteGitTool(string connectionString) : base(GetProjects(connectionString))
        {

        }

        private static IDictionary<string, string> GetProjects(string connectionString)
        {
            IDictionary<string, string> projects;

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                projects = connection.Query<Project>("select * from Project")
                    .ToDictionary(p => p.ProjectName, p => p.GitPath);

                connection.Close();
            }

            return projects;
        }
    }
}

using Dapper;
using GitTools.Models;
using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace GitTools
{
    public class GitHelper
    {
        public List<string> ProjectNames =>
            Projects.Select(s => s.ProjectName).ToList();

        private readonly Dictionary<string, Repository> Repositories = new Dictionary<string, Repository>();
        private readonly IConfiguration Configuration;
        private IDbConnection Connection;
        private List<Project> Projects;

        public GitHelper()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;

            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            StartUpConnection();
            GetProjects();
        }

        public GitHelper(IConfiguration configuration)
        {
            this.Configuration = configuration;

            StartUpConnection();
            GetProjects();
        }

        public Repository GetRepository(string projectName)
        {
            Repository repo;
            var project = Projects.FirstOrDefault(f => f.ProjectName == projectName);

            try
            {
                repo = Repositories.FirstOrDefault(r => r.Key == project.ProjectName).Value;

                if (repo == null)
                {
                    repo = new Repository(project.GitPath);
                    Repositories.Add(project.ProjectName, repo);
                }
            }
            catch
            {
                throw new Exception("找不到此專案，請確認路徑或名稱是否有輸入錯誤");
            }

            return repo;
        }

        public ICommitLog GetCommitLog(string projectName, string branchName)
        {
            var repo = GetRepository(projectName);
            var branch = repo.Branches.FirstOrDefault(b => b.CanonicalName.ToLower().Contains(branchName));

            return branch.Commits;
        }

        public ICommitLog GetCommitLog(string projectName)
        {
            var repo = GetRepository(projectName);

            return repo.Commits;
        }

        public List<Commit> GetAllCommits(string projectName)
        {
            return GetRepository(projectName).Branches.SelectMany(b => b.Commits.ToList()).ToList();
        }

        public List<Commit> GetCommits(string projectName, string branchName)
        {
            return GetCommitLog(projectName, branchName).ToList();
        }

        public List<Commit> GetCommits(string projectName)
        {
            return GetCommitLog(projectName).ToList();
        }

        public List<Commit> GetCommitsByAuthor(string projectName, string authorName)
        {
            return GetCommits(projectName)
                .Where(c => c.Author.Name == authorName)
                .OrderByDescending(c => c.Committer.When).ToList();
        }

        public List<Commit> GetCommitsByAuthor(string projectName, string branchName, string authorName)
        {
            return GetCommits(projectName, branchName)
                .Where(c => c.Author.Name == authorName)
                .OrderByDescending(c => c.Committer.When)
                .ToList();
        }

        private void GetProjects()
        {
            Connection.Open();

            Projects = Connection.Query<Project>("select * from Project")
                .ToList();

            Connection.Close();
        }

        private void StartUpConnection()
        {
            var configPath = Configuration?["SqliteConnectionStr"];

            if (string.IsNullOrWhiteSpace(configPath))
            {
                throw new Exception("找不到Sqlite連線字串，請重新設定");
            }

            Connection = new SQLiteConnection(configPath);
        }
    }
}

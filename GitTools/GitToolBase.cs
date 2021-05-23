using GitTools.Exceptions;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GitTools
{
    public class GitToolBase : IGitTool, IDisposable
    {
        public IEnumerable<string> ProjectNames => projects.Select(p => p.Key);

        private readonly IDictionary<string, string> projects;
        private readonly IDictionary<string, Repository> repos = new Dictionary<string, Repository>();
        private bool disposed = false;
        public GitToolBase(IDictionary<string, string> projects)
        {
            this.projects = projects;
        }

        public IDictionary<string, IEnumerable<Commit>> GetAllCommits()
        {
            return GetAllRepositories().ToDictionary(r => r.Key, r =>
            {
                return r.Value.Branches.SelectMany(b => b.Commits);
            });
        }

        public IDictionary<string, Repository> GetAllRepositories()
        {
            foreach (var p in projects.Where(p => !repos.ContainsKey(p.Key)))
            {
                repos.Add(p.Key, new Repository(p.Value));
            }

            return repos;
        }

        public ICommitLog GetBranchCommit(string projectName, string branchName)
        {
            var branch = GetRepository(projectName).Branches
                .FirstOrDefault(b => b.FriendlyName.ToLower() == branchName.ToLower());

            if (branch == null)
            {
                throw new GitToolException("未找到此分支");
            }

            return branch.Commits;
        }

        public ICommitLog GetRepositoryCommits(string projectName)
        {
            return GetRepository(projectName).Commits;
        }

        public Repository GetRepository(string projectName)
        {
            var project = projects.FirstOrDefault(p => p.Key.ToLower() == projectName.ToLower());

            if (project.Equals(default(KeyValuePair<string, string>)))
            {
                throw new GitToolException("找不到此專案，請確認路徑或名稱是否有輸入錯誤");
            }

            Repository repo;

            if (repos.ContainsKey(projectName))
            {
                repo = repos[projectName];
            }
            else
            {
                repo = new Repository(project.Value);
                repos.Add(projectName, repo);
            }

            return repo;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var repo in repos)
                {
                    repo.Value.Dispose();
                }
            }

            disposed = true;
        }
    }
}

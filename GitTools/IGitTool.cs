using LibGit2Sharp;
using System.Collections.Generic;

namespace GitTools
{
    public interface IGitTool
    {
        IEnumerable<string> ProjectNames { get; }
        IDictionary<string, Repository> GetAllRepositories();
        Repository GetRepository(string projectName);
        IDictionary<string, IEnumerable<Commit>> GetAllCommits();
        ICommitLog GetRepositoryCommits(string projectName);
        ICommitLog GetBranchCommit(string projectName, string branchName);

    }
}

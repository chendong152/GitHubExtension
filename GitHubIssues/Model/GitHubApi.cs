﻿using System;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Alteridem.GitHub.Annotations;
using NLog;
using Octokit;
using Octokit.Reactive;

namespace Alteridem.GitHub.Model
{
    public class GitHubApi : INotifyPropertyChanged
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly GitHubClient _github;
        private readonly IObservableGitHubClient _observableGitHub;
        private string _token;
        private User _user;

        #region Public Data

        public bool LoggedIn
        {
            get { return !string.IsNullOrWhiteSpace(Token); }
        }

        [CanBeNull]
        public User User
        {
            get { return _user; }
            private set
            {
                if (Equals(value, _user)) return;
                _user = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public BindingList<Repository> Repositories { get; set; }

        [NotNull]
        public BindingList<Organization> Organizations { get; set; }

        [NotNull]
        public BindingList<Issue> Issues { get; set; }

        [CanBeNull]
        private string Token
        {
            get { return _token; }
            set
            {
                if (Equals(value, _token)) return;
                _token = value;
                Settings.Token = _token;
                OnPropertyChanged();
                OnPropertyChanged("LoggedIn");
            }
        }

        #endregion

        public GitHubApi()
        {
            _github = new GitHubClient(new ProductHeaderValue("GitHubExtension"));
            _observableGitHub = new ObservableGitHubClient(_github);

            Repositories = new BindingList<Repository>();
            Organizations = new BindingList<Organization>();
            Issues = new BindingList<Issue>();

            // Get user and token from settings and log in using them
            _token = Settings.Token;
            if (!string.IsNullOrWhiteSpace(Token))
            {
                LoginWithToken();
            }
        }

        public void Logout()
        {
            log.Info( "Logging out of GitHub" );
            Token = string.Empty;
            User = null;
            Repositories.Clear();
            Organizations.Clear();
            Issues.Clear();
        }

        private class LogonWatcher : ILogonObservable
        {
            private readonly GitHubApi _parent;

            public LogonWatcher([NotNull] GitHubApi parent)
            {
                _parent = parent;
            }

            public void OnLoggingIn()
            {
                log.Info( "Logging in with authentication token" );
            }

            public void OnSuccess()
            {
                log.Info( "Successfully logged in with authentication token" );
            }

            public void OnError(Exception ex)
            {
                log.ErrorException( "Failed to login to GitHub using token", ex );
                _parent.Token = String.Empty;
            }
        }

        private void LoginWithToken()
        {
            var view = new LogonWatcher(this);
            Login(new Credentials(Token), view);
        }

        public void Login([NotNull] ILogonView view)
        {
            Login(new Credentials(view.Username, view.Password), view);
        }

        private void Login([NotNull] Credentials credentials, [NotNull] ILogonObservable view)
        {
            log.Info("Logging in with credentials");
            _github.Credentials = credentials;
            Token = string.Empty;
            var newAuth = new NewAuthorization
            {
                Scopes = new[] {"user", "repo"},
                Note = "GitHub Visual Studio Extension",
                NoteUrl = "http://www.alteridem.net"
            };

            view.OnLoggingIn();
            _observableGitHub.Authorization
                .GetOrCreateApplicationAuthentication(Secrets.CLIENT_ID, Secrets.CLIENT_SECRET, newAuth)
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(OnAuthorized, view.OnError, () =>
                {
                    view.OnSuccess();
                    LoadData();
                });
        }

        private void OnAuthorized([NotNull] Authorization auth)
        {
            log.Info( "Successfully logged in to GitHub" );
            Token = auth.Token;
        }

        private void LoadData()
        {
            GetUser();
            GetRepositories();
            GetIssues("nunit", "nunit-framework");
        }

        private void GetUser()
        {
            log.Info("Fetching current user");
            _observableGitHub.User
                .Current()
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(user =>
                {
                    log.Info("Finished fetching current user");
                    User = user;
                }, exception => log.ErrorException("Failed to fetch current user", exception) );
        }

        private void GetRepositories()
        {
            log.Info("Fetching repositories for current user");
            Repositories.Clear();
            Organizations.Clear();
            _observableGitHub.Repository
                .GetAllForCurrent()
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(NextRepository, 
                    exception => log.ErrorException("Failed to fetch repositories for current user", exception),
                    () => log.Info("Finished fetching repositories for current user"));

            log.Info("Fetching organizations for current user");
            _observableGitHub.Organization
                .GetAllForCurrent()
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(NextOrganization,
                    exception => log.ErrorException("Failed to fetch organizations for current user", exception),
                    () => log.Info("Finished fetching organizations for current user"));
        }

        private void NextRepository([NotNull] Repository repository)
        {
            log.Debug( "Fetched repository {0}", repository.FullName );
            Repositories.Add(repository);
        }

        private void NextOrganization([NotNull] Organization org)
        {
            Organizations.Add(org);

            log.Info("Fetching repositories for {0}", org.Login);
            _observableGitHub.Repository
                .GetAllForOrg(org.Login)
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(NextRepository,
                    exception => log.ErrorException("Failed to fetch repositories for " + org.Login, exception),
                    () => log.Info("Finished fetching repositories for " + org.Login));
        }

        private async void GetIssues(string owner, string name)
        {
            log.Info("Fetching repositories for {0}/{1}", owner, name);
            try
            {
                var request = new RepositoryIssueRequest();
                var issues = await _github.Issue.GetForRepository(owner, name);
                Issues.Clear();
                foreach(var issue in issues)
                {
                    Issues.Add(issue);
                }
            }
            catch ( Exception ex )
            {
                log.ErrorException( "Failed to fetch issues", ex );
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CanBeNull,CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

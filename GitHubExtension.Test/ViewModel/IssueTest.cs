﻿// **********************************************************************************
// The MIT License (MIT)
// 
// Copyright (c) 2014 Rob Prouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// **********************************************************************************

#region Using Directives

using System;
using Alteridem.GitHub.Extension.Interfaces;
using Alteridem.GitHub.Extension.Test.Mocks;
using Alteridem.GitHub.Extension.ViewModel;
using Alteridem.GitHub.Model;
using NUnit.Framework;

#endregion

namespace Alteridem.GitHub.Extension.Test.ViewModel
{
    [TestFixture]
    public class IssueTest
    {
        private IssueViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            Factory.Rebind<GitHubApiBase>().To<GitHubApiMock>().InScope(o => this);
            Factory.Rebind<ILoginView>().To<LoginView>();
            Factory.Rebind<IIssueEditor>().To<IssueEditor>();
            Factory.Rebind<IAddComment>().To<AddComment>();
            _viewModel = Factory.Get<IssueViewModel>();
        }

        [Test]
        public void TestIssue()
        {
            Assert.That(_viewModel.Issue, Is.Not.Null);
            Assert.That(_viewModel.Issue.Title, Is.EqualTo("title"));
        }

        [Test]
        public void TestIssueMarkdown()
        {
            Assert.That(_viewModel.IssueMarkdown, Is.EqualTo("##body##"));
        }

        [Test]
        public void TestCanAddComment()
        {
            Assert.That(_viewModel.CanAddComment(), Is.True);
        }

        [Test]
        public void TestCanEditIssue()
        {
            Assert.That(_viewModel.CanEditIssue(), Is.True);
        }

        [Test]
        public void TestAddComment()
        {
            var api = Factory.Get<GitHubApiBase>();
            api.IssueMarkdown = "body";

            _viewModel.AddComment();

            Assert.That(api.IssueMarkdown, Contains.Substring("new comment"));
        }

        [Test]
        public void TestEditIssue()
        {
            var api = Factory.Get<GitHubApiBase>();
            Assert.That(api.Issue, Is.Not.Null);
            api.Issue.Title = "title";
            api.Issue.Body = "body";

            _viewModel.EditIssue();

            Assert.That(api.Issue.Title, Contains.Substring("new title"));
            Assert.That(api.Issue.Body, Contains.Substring("new body"));
        }
    }
}
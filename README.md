GitHub Visual Studio Extension
===============

A visual studio extension for working with issues on GitHub. 

Access and manage GitHub issues for repositories that you have commit access to. You can filter and view issues for a repository, edit issues, add comments and close issue. This is the first Alpha release, more features are coming. 

## Download ##

The easiest way to download is by going to *Tools | Extensions* in Visual Studio and searching for the GitHub Extension. It is also available in the [Visual Studio Gallery](https://visualstudiogallery.msdn.microsoft.com/e4ba5ebd-bcd5-4e20-8375-bb8cbdd71d7e) and in the [GitHub Releases](https://github.com/rprouse/GitHubExtension/releases) for the project. 

## Instructions ##

- To view a list of open issues, go to *View | Other Windows | GitHub Issue List* (Ctrl+W, Ctrl+G)
- Log in to GitHub by clicking the logon icon at the upper right of the issue list window
- Open the issue window by double clicking an issue in the list, or by going to *View | Other Windows | GitHub Issue Window* (Ctrl+W, Ctrl+H)
- Add a new issue to the selected repository with the + button in the issue list, or from *Tools | New Issue on GitHub* (Ctrl+W, Ctrl+I)
- Edit an issue with the edit button on the Issue window
- Add comments to, or close and issue with the comment button on the issue window

## Credits ##

- Bug Icon by [David Vignoni](http://www.icon-king.com/) (LGPL)
- Button and application images by [Font Awesome](http://fortawesome.github.io/Font-Awesome/) ([SIL OFL 1.1](http://scripts.sil.org/OFL))

## Screenshots ##

### Login Window ###

![Login](http://i.imgur.com/1kmTNER.png)

### Issue List ###

![Issue List](/images/issue_list.png)

### Issue Window ###

![Issue Window](/images/issue.png)

## Building ##

You will need the Visual Studio 2013 SDK installed.

Set the GitHub `CLIENT_ID` and `CLIENT_SECRET` in `Secrets.cs` which is in the 
`Model` folder of the `GitHubIssues` project.

To debug, 

1. Set `GitHubExtension` as the startup project
2. Open the Project's properties
3. Go to the Debug Tab
4. **Start an external program:** C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe
5. **Command line arguments:** /rootsuffix Exp 
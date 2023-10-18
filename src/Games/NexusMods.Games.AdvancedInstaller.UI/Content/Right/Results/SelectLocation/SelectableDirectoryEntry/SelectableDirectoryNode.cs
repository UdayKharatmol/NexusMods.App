using System.Collections.ObjectModel;
using NexusMods.DataModel.Games;
using NexusMods.Games.AdvancedInstaller.UI.Content.Left;
using NexusMods.Paths;
using NexusMods.Paths.FileTree;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace NexusMods.Games.AdvancedInstaller.UI.Content.Right.Results.SelectLocation.SelectableDirectoryEntry;

/// <summary>
///     Represents an individual node in the 'All Folders' section when selecting a location.
/// </summary>
public interface ISelectableDirectoryNode
{
    /// <summary>
    ///     Status of the node in question.
    /// </summary>
    [Reactive]
    public SelectableDirectoryNodeStatus Status { get; }

    /// <summary>
    ///     Contains the children nodes of this node.
    /// </summary>
    /// <remarks>
    ///     See <see cref="ModContentNode{TNodeValue}.Children"/>
    /// </remarks>
    ObservableCollection<ITreeEntryViewModel> Children { get; }

    /// <summary>
    /// The Directory name displayed for this node.
    /// </summary>
    string DisplayName { get; }
}


public class SelectableDirectoryNode : ReactiveObject, ISelectableDirectoryNode
{
    [Reactive]
    public SelectableDirectoryNodeStatus Status { get; private set; } = SelectableDirectoryNodeStatus.Regular;
    public ObservableCollection<ITreeEntryViewModel> Children { get; init; } = new();
    public GamePath Path { get; init; }
    private string _displayName = string.Empty;
    public string DisplayName => _displayName != string.Empty ? _displayName : Path.FileName;

    /// <summary>
    ///     Creates nodes from a given path that is tied to a FileSystem.
    /// </summary>
    /// <param name="register">The game location register obtained from <see cref="GameInstallation"/>. Helps resolving <see cref="GamePath"/>.</param>
    /// <param name="gamePath">The path of the root node.</param>
    /// <param name="rootName">Name of the root item.</param>
    public static SelectableDirectoryNode Create(GameLocationsRegister register, GamePath gamePath, string rootName = "")
    {
        return Create(register[gamePath.LocationId], gamePath, rootName);
    }

    /// <summary>
    ///     Creates nodes from a given path that is tied to a FileSystem.
    /// </summary>
    /// <param name="absPath">Path of where <see cref="GamePath"/> points to on FileSystem.</param>
    /// <param name="gamePath">The path of the root node.</param>
    /// <param name="rootName">Name of the root item.</param>
    public static SelectableDirectoryNode Create(AbsolutePath absPath, GamePath gamePath, string rootName = "")
    {
        var finalLocation = absPath.Combine(gamePath.Path);
        var finalLocationLength = finalLocation.GetFullPathLength() + 1;
        var root = new SelectableDirectoryNode
        {
            Status = SelectableDirectoryNodeStatus.Regular,
            Path = gamePath,
            _displayName = rootName
        };
        root.CreateChildrenRecursive(finalLocation, gamePath.LocationId, finalLocationLength);
        return root;
    }

    /// <summary>
    ///     Creates nodes from a given list of files.
    /// </summary>
    /// <param name="currentDirectory">The path to the current directory.</param>
    /// <param name="locationId">The named location for the <see cref="GamePath"/>(s) to create.</param>
    /// <param name="dirSubstringLength">Precalculated length of <see cref="currentDirectory"/>.</param>
    internal void CreateChildrenRecursive(AbsolutePath currentDirectory, LocationId locationId, int dirSubstringLength)
    {
        // Get files at this level.
        foreach (var directory in currentDirectory.EnumerateDirectories("*", false))
        {
            var name = directory.GetFullPath().Substring(dirSubstringLength);
            var node = new SelectableDirectoryNode { Path = new GamePath(locationId, name) };
            node.CreateChildrenRecursive(directory, locationId, dirSubstringLength + name.Length + 1);
            Children.Add(new TreeEntryViewModel(node));
        }
    }
}


/// <summary>
///     Represents the current status of the <see cref="SelectableDirectoryNode" />.
/// </summary>
public enum SelectableDirectoryNodeStatus
{
    /// <summary>
    /// Regular selectable directory node.
    /// </summary>
    Regular,

    /// <summary>
    /// Special "Create new folder" entry node.
    /// </summary>
    Create,

    /// <summary>
    /// Create node after button was pressed, user can input the name of the new folder.
    /// </summary>
    Editing,

    /// <summary>
    /// A new node created with "Create new folder button
    /// </summary>
    Created,
}

﻿using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using NexusMods.App.UI;
using NexusMods.Paths;
using NexusMods.Paths.FileTree;

namespace NexusMods.Games.AdvancedInstaller.UI;

public class AdvancedInstallerModContentDesignViewModel : AViewModel<IAdvancedInstallerModContentViewModel>,
    IAdvancedInstallerModContentViewModel
{
    /// <summary>
    /// The visual representation of the tree.
    /// </summary>
    public HierarchicalTreeDataGridSource<IModContentFileNode> Tree => new(GetTreeData())
    {
        Columns =
        {
            new HierarchicalExpanderColumn<IModContentFileNode>(
                new TemplateColumn<IModContentFileNode>(null,
                    new FuncDataTemplate<IModContentFileNode>((node, scope) => new AdvancedInstallerTreeEntryView()
                    {
                        DataContext = node,
                    }),
                    width: new GridLength(1, GridUnitType.Star)
                    ),
                x => x.Children)
        }
    };

    protected virtual IModContentFileNode GetTreeData() => CreateTestTree();

    private static IModContentFileNode CreateTestTree()
    {
        var fileEntries = new Dictionary<RelativePath, int>
        {
            { new RelativePath("BWS.bsa"), 1 },
            { new RelativePath("BWS - Textures.bsa"), 2 },
            { new RelativePath("Readme-BWS.txt"), 3 },
            { new RelativePath("Textures/greenBlade.dds"), 4 },
            { new RelativePath("Textures/greenBlade_n.dds"), 5 },
            { new RelativePath("Textures/greenHilt.dds"), 6 },
            { new RelativePath("Textures/Armors/greenArmor.dds"), 7 },
            { new RelativePath("Textures/Armors/greenBlade.dds"), 8 },
            { new RelativePath("Textures/Armors/greenHilt.dds"), 9 },
            { new RelativePath("Meshes/greenBlade.nif"), 10 }
        };

        var tree = FileTreeNode<RelativePath, int>.CreateTree(fileEntries);
        return ModContentFileNode<RelativePath, int>.FromFileTree(tree);
    }
}



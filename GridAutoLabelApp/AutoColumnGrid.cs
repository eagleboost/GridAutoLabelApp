// Author : Shuo Zhang
// E-MAIL : eagleboost@msn.com
// Creation :2018-05-30 11:52 AM

namespace GridAutoLabelApp
{
  using System;
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// AutoColumnGrid - In case of using Grid as container for the layout, e.g. settings dialog, we usually do this
  /// 1. Put Label as first column with Width=Auto and SharedSizeGroup="Label"
  /// 2. TextBox as second column with Width=*
  /// The problem is the parent grid needs to have a lot of row definitions and when we want to move the settings around
  /// we also need to update Grid.Row and Grid.Column
  /// With this class there's no need to do that anymore
  ///     <local:AutoColumnGrid>
  ///       <Label Content = "abc" />
  ///       <TextBox/>
  ///     </local:AutoColumnGrid>
  /// </summary>
  public class AutoColumnGrid : Grid
  {
    #region Declarations
    private bool? _rearrangeRequired;
    #endregion Declarations

    #region Overrides
    protected override Size MeasureOverride(Size constraint)
    {
      if (!_rearrangeRequired.HasValue || _rearrangeRequired.Value)
      {
        if (_rearrangeRequired.HasValue)
        {
          _rearrangeRequired = false;
        }

        UpdateChildrenColumns();
      }

      return base.MeasureOverride(constraint);
    }

    protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
    {
      base.OnVisualChildrenChanged(visualAdded, visualRemoved);

      _rearrangeRequired = true;
    }

    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
      base.OnVisualParentChanged(oldParent);

      var parentGrid = (Grid)VisualParent;
      if (parentGrid != null)
      {
        SetColumnSpan(this, parentGrid.ColumnDefinitions.Count);
        RefreshParentGridRows(parentGrid);
      }
      else
      {
        parentGrid = (Grid) oldParent;
        RefreshParentGridRows(parentGrid);
      }
    }
    #endregion Overrides

    #region Private Methods
    private void UpdateChildrenColumns()
    {
      RowDefinitions.Clear();
      ColumnDefinitions.Clear();

      var parentGrid = (Grid)VisualParent;
      for (var i = 0; i < Children.Count; i++)
      {
        var parentColumnDef = parentGrid.ColumnDefinitions[i];
        var columnDef = new ColumnDefinition
        {
          Width = parentColumnDef.Width,
          SharedSizeGroup = parentColumnDef.SharedSizeGroup
        };
        ColumnDefinitions.Add(columnDef);

        var child = Children[i];
        SetColumn(child, i);
      }
    }

    private void RefreshParentGridRows(Grid parentGrid)
    {
      var parentRows = parentGrid.RowDefinitions;
      parentRows.Clear();

      for (var i = 0; i < parentGrid.Children.Count; i++)
      {
        var child = parentGrid.Children[i];
        ////child would be null when this control is removed from the parent grid
        if (child != null)
        {
          parentRows.Add(new RowDefinition());
          SetRow(child, parentRows.Count - 1);
        }
      }
    }
    #endregion Private Methods
  }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace Callidex
{
    public class CustomDataGrid : DataGrid
    {
        static CustomDataGrid()
        {
            CommandManager.RegisterClassCommandBinding(
                typeof(CustomDataGrid),
                new CommandBinding(ApplicationCommands.Paste,
                   new ExecutedRoutedEventHandler(  OnExecutedPaste),
                  new CanExecuteRoutedEventHandler(  OnCanExecutePaste)));

            CommandManager.RegisterClassCommandBinding(
                typeof(CustomDataGrid),
                new CommandBinding(ApplicationCommands.Copy,
                     new ExecutedRoutedEventHandler(OnExecutedCopy),
                    new CanExecuteRoutedEventHandler(OnCanExecuteCopy)));
        }

        #region Clipboard Paste

        private static void OnCanExecutePaste(object target, CanExecuteRoutedEventArgs args)
        {
            ((CustomDataGrid)target).OnCanExecutePaste(args);
        }

        protected virtual void OnCanExecutePaste(CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = CurrentCell != null;
            args.Handled = true;
        }

        private static void OnExecutedPaste(object target, ExecutedRoutedEventArgs args)
        {
            ((CustomDataGrid)target).OnExecutedPaste(args);
        }

        protected virtual void OnExecutedPaste(ExecutedRoutedEventArgs args)
        {
            // parse the clipboard data            
            List<string[]> rowData = ClipboardHelper.ParseClipboardData();

            // call OnPastingCellClipboardContent for each cell
            int minRowIndex = Items.IndexOf(CurrentItem);
            int maxRowIndex = Items.Count - 1;
            int minColumnDisplayIndex = (SelectionUnit != DataGridSelectionUnit.FullRow) ? Columns.IndexOf(CurrentColumn) : 0;
            int maxColumnDisplayIndex = Columns.Count - 1;
            int rowDataIndex = 0;
            bool hasAddedNewRow = false;
            for (int i = minRowIndex; i <= maxRowIndex && rowDataIndex < rowData.Count; i++, rowDataIndex++)
            {

                if (i == maxRowIndex)
                {
                    
                    maxRowIndex++;
                    hasAddedNewRow = true;
                }
                int columnDataIndex = 0;
                for (int j = minColumnDisplayIndex; j <= maxColumnDisplayIndex && columnDataIndex < rowData[rowDataIndex].Length; j++, columnDataIndex++)
                {
                    DataGridColumn column = ColumnFromDisplayIndex(j);
                    column.OnPastingCellClipboardContent(Items[i], rowData[rowDataIndex][columnDataIndex]);
                }

            }
            if (hasAddedNewRow)
            {
                UnselectAll();
                UnselectAllCells();

                CurrentItem = Items[minRowIndex];
                if (SelectionUnit == DataGridSelectionUnit.FullRow)
                {
                    SelectedItem = Items[minRowIndex];

                }
                else if (SelectionUnit == DataGridSelectionUnit.CellOrRowHeader || SelectionUnit==DataGridSelectionUnit.Cell)
                {
                    SelectedCells.Add(new DataGridCellInfo(Items[minRowIndex],Columns[minColumnDisplayIndex]));
                }
            }
        }

        #endregion Clipboard Paste

        #region Clipboard Copy

        private static void OnCanExecuteCopy(object target, CanExecuteRoutedEventArgs args)
        {
            ((CustomDataGrid)target).OnCanExecuteCopy(args);
        }

        protected virtual void OnCanExecuteCopy(CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = CurrentCell != null;
            args.Handled = true;
        }

        private static void OnExecutedCopy(object target, ExecutedRoutedEventArgs args)
        {
            ((CustomDataGrid)target).OnExecutedCopy(args);
        }

        protected virtual void OnExecutedCopy(ExecutedRoutedEventArgs args)
        {
            // Now find all selected cells and construct the right value (not display value)
            // parse the clipboard data            
            List<string[]> rowData = ClipboardHelper.ParseClipboardData();

            // call OnPastingCellClipboardContent for each cell
            int minRowIndex = Items.IndexOf(CurrentItem);
            int maxRowIndex = Items.Count - 1;
            int minColumnDisplayIndex = (SelectionUnit != DataGridSelectionUnit.FullRow) ? Columns.IndexOf(CurrentColumn) : 0;
            int maxColumnDisplayIndex = Columns.Count - 1;
            int rowDataIndex = 0;
            bool hasAddedNewRow = false;
            for (int i = minRowIndex; i <= maxRowIndex && rowDataIndex < rowData.Count; i++, rowDataIndex++)
            {

                if (i == maxRowIndex)
                {

                    maxRowIndex++;
                    hasAddedNewRow = true;
                }
                int columnDataIndex = 0;
                for (int j = minColumnDisplayIndex; j <= maxColumnDisplayIndex && columnDataIndex < rowData[rowDataIndex].Length; j++, columnDataIndex++)
                {
                    DataGridColumn column = ColumnFromDisplayIndex(j);
                    column.OnPastingCellClipboardContent(Items[i], rowData[rowDataIndex][columnDataIndex]);
                }

            }
            if (hasAddedNewRow)
            {
                UnselectAll();
                UnselectAllCells();

                CurrentItem = Items[minRowIndex];
                if (SelectionUnit == DataGridSelectionUnit.FullRow)
                {
                    SelectedItem = Items[minRowIndex];

                }
                else if (SelectionUnit == DataGridSelectionUnit.CellOrRowHeader || SelectionUnit == DataGridSelectionUnit.Cell)
                {
                    SelectedCells.Add(new DataGridCellInfo(Items[minRowIndex], Columns[minColumnDisplayIndex]));
                }
            }
        }

        #endregion Clipboard Copy

    }
}

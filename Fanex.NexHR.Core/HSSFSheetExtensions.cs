// --------------------------------------------------------------------------
// <copyright file="HSSFSheetExtensions.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.20">Created</change>
// </history>

namespace Fanex.NexHR.Core
{
    using System;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// HSSFSheet Extensions.
    /// </summary>
    public static class HSSFSheetExtensions
    {
        /// <summary>
        /// CloneSheet method is used to copy a sheet to new sheet.
        /// </summary>
        /// <param name="workbook">Target workbook.</param>
        /// <param name="sourceSheetName">Template sheet name.</param>
        /// <param name="newSheetName">New sheet name.</param>
        /// <returns>Return the copied sheet.</returns>
        public static ISheet CloneSheet(this IWorkbook workbook, string sourceSheetName, string newSheetName)
        {
            var sheetIndex = workbook.GetSheetIndex(sourceSheetName);
            var newSheet = workbook.CloneSheet(sheetIndex);
            var newSheetIndex = workbook.GetSheetIndex(newSheet);
            workbook.SetSheetName(newSheetIndex, newSheetName);
            return newSheet;
        }

        /// <summary>
        /// Remove the sheet by sheet name.
        /// </summary>
        /// <param name="workbook">Target workbook.</param>
        /// <param name="sheetName">The sheet name.</param>
        public static void RemoveSheet(this IWorkbook workbook, string sheetName)
        {
            var sheet = workbook.GetSheet("Sheet1");

            if (sheet != null)
            {
                var sheetIndex = workbook.GetSheetIndex(sheet);
                workbook.RemoveSheetAt(sheetIndex);
            }
        }

        /// <summary>
        /// CopyRow method is used to copy one row from a template to another template.
        /// </summary>
        /// <param name="sheet">Main sheet object.</param>
        /// <param name="sourceSheet">Template sheet.</param>
        /// <param name="sourceRowIndex">Index of source row in template sheet, base by zero.</param>
        /// <param name="destRowIndex">Index of destination row in main sheet, base by zero.</param>
        /// <returns>Return the copied row.</returns>
        public static IRow CopyRow(this ISheet sheet, ISheet sourceSheet, int sourceRowIndex, int destRowIndex)
        {
            if (sheet.LastRowNum < destRowIndex)
            {
                for (var i = sheet.LastRowNum + 1; i <= destRowIndex; i++)
                {
                    sheet.CreateRow(i);
                }
            }

            // Get the source / new row
            var sourceRow = sourceSheet.GetRow(sourceRowIndex);
            if (sourceRow == null)
            {
                return null;
            }

            var newRow = sheet.GetRow(destRowIndex) ?? sheet.CreateRow(destRowIndex);

            // Loop through source columns to add to new row
            for (int i = 0; i < sourceRow.LastCellNum; i++)
            {
                // Grab a copy of the old/new cell
                var oldCell = sourceRow.GetCell(i);
                var newCell = newRow.CreateCell(i);

                sheet.CopyCell(oldCell, newCell);
            }

            return newRow;
        }

        /// <summary>
        /// Copy a cell to another cell.
        /// </summary>
        /// <param name="sheet">Main sheet object.</param>
        /// <param name="sourceCell">Source cell.</param>
        /// <param name="destCell">Destination cell.</param>
        /// <returns>A copy cell.</returns>
        public static ICell CopyCell(this ISheet sheet, ICell sourceCell, ICell destCell)
        {
            //// If the old cell is null jump to next cell
            if (sourceCell == null)
            {
                return null;
            }

            destCell.CellStyle = sourceCell.CellStyle;
            //// If there is a cell comment, copy
            if (destCell.CellComment != null)
            {
                destCell.CellComment = sourceCell.CellComment;
            }

            //// If there is a cell hyperlink, copy
            if (sourceCell.Hyperlink != null)
            {
                destCell.Hyperlink = sourceCell.Hyperlink;
            }

            //// Set the cell data type
            destCell.SetCellType(sourceCell.CellType);
            switch (sourceCell.CellType)
            {
                case CellType.BLANK:
                    destCell.SetCellValue(sourceCell.StringCellValue);
                    break;
                case CellType.BOOLEAN:
                    destCell.SetCellValue(sourceCell.BooleanCellValue);
                    break;

                case CellType.ERROR:
                    destCell.SetCellErrorValue(sourceCell.ErrorCellValue);
                    break;

                case CellType.NUMERIC:
                    destCell.SetCellValue(sourceCell.NumericCellValue);
                    break;

                case CellType.STRING:
                    destCell.SetCellValue(sourceCell.RichStringCellValue);
                    break;

                case CellType.Unknown:
                    destCell.SetCellValue(sourceCell.StringCellValue);
                    break;
            }

            return destCell;
        }

        /// <summary>
        /// MergeCells method is used to Merge a region.
        /// </summary>
        /// <param name="sheet">Main sheet object.</param>
        /// <param name="firstRow">First row index, base by zero.</param>
        /// <param name="firstCol">First column index, , base by zero.</param>
        /// <param name="lastRow">Last row index, base by zero.</param>
        /// <param name="lastCol">Last column Index.</param>
        /// <returns>Return affected cell count.</returns>
        public static int MergeCells(this ISheet sheet, int firstRow, int firstCol, int lastRow, int lastCol)
        {
            var rangeAddress = new CellRangeAddress(firstRow, lastRow, firstCol, lastCol);
            return sheet.AddMergedRegion(rangeAddress);
        }

        /// <summary>
        /// Assign decimal value to cell.
        /// </summary>
        /// <param name="cell">Target cell.</param>
        /// <param name="value">The value to be set.</param>
        public static void SetCellValue(this ICell cell, decimal value)
        {
            cell.SetCellValue(Convert.ToDouble(value));
        }

        /// <summary>
        /// Assign object value to cell.
        /// </summary>
        /// <param name="cell">Target cell.</param>
        /// <param name="value">The value to be set.</param>
        public static void SetCellValue(this ICell cell, object value)
        {
            var valueType = value.GetType();

            if (valueType == typeof(bool))
            {
                cell.SetCellValue(Convert.ToBoolean(value));
                return;
            }

            if (valueType == typeof(DateTime))
            {
                cell.SetCellValue(Convert.ToDateTime(value));
                return;
            }

            if (valueType == typeof(decimal))
            {
                cell.SetCellValue(Convert.ToDecimal(value));
                return;
            }

            if (valueType == typeof(double))
            {
                cell.SetCellValue(Convert.ToDouble(value));
                return;
            }

            if (valueType == typeof(int))
            {
                cell.SetCellValue(Convert.ToInt32(value));
                return;
            }

            if (valueType == typeof(long))
            {
                cell.SetCellValue(Convert.ToInt64(value));
                return;
            }

            if (valueType == typeof(string))
            {
                cell.SetCellValue(Convert.ToString(value));
            }
        }
    }
}

// --------------------------------------------------------------------------
// <copyright file="ExportExcelService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.20">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;

    /// <summary>
    /// Export Excel Service.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
    public class ExportExcelService : IExportExcelService
    {
        private const int _RowStartBodyAttendance = 3;
        private const int _RowStartBodyLeaveSummary = 4;
        private const int _RowStartBodyWorkingTimeRequest = 3;
        private const string _AttendanceFilePathExcelTemplate = "~\\ExcelTemplates\\AttendanceReportTemplate.xls";
        private const string _LeaveSummaryFilePathExcelTemplate = "~\\ExcelTemplates\\LeaveSummaryTemplate.xls";
        private const string _WorkingTimeRequestFilePathExcelTemplate = "~\\ExcelTemplates\\WorkingTimeRequestTemplate.xls";

        private const string _MsgLeave = "{1} ({0} days)";
        private const string _MsgLeaveHalfDay = "{0} ({1}-{2}) 0.5 day";
        private const string _MsgLeaveOneDay = "{0} (1 day)";
        private const string _MsgLeaveDuringWorking = "{0} ({1}-{2}) {3} munites";
        private const string _MsgChangeShift = "{0} --> {1} ({2})";
        private const string _MsgCheckInOut = "{0} {1} ({2})";
        private const string _MsgLateIn = "+{0}' {1} ({2})";
        private const string _MsgEarlyOut = "-{0}' {1} ({2})";
        private const string _MsgOvertimeHours = "{0} ({1}-{2}) {3}' - {4}";
        private const string _MsgOvertimeShift = "{0} ({1}) {2}";

        /// <summary>
        /// Export excel total working hours report.
        /// </summary>
        /// <param name="listTotalWorkingHours">List total working hours object.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">The to date.</param>
        /// <returns>HSSF Workbook.</returns>
        public HSSFWorkbook ExportTotalWorkingHours(IList<TotalWorkingHours> listTotalWorkingHours, DateTime fromDate, DateTime toDate)
        {
            // Create a new workbook and a sheet named "User Accounts"
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("Total Working Hours");
            var rowTitle = sheet.CreateRow(1);
            var titleString = "Total Working Hours: " + fromDate.ToString("dd MMM, yyyy") + " - " + toDate.ToString("dd MMM, yyyy");

            var font = workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;

            var cell = rowTitle.CreateCell(1);
            cell.SetCellValue(titleString);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.SetFont(font);

            sheet.AddMergedRegion(new CellRangeAddress(1, 1, 1, 12));

            // header
            var rowIndex = 3;
            var row = sheet.CreateRow(rowIndex);
            var font1 = workbook.CreateFont();
            font1.FontHeightInPoints = 11;
            font1.Color = NPOI.HSSF.Util.HSSFColor.GREY_80_PERCENT.WHITE.index;
            font1.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;

            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.GREY_80_PERCENT.index;
            cellStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
            cellStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.GREY_80_PERCENT.index;
            cellStyle.Alignment = HorizontalAlignment.CENTER;
            cellStyle.VerticalAlignment = VerticalAlignment.CENTER;
            cellStyle.SetFont(font1);

            var cell1 = row.CreateCell(0);
            cell1.CellStyle = cellStyle;
            cell1.SetCellValue("No.");

            var cell2 = row.CreateCell(1);
            cell2.CellStyle = cellStyle;
            cell2.SetCellValue(Labels.Code);

            var cell3 = row.CreateCell(2);
            cell3.CellStyle = cellStyle;
            cell3.SetCellValue(Labels.Name);

            var cell4 = row.CreateCell(3);
            cell4.CellStyle = cellStyle;
            cell4.SetCellValue(Labels.Nick);

            var cell5 = row.CreateCell(4);
            cell5.CellStyle = cellStyle;
            cell5.SetCellValue(Labels.Team);

            var cell6 = row.CreateCell(5);
            cell6.CellStyle = cellStyle;
            cell6.SetCellValue(Labels.WorkigDay);

            var cell7 = row.CreateCell(6);
            cell7.CellStyle = cellStyle;
            cell7.SetCellValue(Labels.ActualWorkingDay);

            var cell8 = row.CreateCell(7);
            cell8.CellStyle = cellStyle;
            cell8.SetCellValue("Leave Taken");

            var cell9 = row.CreateCell(8);
            cell9.CellStyle = cellStyle;
            cell9.SetCellValue("Unpaid Leave");

            var cell10 = row.CreateCell(9);
            cell10.CellStyle = cellStyle;
            cell10.SetCellValue(Labels.LateIn);

            var cell11 = row.CreateCell(10);
            cell11.CellStyle = cellStyle;
            cell11.SetCellValue(Labels.EarlyOut);

            var cell12 = row.CreateCell(11);
            cell12.CellStyle = cellStyle;
            cell12.SetCellValue(Labels.TotalHours);

            var cell13 = row.CreateCell(12);
            cell13.CellStyle = cellStyle;
            cell13.SetCellValue("Average Hours");
            //// end header

            // body
            rowIndex++;

            ICellStyle bodyStyle = workbook.CreateCellStyle();

            ICellStyle bodyStyle1 = workbook.CreateCellStyle();
            bodyStyle1.Alignment = HorizontalAlignment.CENTER;
            bodyStyle1.VerticalAlignment = VerticalAlignment.CENTER;

            ICellStyle bodyStyle2 = workbook.CreateCellStyle();
            bodyStyle2.Alignment = HorizontalAlignment.CENTER;
            bodyStyle2.VerticalAlignment = VerticalAlignment.CENTER;
            bodyStyle2.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LIGHT_CORNFLOWER_BLUE.index;
            bodyStyle2.FillPattern = FillPatternType.SOLID_FOREGROUND;
            bodyStyle2.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.LIGHT_CORNFLOWER_BLUE.index;

            ICellStyle bodyStyle3 = workbook.CreateCellStyle();
            bodyStyle3.Alignment = HorizontalAlignment.LEFT;

            ICellStyle bodyStyle4 = workbook.CreateCellStyle();
            bodyStyle4.Alignment = HorizontalAlignment.LEFT;
            bodyStyle4.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LIGHT_CORNFLOWER_BLUE.index;
            bodyStyle4.FillPattern = FillPatternType.SOLID_FOREGROUND;
            bodyStyle4.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.LIGHT_CORNFLOWER_BLUE.index;
            ICell cellBody;
            //// Add data rows
            foreach (var totalWorkingHours in listTotalWorkingHours)
            {
                if (rowIndex % 2 == 0)
                {
                    bodyStyle = bodyStyle2;
                }
                else
                {
                    bodyStyle = bodyStyle1;
                }

                row = sheet.CreateRow(rowIndex);
                cellBody = row.CreateCell(0);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(rowIndex - 3);

                cellBody = row.CreateCell(1);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.Employee.Code.Trim());

                bodyStyle3.Alignment = HorizontalAlignment.LEFT;
                cellBody = row.CreateCell(2);
                if (rowIndex % 2 == 0)
                {
                    cellBody.CellStyle = bodyStyle4;
                }
                else
                {
                    cellBody.CellStyle = bodyStyle3;
                }

                cellBody.SetCellValue(totalWorkingHours.Employee.FullName);

                // bodyStyle.Alignment = HorizontalAlignment.CENTER;
                cellBody = row.CreateCell(3);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.Employee.NickName);

                cellBody = row.CreateCell(4);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.Employee.Team.Name);

                cellBody = row.CreateCell(5);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.WorkingDay);

                cellBody = row.CreateCell(6);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.ActualWorkingDay);

                cellBody = row.CreateCell(7);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.LeaveTaken);

                cellBody = row.CreateCell(8);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.UnpaidLeave);

                cellBody = row.CreateCell(9);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.LateInMins);

                cellBody = row.CreateCell(10);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.EarlyOutMins);

                cellBody = row.CreateCell(11);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.TotalHours.ToString("#.##"));

                cellBody = row.CreateCell(12);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(totalWorkingHours.AverageHours.ToString("#.##"));

                rowIndex++;
            }
            ////end body

            sheet.CreateFreezePane(0, 4);
            //// Auto-size each column
            for (var i = 0; i < sheet.GetRow(3).LastCellNum; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            return workbook;
        }

        /// <summary>
        /// Function to Export attendance report.
        /// </summary>
        /// <param name="attendanceList">List EmployeeAttendanceModel need export.</param>
        /// <param name="fromDate">From Date which list object is filter from it.</param>
        /// <param name="toDate">To Date which list object is filter from it.</param>
        /// <returns>HSSF Workbook.</returns>
        public HSSFWorkbook ExportAttendance(IList<EmployeeAttendanceModel> attendanceList, DateTime fromDate, DateTime toDate)
        {
            HSSFWorkbook hssfworkbook;

            var templatePath = HttpContext.Current.Server.MapPath(_AttendanceFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, System.IO.FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "AttendanceReport");

            //// render body
            var rowIndex = _RowStartBodyAttendance;
            var rowCopy = _RowStartBodyAttendance;
            var height = sheet.GetRow(_RowStartBodyAttendance).Height;
            
            foreach (var item in attendanceList)
            {
                rowCopy = RenderBodyAttendance(templateSheet, sheet, rowIndex, rowCopy, height, item);

                rowIndex++;
            }
            //// end render body

            //// remove sheet & row template
            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        /// <summary>
        /// Export Leave summary report.
        /// </summary>
        /// <param name="leaveSummaryList">Leave Summary List.</param>
        /// <param name="year">Year which query report.</param>
        /// <returns>HSSF Workbook.</returns>
        public HSSFWorkbook ExportLeaveSummary(IList<LeaveSummaryByTeam> leaveSummaryList, int year)
        {
            HSSFWorkbook hssfworkbook;

            var templatePath = HttpContext.Current.Server.MapPath(_LeaveSummaryFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, System.IO.FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "LeaveSummary");

            //// render hearder
            var rowTakeLeave = sheet.GetRow(2);
            var cellTakeLeave = rowTakeLeave.GetCell(5);
            cellTakeLeave.SetCellValue("Take annual leave " + year);

            var rowAnnualleave = sheet.GetRow(3);
            var cellAnnualleaveLastYear = rowAnnualleave.GetCell(3);
            var cellAnnualleaveCurrentYear = rowAnnualleave.GetCell(4);
            cellAnnualleaveLastYear.SetCellValue(year - 1);
            cellAnnualleaveCurrentYear.SetCellValue(year);

            var rowAnnualBalance = sheet.GetRow(2);
            var cellAnnualBalance = rowAnnualBalance.GetCell(17);
            cellAnnualBalance.SetCellValue("Balance Of AL " + year);

            //// render body
            var rowIndex = _RowStartBodyLeaveSummary;
            var rowCopy = _RowStartBodyLeaveSummary;
            var height = sheet.GetRow(_RowStartBodyLeaveSummary).Height;
            var index = 1;

            foreach (var item in leaveSummaryList)
            {
                if (item.Team != null && item.LeaveSummaryInTeam.Count > 0)
                {
                    //// add row team
                    rowCopy = _RowStartBodyLeaveSummary + 2;
                    sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                    var rowTeam = sheet.GetRow(rowIndex);
                    var cellTeam = rowTeam.GetCell(0);
                    cellTeam.SetCellValue(item.Team.Name.ToUpper());
                    sheet.MergeCells(rowIndex, 0, rowIndex, 18);
                    rowTeam.Height = height;

                    rowIndex++;
                    foreach (var leaveSummary in item.LeaveSummaryInTeam)
                    {
                        rowCopy = _RowStartBodyLeaveSummary + (index % 2);
                        sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                        var row = sheet.GetRow(rowIndex);
                        row.Height = height;

                        var cellNo = row.GetCell(0);
                        var cellNick = row.GetCell(1);
                        var cellName = row.GetCell(2);
                        var cellLastYear = row.GetCell(3);
                        var cellCurrentYear = row.GetCell(4);
                        var cellJan = row.GetCell(5);
                        var cellFeb = row.GetCell(6);
                        var cellMar = row.GetCell(7);
                        var cellApr = row.GetCell(8);
                        var cellMay = row.GetCell(9);
                        var cellJun = row.GetCell(10);
                        var cellJul = row.GetCell(11);
                        var cellAug = row.GetCell(12);
                        var cellSep = row.GetCell(13);
                        var cellOct = row.GetCell(14);
                        var cellNov = row.GetCell(15);
                        var cellDec = row.GetCell(16);
                        var cellBalance = row.GetCell(17);
                        var cellRemark = row.GetCell(18);
                        
                        cellNo.SetCellValue(index);
                        cellNick.SetCellValue(leaveSummary.Employee.NickName);
                        cellName.SetCellValue(leaveSummary.Employee.FullName);
                        cellLastYear.SetCellValue(leaveSummary.AnnualLeaveLastYear);
                        cellCurrentYear.SetCellValue(leaveSummary.AnnualLeaveCurrentYear);
                        cellJan.SetCellValue(leaveSummary.Jan);
                        cellFeb.SetCellValue(leaveSummary.Feb);
                        cellMar.SetCellValue(leaveSummary.Mar);
                        cellApr.SetCellValue(leaveSummary.Apr);
                        cellMay.SetCellValue(leaveSummary.May);
                        cellJun.SetCellValue(leaveSummary.Jun);
                        cellJul.SetCellValue(leaveSummary.Jul);
                        cellAug.SetCellValue(leaveSummary.Aug);
                        cellSep.SetCellValue(leaveSummary.Sep);
                        cellOct.SetCellValue(leaveSummary.Oct);
                        cellNov.SetCellValue(leaveSummary.Nov);
                        cellDec.SetCellValue(leaveSummary.Dec);
                        cellBalance.SetCellValue(leaveSummary.LeaveBalance);
                        cellRemark.SetCellValue(leaveSummary.Remark);

                        index++;
                        rowIndex++;
                    }
                }
            }
            //// end render body

            //// remove sheet & row template
            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        /// <summary>
        /// Export WorkingTime Request.
        /// </summary>
        /// <param name="requestList">Request List.</param>
        /// <returns>HSSF Workbook.</returns>
        public HSSFWorkbook ExportWorkingTimeRequest(IList<RequestEntity> requestList)
        {
            HSSFWorkbook hssfworkbook;
            var templatePath = HttpContext.Current.Server.MapPath(_WorkingTimeRequestFilePathExcelTemplate);
            using (FileStream template = new FileStream(templatePath, FileMode.Open, System.IO.FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(template);
            }

            var templateSheet = hssfworkbook.GetSheet("sheet1");
            var sheet = hssfworkbook.CloneSheet("sheet1", "WorkingTimeRequest");
            var rowIndex = _RowStartBodyWorkingTimeRequest;
            var rowCopy = _RowStartBodyWorkingTimeRequest;
            var height = sheet.GetRow(_RowStartBodyWorkingTimeRequest).Height;
            ICell cellNo = null;
            ICell cellTeam = null;
            ICell cellNick = null;
            ICell cellName = null;
            ICell cellRequest = null;
            ICell cellDetails = null;
            ICell cellRemarks = null;
            ICell cellCreateDate = null;
            ICell cellStatus = null;

            foreach (var item in requestList)
            {
                rowCopy = _RowStartBodyWorkingTimeRequest + (rowIndex % 2);
                sheet.CopyRow(templateSheet, rowCopy, rowIndex);
                var row = sheet.GetRow(rowIndex);
                row.Height = height;
                RenderBodyWorkingTimeRequest(
                    rowIndex, 
                    ref cellNo, 
                    ref cellTeam, 
                    ref cellNick, 
                    ref cellName, 
                    ref cellRequest, 
                    ref cellDetails, 
                    ref cellRemarks, 
                    ref cellCreateDate, 
                    ref cellStatus, 
                    item, 
                    row);
                rowIndex++;
            }

            hssfworkbook.RemoveSheetAt(0);

            return hssfworkbook;
        }

        /// <summary>
        /// Render Body WorkingTime Request.
        /// </summary>
        /// <param name="rowIndex">Row Index to render data.</param>
        /// <param name="cellNo">The cellNo.</param>
        /// <param name="cellTeam">The cellTeam.</param>
        /// <param name="cellNick">The cellNick.</param>
        /// <param name="cellName">The cellName.</param>
        /// <param name="cellRequest">The cellRequest.</param>
        /// <param name="cellDetails">The cellDetails.</param>
        /// <param name="cellRemarks">The cellRemarks.</param>
        /// <param name="cellCreateDate">The cellCreateDate.</param>
        /// <param name="cellStatus">The cellStatus.</param>
        /// <param name="item">Item data.</param>
        /// <param name="row">Row to render data.</param>
        private void RenderBodyWorkingTimeRequest(
            int rowIndex, 
            ref ICell cellNo, 
            ref ICell cellTeam, 
            ref ICell cellNick, 
            ref ICell cellName, 
            ref ICell cellRequest, 
            ref ICell cellDetails, 
            ref ICell cellRemarks, 
            ref ICell cellCreateDate, 
            ref ICell cellStatus, 
            RequestEntity item, 
            IRow row)
        {
            cellNo = row.GetCell(0);
            cellTeam = row.GetCell(1);
            cellNick = row.GetCell(2);
            cellName = row.GetCell(3);
            cellRequest = row.GetCell(4);
            cellDetails = row.GetCell(5);
            cellRemarks = row.GetCell(6);
            cellCreateDate = row.GetCell(7);
            cellStatus = row.GetCell(8);

            cellNo.SetCellValue(rowIndex - _RowStartBodyLeaveSummary + 2);
            cellTeam.SetCellValue(item.Requester.Team.Code);
            cellNick.SetCellValue(item.Requester.NickName);
            cellName.SetCellValue(item.Requester.FullName);
            if (item.Type.Id == (int)RequestTypes.Leave)
            {
                cellRequest.SetCellValue(item.LeaveType);
            }
            else
            {
                cellRequest.SetCellValue(item.Type.Name);
            }

            cellDetails.SetCellValue(this.GetRequestDetail(item));
            cellRemarks.SetCellValue(item.Remark);
            cellCreateDate.SetCellValue(item.CreatedDate.ToShortFormatedDate());
            cellStatus.SetCellValue(item.Status.Name);
        }

        /// <summary>
        /// Render Body Attendance.
        /// </summary>
        /// <param name="templateSheet">Template Sheet.</param>
        /// <param name="sheet">Destination sheet.</param>
        /// <param name="rowIndex">Row Index to render data.</param>
        /// <param name="rowCopy">Row Index to copy.</param>
        /// <param name="height">Height of row.</param>
        /// <param name="item">Employee Attendance Model.</param>
        /// <returns>Index of Row Copy.</returns>
        private int RenderBodyAttendance(ISheet templateSheet, ISheet sheet, int rowIndex, int rowCopy, short height, EmployeeAttendanceModel item)
        {
            var requests = string.Empty;
            var overtimes = string.Empty;

            if (item.LateIn > 30 || item.EarlyOut > 30)
            {
                rowCopy = _RowStartBodyAttendance + 2;
            }
            else
            {
                rowCopy = _RowStartBodyAttendance + ((rowIndex - _RowStartBodyAttendance) % 2);
            }

            sheet.CopyRow(templateSheet, rowCopy, rowIndex);
            var row = sheet.GetRow(rowIndex);

            var cellNo = row.GetCell(0);
            var cellDate = row.GetCell(1);
            var cellCode = row.GetCell(2);
            var cellNick = row.GetCell(3);
            var cellName = row.GetCell(4);
            var cellTeam = row.GetCell(5);
            var cellShift = row.GetCell(6);
            var cellCheckIn = row.GetCell(7);
            var cellCheckOut = row.GetCell(8);
            var cellTotalHours = row.GetCell(9);
            var cellLateIn = row.GetCell(10);
            var cellEarlyOut = row.GetCell(11);
            var cellRequest = row.GetCell(12);
            var cellOvertime = row.GetCell(13);
            var cellNotes = row.GetCell(14);

            FillValueBodyAttendance(
                rowIndex, 
                height, 
                item, 
                ref requests, 
                ref overtimes, 
                row, 
                cellNo, 
                cellDate, 
                cellCode, 
                cellNick, 
                cellName, 
                cellTeam, 
                cellShift, 
                cellCheckIn, 
                cellCheckOut, 
                cellTotalHours, 
                cellLateIn, 
                cellEarlyOut, 
                cellRequest, 
                cellOvertime, 
                cellNotes);

            return rowCopy;
        }

        /// <summary>
        /// Fill Value of Body Attendance report.
        /// </summary>
        /// <param name="rowIndex">The rowIndex.</param>
        /// <param name="height">The height of row.</param>
        /// <param name="item">EmployeeAttendance Model.</param>
        /// <param name="requests">Request list in this item.</param>
        /// <param name="overtimes">Overtimes list in this item.</param>
        /// <param name="row">The row to render data.</param>
        /// <param name="cellNo">The cellNo.</param>
        /// <param name="cellDate">The cellDate.</param>
        /// <param name="cellCode">The cellCode.</param>
        /// <param name="cellNick">The cellNick.</param>
        /// <param name="cellName">The cellName.</param>
        /// <param name="cellTeam">The cellTeam.</param>
        /// <param name="cellShift">The cellShift.</param>
        /// <param name="cellCheckIn">The cellCheckIn.</param>
        /// <param name="cellCheckOut">The cellCheckOut.</param>
        /// <param name="cellTotalHours">The cellTotalHours.</param>
        /// <param name="cellLateIn">The cellLateIn.</param>
        /// <param name="cellEarlyOut">The cellEarlyOut.</param>
        /// <param name="cellRequest">The cellRequest.</param>
        /// <param name="cellOvertime">The cellOvertime.</param>
        /// <param name="cellNotes">The cellNotes.</param>
        private void FillValueBodyAttendance(
            int rowIndex, 
            short height, 
            EmployeeAttendanceModel item, 
            ref string requests, 
            ref string overtimes, 
            IRow row, 
            ICell cellNo, 
            ICell cellDate, 
            ICell cellCode, 
            ICell cellNick, 
            ICell cellName, 
            ICell cellTeam, 
            ICell cellShift, 
            ICell cellCheckIn, 
            ICell cellCheckOut, 
            ICell cellTotalHours, 
            ICell cellLateIn, 
            ICell cellEarlyOut, 
            ICell cellRequest, 
            ICell cellOvertime, 
            ICell cellNotes)
        {
            cellNo.SetCellValue(rowIndex - _RowStartBodyAttendance + 1);
            cellDate.SetCellValue(item.AttendanceDate);
            cellCode.SetCellValue(item.Code);
            cellNick.SetCellValue(item.NickName);
            cellName.SetCellValue(item.FullName);
            cellTeam.SetCellValue(item.TeamCode);
            cellShift.SetCellValue(string.IsNullOrEmpty(item.ShiftCode) ? string.Empty : (item.ShiftCode + " (" + item.ExpectedInTime + "-" + item.ExpectedOutTime + ")"));
            cellCheckIn.SetCellValue((item.ShiftId != null && item.InFixed.HasValue) ? item.InFixed.Value.ToString("HH:mm") : string.Empty);
            cellCheckOut.SetCellValue((item.ShiftId != null && item.OutFixed.HasValue) ? item.OutFixed.Value.ToString("HH:mm") : string.Empty);
            cellTotalHours.SetCellValue(item.TotalWorkHours != 0 ? Utilities.TotalHoursToTime((float)item.TotalWorkHours) : string.Empty);
            cellLateIn.SetCellValue(Utilities.MinuteToTime(item.LateIn));
            cellEarlyOut.SetCellValue(Utilities.MinuteToTime(item.EarlyOut));

            cellNotes.SetCellValue(item.Notes);

            if (item.RequestList.Count > 0)
            {
                row.Height = (short)(height * item.RequestList.Count);

                foreach (var request in item.RequestList)
                {
                    requests += "- " + request.RequestType.Name + ": " + this.GetRequestDetail(request) 
                        + (string.IsNullOrEmpty(request.Remark) ? string.Empty : " --> " + request.Remark) + "\r\n";
                }

                requests = requests.Substring(0, requests.Length - 2);
                cellRequest.SetCellValue(requests);
            }
            else
            {
                row.Height = height;
            }

            if (item.OvertimeList.Count > 0)
            {
                foreach (var overtime in item.OvertimeList)
                {
                    overtimes += overtime.OvertimeName + " ("
                        + (overtime.OvertimeFromTime.HasValue ? overtime.OvertimeFromTime.Value.ToString("HH:mm") : "N/A") + "-"
                        + (overtime.OvertimeToTime.HasValue ? overtime.OvertimeToTime.Value.ToString("HH:mm") : "N/A") + ") \r\n";
                }

                overtimes = overtimes.Substring(0, overtimes.Length - 2);
                cellOvertime.SetCellValue(overtimes);
            }
        }

        /// <summary>
        /// Get request details.
        /// </summary>
        /// <param name="request">Employee Attendance Request.</param>
        /// <returns>Request details.</returns>
        private string GetRequestDetail(EmployeeAttendanceRequest request)
        {
            var detail = string.Empty;
            var date = string.Empty;

            if (request.FromDate.HasValue && request.ToDate.HasValue)
            {
                date = RequestService.GetMessageDateforLeaveRequest(request.FromDate.Value, request.ToDate.Value);
            }

            switch (request.RequestType.Id)
            {
                case (int)RequestTypes.Leave:
                    if (request.TotalDays > 1)
                    {
                        detail = string.Format(_MsgLeave, Convert.ToInt32(request.TotalDays), date);
                    }
                    else if (request.TotalDays == 1)
                    {
                        detail = string.Format(_MsgLeaveOneDay, date);
                    }
                    else if ((request.TotalDays - (decimal)0.5) == 0)
                    {
                        // 0.5 day
                        detail = string.Format(_MsgLeaveHalfDay, date, request.FromDate.Value.ToString("HH:mm"), request.ToDate.Value.ToString("HH:mm"));
                    }
                    else
                    {
                        detail = string.Format(
                            _MsgLeaveDuringWorking, 
                            date, 
                            request.FromDate.Value.ToString("HH:mm"), 
                            request.ToDate.Value.ToString("HH:mm"), 
                            (request.ToDate.Value - request.FromDate.Value).TotalMinutes);
                    }

                    break;

                case (int)RequestTypes.ChangeShift:
                    detail = string.Format(_MsgChangeShift, request.FromShift, request.ToShift, request.FromDate.ToDateMonthYearFormatedDate());
                    break;

                case (int)RequestTypes.CheckIn:
                case (int)RequestTypes.CheckOut:
                    detail = string.Format(_MsgCheckInOut, request.FromDate.Value.ToString("HH:mm"), request.FromShift, request.FromDate.ToDateMonthYearFormatedDate());
                    break;

                case (int)RequestTypes.LateIn:
                    detail = string.Format(_MsgLateIn, request.TotalMins, request.FromShift, date);
                    break;
                case (int)RequestTypes.EarlyOut:
                    detail = string.Format(_MsgEarlyOut, request.TotalMins, request.FromShift, date);
                    break;
                case (int)RequestTypes.Overtime:
                    if (request.IsOvertimeHours.Value)
                    {
                        detail = string.Format(
                            _MsgOvertimeHours,
                            request.FromDate.ToDateMonthYearFormatedDate(),
                            request.FromDate.Value.ToString("HH:mm"),
                            request.ToDate.Value.ToString("HH:mm"),
                            request.TotalDays.Value.ToString("0.#"),
                            request.OvertimeType.Name);
                    }
                    else
                    {
                        detail = string.Format(
                            _MsgOvertimeShift,
                            request.FromDate.ToDateMonthYearFormatedDate(),
                            request.FromShift != null ? request.FromShift : string.Empty,
                            request.OvertimeType.Name);
                    }

                    break;
                default:
                    break;
            }

            return detail;
        }

        /// <summary>
        /// Get request details.
        /// </summary>
        /// <param name="request">Request Entity.</param>
        /// <returns>Request details.</returns>
        private string GetRequestDetail(RequestEntity request)
        {
            var detail = string.Empty;
            var date = RequestService.GetMessageDateforLeaveRequest(request.FromDate, request.ToDate);

            switch (request.Type.Id)
            {
                case (int)RequestTypes.Leave:
                    if (request.TotalDays > 1)
                    {
                        detail = string.Format(_MsgLeave, Convert.ToInt32(request.TotalDays), date);
                    }
                    else if (request.TotalDays == 1)
                    {
                        detail = string.Format(_MsgLeaveOneDay, date);
                    }
                    else if ((request.TotalDays - (decimal)0.5) == 0)
                    {
                        // 0.5 day
                        detail = string.Format(_MsgLeaveHalfDay, date, request.FromDate.ToString("HH:mm"), request.ToDate.ToString("HH:mm"));
                    }
                    else
                    {
                        detail = string.Format(
                            _MsgLeaveDuringWorking,
                            date,
                            request.FromDate.ToString("HH:mm"),
                            request.ToDate.ToString("HH:mm"),
                            (request.ToDate - request.FromDate).TotalMinutes);
                    }

                    break;

                case (int)RequestTypes.ChangeShift:
                    detail = string.Format(_MsgChangeShift, request.FromShift.Name, request.ToShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
                    break;

                case (int)RequestTypes.CheckIn:
                case (int)RequestTypes.CheckOut:
                    detail = string.Format(_MsgCheckInOut, request.FromDate.ToString("HH:mm"), request.FromShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
                    break;

                case (int)RequestTypes.LateIn:
                    detail = string.Format(_MsgLateIn, request.TotalMins, request.FromShift.Name, date);
                    break;
                case (int)RequestTypes.EarlyOut:
                    detail = string.Format(_MsgEarlyOut, request.TotalMins, request.FromShift.Name, date);
                    break;
                case (int)RequestTypes.Overtime:
                    if (request.IsOvertimeHours)
                    {
                        detail = string.Format(
                            _MsgOvertimeHours,
                            request.FromDate.ToDateMonthYearFormatedDate(),
                            request.FromDate.ToString("HH:mm"),
                            request.ToDate.ToString("HH:mm"),
                            request.TotalDays.ToString("0.#"),
                            new OvertimeType { Id = request.OvertimeType }.Name);
                    }
                    else
                    {
                        detail = string.Format(
                            _MsgOvertimeShift,
                            request.FromDate.ToDateMonthYearFormatedDate(),
                            request.FromShift != null ? request.FromShift.Name : string.Empty,
                            new OvertimeType { Id = request.OvertimeType }.Name);
                    }

                    break;
                default:
                    break;
            }

            return detail;
        }
    }
}

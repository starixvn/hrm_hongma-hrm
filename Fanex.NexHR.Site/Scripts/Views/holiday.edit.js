$(document).ready(function () {
	$("#ActualFromDate, #ActualToDate, #FromDate, #ToDate").datepicker({
		changeMonth: true,
		changeYear: true
	});
	var holidayName = $("#HolidayName").val();
	var numday = $("#numDay").val();
	var fromDate = $("#FromDate").val();
	var toDate = $("#ToDate").val();
	var actualFromDate = $("#ActualFromDate").val();

	$("#numDay").change(function () {
		setTheDate();
	});

	$("#numDay").click(function () {
		setTheDate();
	});

	function setTheDate() {
		var numday = $("#numDay").val();
		var fromDate = $("#FromDate").val();
		var dat = Date.parse(fromDate);
		var theDate = new Date(dat);
		theDate.setDate(theDate.getDate() + parseInt(numday) - 1);
		$("#ToDate, #ActualToDate").val(theDate.format("dd mmm, yyyy"));
	}

	$("#ActualFromDate").change(function () {
		var numday = $("#numDay").val();
		var fromDate = $("#ActualFromDate").val();
		var dat = Date.parse(fromDate);
		var theDate = new Date(dat);
		theDate.setDate(theDate.getDate() + parseInt(numday) - 1);
		$("#ActualToDate").val(theDate.format("dd mmm, yyyy"));
	});

	$("#btnClear").click(function () {
		$("#HolidayName").val(holidayName);
		$("#ActualFromDate").val(actualFromDate);
		$("#numDay").val(numday);
		$("#FromDate").val(fromDate);
		$("#ToDate").val(toDate);
	});
});
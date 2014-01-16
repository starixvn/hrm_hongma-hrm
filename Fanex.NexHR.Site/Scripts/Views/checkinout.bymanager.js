$(document).ready(function () {
	$("#FromDate").datepicker({
		changeMonth: true,
		changeYear: true,
		dateFormat: "dd M, yy"
	});

	$("#ToDate").datepicker({
		changeMonth: true,
		changeYear: true,
		dateFormat: "dd M, yy"
	});
});
$(document).ready(function () {
	$("#Date").change(function () {
		setDateTimeRange($(this).val());
		$("#DateOption").attr("value", $(this).val());
	});

	$("#Date").val($("#DateOption").val());
	$("#FromDate, #ToDate").change(function () {
		$("#Date").val("0");
		$("#DateOption").attr("value", "0");
	});

	initDatePicker();

	var mindate = $('.datepicker').attr('mindate');
	var maxdate = $('.datepicker').attr('maxdate');
	if (mindate != 'null') {
		mindate = mindate.split("/");
		$('.datepicker').datepicker("option", "minDate", new Date(mindate[0], mindate[1] - 1, mindate[2]));
	}
	if (maxdate != 'null') {
		maxdate = maxdate.split("/");
		$('.datepicker').datepicker("option", "maxDate", new Date(maxdate[0], maxdate[1] - 1, maxdate[2]));
	}
});

function initDatePicker() {
	$('#FromDate, #ToDate').keypress(function (e) {
		var tab = 9, enter = 13;
		if (e.keyCode == tab || e.keyCode == enter)
			return true;
		return false;
	});

	$('#Date').val($("#DateOption").val());
	setDateTimeRange($("#DateOption").val());
}

function setDateTimeRange(selectedIndex) {
	var now = new Date();
	var fromDate = new Date($("#FromDate").val()).toString('dd MMM, yyyy');
	var toDate = new Date($("#ToDate").val()).toString('dd MMM, yyyy');
	var now = $('#DateTime-Now').val();

	switch (parseInt(selectedIndex)) {
		case 0: //default
			break;
		case 1: //today
			toDate = fromDate = Date.parse(now).toString('dd MMM, yyyy');
			break;
		case 2: // yesterday
			toDate = fromDate = Date.parse(now).add(-1).days().toString('dd MMM, yyyy');
			break;
		case 3: //current week
			fromDate = Date.parse(now).moveToDayOfWeek(0, -1).toString('dd MMM, yyyy');
			toDate = Date.parse(now).toString('dd MMM, yyyy');
			break;
		case 4: //last week
			fromDate = Date.parse(now).moveToDayOfWeek(0, -1).add(-7).days().toString('dd MMM, yyyy');
			toDate = Date.parse(now).moveToDayOfWeek(0, -1).add(-1).days().toString('dd MMM, yyyy');
			break;
		case 5: //current month
			fromDate = Date.parse(now).moveToFirstDayOfMonth().toString('dd MMM, yyyy');
			toDate = Date.parse(now).toString('dd MMM, yyyy');
			break;
		case 6: //last month
			fromDate = Date.parse(now).add(-1).months().moveToFirstDayOfMonth().toString('dd MMM, yyyy');
			toDate = Date.parse(now).add(-1).months().moveToLastDayOfMonth().toString('dd MMM, yyyy');
			break;
		case 7: //since last month
			fromDate = Date.parse(now).add(-1).months().moveToFirstDayOfMonth().toString('dd MMM, yyyy');
			toDate = Date.parse(now).toString('dd MMM, yyyy');
			break;
	}

	$("#FromDate").val(fromDate);
	$("#ToDate").val(toDate);
}

function DatePicker(controlId, dateFormat) {
	var seft = this;

	self.control = $(controlId);

	this.__defineGetter__('Value', function () {
		return control.val(); ;
	});

	this.__defineSetter__('Value', function (val) {
		control.val(val);
	});

	//ctor
	control.datepicker({
		changeMonth: false,
		changeYear: false,
		dateFormat: (dateFormat === undefined) ? "dd M, yy" : dateFormat.toString()
	});

	//define onchange event
	self.onchange = function (fn) {
		self.control.bind("onchange", function () {
			fn.apply(self);
		});
	};
}

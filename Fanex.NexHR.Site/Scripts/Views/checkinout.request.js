$(document).ready(function () {
	$('#shiftinfo').tooltip({
		track: true,
		delay: 0,
		showURL: false,
		bodyHandler: function () {
			return $('#shifts').html();
		}
	});

	$("#Employee").blur(function () {
		$.getJSON(_global.root + 'api/employee/GetUnavailDate', { employeeId: $('#EmployeeId').val(), date: new Date().toDateString() },
				function (response) {
					if (response != null) {
						$("#ForDate").attr("disabledDate", response.join(","));
						$('.datepicker').change();
					}
				}
			);
	});

	$('.datepicker').change(function () {
		$.getJSON(_global.root + 'api/shift/getshiftbydate', { employeeId: $("#EmployeeId").val(), date: $("#ForDate").val() },
		function (data) {
			if (data != null) {
				$('#shiftname').text(data.Name);
				$('#Shift').val(data.Name);
				$('#InTime').val(data.BeginCheckInTime);
				$('#OutTime').val(data.BeginCheckOutTime);
				bindingHourAndMinute();
				$('input[type=submit]').show();
			}
			else {
				$('#shiftname').text(_messages.notAvailable);
				$('#Shift').val(_messages.notAvailable);
				$('input[type=submit]').hide();
			}
		});
	});

	$('#Type_1, #Type_2').change(function () {
		bindingHourAndMinute();
	});

	if ($('#Type_1').val() != undefined)
		$("#Employee").blur();
	else
		getShiftByDate();

	//Process redirect to Check In/Out request.
	var successed = $("#successed").val();
	var year = $("#year").val();

	if (successed == "true") {
		if (successed == "true") {
			confirm('Successfull!\nPress OK to continue', function () {
				document.location = _global.root + "checkinout/EmployeeView/" + year;
			});
		}
	}

	updateZeroPrefix();
});

function updateZeroPrefix() {
	//add zeroPrefix
	$('input[Name=Hour]').val(zeroPrefix($('input[Name=Hour]').val()));
	$('input[Name=Minute]').val(zeroPrefix($('input[Name=Minute]').val()));
}

function bindingHourAndMinute() {
	if (($('#Type').val() == "1") || ($('#Type_1').attr("checked") == "checked")) {
		$('#Hour').val($('#InTime').val().substring(0, 2));
		$('#Minute').val($('#InTime').val().trim().substring(3, 5));
	}
	else {
		$('#Hour').val($('#OutTime').val().substring(0, 2));
		$('#Minute').val($('#OutTime').val().trim().substring(3, 5));
	}
}

//this function for load Edit
function getShiftByDate() {
	$.getJSON(_global.root + 'api/shift/getshiftbydate', { employeeId: $("#EmployeeId").val(), date: $("#ForDate").val() },
	function (data) {
		if (data != null) {
			$('#shiftname').text(data.Name);
			$('#Shift').val(data.Name);
			$('#InTime').val(data.BeginCheckInTime);
			$('#OutTime').val(data.BeginCheckOutTime);
			$('input[type=submit]').show();
		}
		else {
			$('#shiftname').text(_messages.notAvailable);
			$('#Shift').val(_messages.notAvailable);
			$('input[type=submit]').hide();
		}
	});
}
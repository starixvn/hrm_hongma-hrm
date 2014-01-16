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

$("#Employee").blur();

	$('.datepicker').change(function () {
		$.getJSON(_global.root + 'api/shift/getshiftbydate', { employeeId: $('#EmployeeId').val(), date: $('#ForDate').val() },
		function (data) {
			if (data != null) {
				$('#shiftOfDate').text(data.Name);
				$('#FromShiftName').val(data.Name);
				$('#FromShift').val(data.Id);
				$('input[type=submit]').show();
			}
			else {
				$('#shiftOfDate').text('OFF');
				$('#FromShiftName').val('OFF');
				$('#FromShift').val('');
				$('input[type=submit]').hide();
			}
			hideShift();
		});
	});

	//hide duplicate shift
	hideShift();

	//Process redirect to My change shift request.
	var successed = $("#successed").val();
	var year = $("#year").val();

	if (successed == "true") {
		confirm('Successfull!\nPress OK to continue', function () {
			document.location = _global.root + "ChangeShift/EmployeeView/" + year;
		});
	}

	$('form').submit(function (evt) {
		$('#ToShiftName').val($('#ToShift').find(":selected").text().trim());
	});
});

function hideShift() {
	var shift = $('#FromShift').val();
	$('#ToShift option').show();
	$('#ToShift option[value=' + shift + ']').hide();
}
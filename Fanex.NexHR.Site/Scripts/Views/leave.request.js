var leaveTypeId = null;

$(document).ready(function () {
    var successed = $("#successed").val();
    var year = $("#year").val();
    if (successed == "true") {
        confirm('Successfull!\nPress OK to continue', function () {
            document.location = _global.root + "Leave/EmployeeView/" + year;
        });
    }

    $('#TotalDays').prop('readonly', true);
    enableControlforHalfday();
    $('#HalfDayOff').change(function () {
        bindHalfDay();
    });

    $("#Employee").blur(function () {
        $.getJSON(_global.root + 'api/employee/GetUnavailDate', { employeeId: $('#EmployeeId').val(), date: new Date().toDateString() },
				function (response) {
				    if (response != null) {
				        $("#FromDate, #ToDate").attr("disabledDate", response.join(","));
				    }
				}
			);
    });

    $("#Employee").blur();

    $("#FromDate, #ToDate").change(function () {
        if ($("#IsLeaveDuringWorking").val().toLowerCase() != "true") {
            if ($('#HalfDayOff').attr('checked')) {
                $('#ToDate').val($('#FromDate').val());
                $.getJSON(_global.root + 'api/shift/getshiftbydate', { employeeId: $('#EmployeeId').val(), date: $('#FromDate').val() },
				function (response) {
				    if (response != null) {
				        $('#FromTime').val(response.BeginCheckInTime);
				        setValueforToTime();
				        $('input[type=submit]').show();
				    }
				    else {
				        $('input[type=submit]').hide();
				    }
				}
			);
            }
            else {
                $.getJSON(_global.root + 'api/employee/GetRangeDate',
			{ employeeId: $('#EmployeeId').val(), fromDate: $('#FromDate').val(), toDate: $('#ToDate').val() },
			function (response) {
			    if (response != null) {
			        $("#TotalDays").val(response);
			    }
			});
            }
        }
        else {
            $("#ToDate").val($("#FromDate").val());
        }
    });

    $("#LeaveTypeId").change(function () {
        $.getJSON(_global.root + 'Leave/GetLeaveType',
                    { id: $("#LeaveTypeId").val() },
                    function (response) {
                        if (response != null) {
                            var isLeaveDuringWorking = response == 1;
                            var oldStạtus = $("#IsLeaveDuringWorking").val().toLowerCase() == "true";
                            $("#IsLeaveDuringWorking").val(isLeaveDuringWorking);

                            if (isLeaveDuringWorking != oldStạtus) {
                                showControlFollowLeaveType(isLeaveDuringWorking);
                            }
                        }
                    });
    });

    $('form').submit(function (evt) {
        $('#LeaveTypeName').val($('#LeaveTypeId').find(":selected").text().trim());
    });

    showControlFollowLeaveType($("#IsLeaveDuringWorking").val().toLowerCase() == "true");

    $("#FromTime").change(function () {
        if ($("#IsLeaveDuringWorking").val().toLowerCase() != "true") {
            setValueforToTime();
        }
        else {
            setValueforTotalHours();
        }
    });

    $("#ToTime").change(function () {
        if ($("#IsLeaveDuringWorking").val().toLowerCase() == "true") {
            setValueforTotalHours();
        }
    });
});

function bindHalfDay() {
	//leaveTypeId = parseInt($('#LeaveTypeId').val());
	enableControlforHalfday();
	if ($('#HalfDayOff').prop('checked')) {
		$('#TotalDays').val(0.5);
		//get shift information
		$.getJSON(_global.root + 'api/shift/getshiftbydate',
			{ employeeId: $('#EmployeeId').val(), date: $('#FromDate').val() },
			function (response) {
				if (response != null) {
					$('#FromTime').val(response.BeginCheckInTime);
					setValueforToTime();
				}
			});
	}
	else {
		$('#TotalDays').val(1);
	}
}

function enableControlforHalfday() {
	if ($('#HalfDayOff').prop('checked')) {
		$('#ToDate').val($('#FromDate').val());
		$('.time').show();
		$('#ToDate').prop('readonly', true).datepicker('disable').prop('disabled', false);
	}
	else {
		$('.time').hide();
		$('#ToDate').prop('readonly', false).datepicker('enable');
	}
}

function showControlFollowLeaveType(isLeaveDuringWorking) {
    if (isLeaveDuringWorking) {
        $("#lblHalfDay").hide();
        $("#lblTotalDay").hide();
        $("#lblTotalHour").show();
        $('#ToDate').prop('readonly', true).datepicker('disable').prop('disabled', false);
        $('.time').show();
        $('#ToDate').val($('#FromDate').val());
    }
    else {
        $('#HalfDayOff').attr('checked', false);
        $("#lblHalfDay").show();
        $("#lblTotalDay").show();
        $("#lblTotalHour").hide();
        $('#ToDate').prop('readonly', false).datepicker('enable');
        $('.time').hide();
    }

    setValueforTotalHours();
}

function setValueforTotalHours() {
    var isLeaveDuringWorking = $("#IsLeaveDuringWorking").val().toLowerCase() == "true";
    if (isLeaveDuringWorking) {
        var arrFromTime = $("#FromTime").val().split(":");
        var arrToTime = $("#ToTime").val().split(":");

        var fromTime = parseInt(arrFromTime[0] * 60) + parseInt(arrFromTime[1]);
        var toTime = parseInt(arrToTime[0] * 60) + parseInt(arrToTime[1]);
        $("#TotalDays").val((toTime - fromTime));
    }
    else {
        $("#TotalDays").val(1);
    }
}

function setValueforToTime() {
	var arr = $('#FromTime').val().split(':');
	var x = parseInt(arr[0].trim(), 10);
	x = x + 4;
	if (x >= 24)
		x = x - 24;

	var hour = "";
	if (x < 10)
		hour = "0" + x;
	else
		hour = x;

	$('#ToTime').val(hour + ":" + arr[1]);
	$("#TotalDays").val(0.5);
}
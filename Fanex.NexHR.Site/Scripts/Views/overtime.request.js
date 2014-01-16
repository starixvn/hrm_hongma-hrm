$(document).ready(function () {
    $('#TotalMinutes').prop('readonly', true);
    var successed = $("#successed").val();
    var year = $("#year").val();
    if (successed == "true") {
        confirm('Successfull!\nPress OK to continue', function () {
            document.location = _global.root + "Overtime/EmployeeView/" + year;
        });
    }

    $('#shiftinfo').tooltip({
        track: true,
        delay: 0,
        showURL: false,
        bodyHandler: function () {
            return $('#shifts').html();
        }
    });

    $('#IsOvertimeHours').change(function () {
        var isOvertimeHours = $("#IsOvertimeHours").attr("checked");
        if (isOvertimeHours == "checked") {
            $('#shift-row').hide();
            $('#time-row').show();
            $('#totalhours-row').show();
        }
        else {
            $('#shift-row').show();
            $('#time-row').hide();
            $('#totalhours-row').hide();
        }
    });

    $('#IsOvertimeHours').change();

    $("#FromTime, #ToTime, #IsGrossDay").change(function () {
        var isGrossDay = $("#IsGrossDay").attr("checked");

        var arrFromTime = $("#FromTime").val().split(":");
        var arrToTime = $("#ToTime").val().split(":");

        var fromTime = parseInt(arrFromTime[0] * 60) + parseInt(arrFromTime[1]);
        var toTime = parseInt(arrToTime[0] * 60) + parseInt(arrToTime[1]);
        var totalMunites = 0;
        if (isGrossDay) {
            totalMunites = 1440 + (toTime - fromTime); // 1440 = 24h * 60
        }
        else {
            totalMunites = toTime - fromTime;
        }

        $("#TotalMinutes").val(totalMunites);
    });

    //    $("#Employee").blur(function () {
    //        $.getJSON(_global.root + 'api/employee/GetUnavailDate', { employeeId: $('#EmployeeId').val(), date: new Date().toDateString() },
    //				function (response) {
    //				    if (response != null) {
    //				        $("#ForDate").attr("disabledDate", response.join(","));
    //				        $('.datepicker').change();
    //				    }
    //				}
    //			);
    //    });

    //    $("#Employee").blur();

    //    $('.datepicker').change(function () {
    //        $.getJSON(_global.root + 'api/shift/getshiftbydate', { employeeId: $('#EmployeeId').val(), date: $('#ForDate').val() },
    //		function (data) {
    //		    if (data != null) {
    //		        $('#shiftOfDate').text(data.Name);
    //		        $('#FromShiftName').val(data.Name);
    //		        $('#FromShift').val(data.Id);
    //		        $('input[type=submit]').show();
    //		    }
    //		    else {
    //		        $('#shiftOfDate').text('OFF');
    //		        $('#FromShiftName').val('OFF');
    //		        $('#FromShift').val('');
    //		        $('input[type=submit]').hide();
    //		    }
    //		    hideShift();
    //		});
    //    });

    //    //hide duplicate shift
    //    hideShift();

    //    //Process redirect to My change shift request.
    //    var successed = $("#successed").val();
    //    var year = $("#year").val();

    //    if (successed == "true") {
    //        confirm('Successfull!\nPress OK to continue', function () {
    //            document.location = _global.root + "ChangeShift/EmployeeView/" + year;
    //        });
    //    }

    //    $('form').submit(function (evt) {
    //        $('#ToShiftName').val($('#ToShift').find(":selected").text().trim());
    //    });
});
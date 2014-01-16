$(document).ready(function () {

    $('#Minute').prop('readonly', true);
    //Process redirect to My late in/earlt out request.
    var successed = $("#successed").val();
    var year = $("#year").val();
    if (successed == "true") {
        confirm('Successfull!\nPress OK to continue', function () {
            document.location = _global.root + "LateInEarlyOut/EmployeeView/" + year;
        });
    }

    $("#Employee").blur(function () {
        $.getJSON(_global.root + 'api/employee/GetUnavailDate', { employeeId: $('#EmployeeId').val(), date: new Date().toDateString() },
				function (response) {
				    if (response != null) {
				        $("#FromDate").attr("disabledDate", response.join(","));
				        $("#ToDate").attr("disabledDate", response.join(","));
				        shiftChange();
				    }
				}
			);
    });

    $("#Employee").blur();
    $('.datepicker').datepicker("option", "onSelect", function (dateText, inst) {
        shiftChange();
    });

    $("#FromTime, #ToTime").blur(function () {
        var arrFromTime = $("#FromTime").val().split(":");
        var arrToTime = $("#ToTime").val().split(":");

        var fromTime = parseInt(arrFromTime[0] * 60) + parseInt(arrFromTime[1]);
        var toTime = parseInt(arrToTime[0] * 60) + parseInt(arrToTime[1]);
        $("#Minute").val((toTime - fromTime));
    });

});

function shiftChange() {
    $.getJSON(_global.root + 'api/shift/getshiftbydate', { employeeId: $('#EmployeeId').val(), date: $('#FromDate').val() },
	function (data) {
	    if (data != null) {
	        $('input[type=submit]').show();
	    }
	    else {
	        $('input[type=submit]').hide();
	    }
	});
}
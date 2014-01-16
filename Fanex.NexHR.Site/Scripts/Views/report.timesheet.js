$(document).ready(function () {

	$('#monthPickr').monPickr({
		showDateRange: false,
		sortDesc: true,
		today: new Date(),
		minDate: '2010/1/1',
		maxDate: $('#maxDate').val(),
		sortDesc: true,
		afterChangeMonth: function () {
			var Month = $('#monthPickr').monPickr("getMonth", "MM");
			var Year = $('#monthPickr').monPickr("getMonth", "yyyy");
			$('#Month').val(Month);
			$('#Year').val(Year);
		}
	});

	view();

	$('#view').click(function (evt) {
		evt.preventDefault();
		view();
	});
});

function view() {
	var $form = $('form');
	if (!$form.valid()) return;

	actingButton('#view', true);
	$('#nodata').remove();
	$.getJSON("/api/report/timesheet", $('form').serialize(), function (response) {
		if (response.length > 0) {
			rowIndex = 1;
			$('#data tbody').children().remove();
			$('#itemTemplate').tmpl(response).appendTo('#data');

			$('.tooltip-request').tooltip({
			    track: true,
			    delay: 0,
			    showURL: false,
			    bodyHandler: function () {
			        var object = $(this);
			        var result = getMessageDetail(
                        object.attr('data-requesttype'),
                        object.attr('data-fromdate'),
                        object.attr('data-todate'),
                        object.attr('data-fromshift'),
                        object.attr('data-toshift'),
                        object.attr('data-leavetype'),
                        object.attr('data-totaldays'),
                        object.attr('data-totalmins'),
                        object.attr('data-ottype'),
                        object.attr('data-isothours'));

			        return "<div class='tooltip'>"
                        + "<table class='table-tooltip checkinout'>"
                        + "<tr class='hasborder'>"
                        + "<td class='col1'>Details</td>"
                        + "<td class='col2'>"
                        + result
                        + "</td></tr><tr><td>Remark</td><td class='col2'>"
                        + ((object.attr('data-requesttype') == 1 || object.attr('data-requesttype') == 2)
                        ? "N/A" : object.attr('data-remark')) + "</td></tr></table></div>";
			    }
			});

			$('.tooltip-overtime').tooltip({
			    track: true,
			    delay: 0,
			    showURL: false,
			    bodyHandler: function () {
			        var object = $(this);

			        return "<div class='tooltip '>"
                        + "<div class='overtime-tooltip'>"
                        + object.attr('data-fromtime')
                        + "-" + object.attr('data-totime') + "</div></div>";
			    }
			});
		}
		else {
			$('#data tbody').children().remove();
			$(noInformation(11)).appendTo('#data');
		}
		reloadButtonText('#view');
	}).error(function () {
		reloadButtonText('#view');
		alert(_messages.errorTryAgain);
	});
}

var rowIndex = 1;
function index() {
	return rowIndex++;
}

function getMessageDetail(requestType, fromDate, toDate, fromShift, toShift, leaveType, totalDays, totalMins, overtimeType, isOvertimeHours) {
    var result = '';
    $.ajax({
        url: _global.root + "ajax/GetMessageDetailRequest",
        async: false,
        type: 'post',
        data: { "requesttype": requestType, "fromdate": fromDate, "todate": toDate, "fromshiftCode": fromShift,
            "toshiftcode": toShift, "leavetype": leaveType, "totaldays": totalDays, "totalmins": totalMins,
            "overtimetype": overtimeType, "isOvertimehours": isOvertimeHours
        },
        success: function (data) {
            result = data;
        }
    });

    return result;
}
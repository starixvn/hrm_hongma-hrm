var nickName;
$(document).ready(function () {
    $("#GroupId").multiselect();
    $("#Employee").autocomplete({
        source: function (request, response) {
            $.ajax({
                dataType: 'json',
                url: _global.root + 'api/employee/getemployees',
                data: {
                    query: request.term,
                    $top: 10
                },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: item.NickName + ' (' + item.FullName + ')',
                            value: item.Id
                        }
                    }));
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            nickName = ui.item.label;
            $('#EmpId').val(ui.item.value);
        },
        focus: function (event, ui) {
            nickName = ui.item.label;
            $('#Employee').val(nickName);
            $('#EmpId').val(ui.item.value);
            event.preventDefault();
        },
        change: function (event, ui) {
            nickName = ui.item.label;
            $('#Employee').val(nickName);
            $('#EmpId').val(ui.item.value);
        },
        close: function () {
            $('#Employee').val(nickName);
        }
    });

    $('input[type=text]').keypress(function (evt) {
        if (evt.keyCode == 13) {
            view();
        }
    });
    $('#view').click(function (evt) {
        evt.preventDefault();
        view();
    });
    $("#btnExcel").click(function () {
        var groups = $("#GroupId").val() != null ? $("#GroupId").val().join() : "";
        var url = _global.root + "export/Attendance?"
            + "FromDate=" + $("#FromDate").val()
            + "&ToDate=" + $("#ToDate").val()
            + "&EmpId=" + $("#EmpId").val()
            + "&Employee=" + $("#Employee").val()
            + "&GroupIds=" + groups
            + "&Type=" + $("#Type").val()
            + "&DateOption" + $("#Date").val()
            + "&ExcludeUserDelete" + $("#ExcludeUserDelete").val();
        location.href = url;
    });
    view();
});

function view() {
	var $form = $('form');
	if (!$form.valid()) return;

	actingButton('#view', true);

	//if no user is selected, clear the empId also
	if ($('#Employee').val() == '') $('#EmpId').val('');
	$('#nodata').remove();

	var groups = $("#GroupId").val() != null ? $("#GroupId").val().join() : "";

	$.getJSON("/api/report/getemployeeattendances", 
    { "FromDate": $("#FromDate").val(), 
    "ToDate": $("#ToDate").val(), 
    "EmpId": $("#EmpId").val(), 
    "Employee": $("#Employee").val(),
    "GroupIds": groups,
    "Type": $("#Type").val(),
    "DateOption": $("#Date").val(),
    "ExcludeUserDelete": $("#ExcludeUserDelete").val() },

    //$('form').serialize(), 
    function (response) {
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
	        $(noInformation(15)).appendTo('#data');
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
       success: function(data) {
                     result = data;
                }
   });

   return result;
}
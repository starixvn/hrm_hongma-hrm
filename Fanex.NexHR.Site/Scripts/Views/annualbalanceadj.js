var _checked = false;
var _totalDays = 0;
var _ids;

$(document).ready(function () {
	$('.checkuncheckall').click(function () {
		_checked = !_checked;
		$('.aba').prop('checked', _checked);
		$('.checkuncheckall').prop('checked', _checked);
		if (_checked) {
			$('tr').addClass('selected');
		}
		else {
			$('tr').removeClass('selected');
		}
	});

	$('.aba').click(function () {
		var tr = $(this).parent().parent();
		if (tr.hasClass('selected')) {
			tr.removeClass();
		}
		else {
			tr.addClass('selected');
		}
	});

	$('#addadj').click(function () {
		_ids = new Array();
		$('.aba').each(function (index, item) {
			var checked = $(item).prop('checked');
			if (checked) {
				_ids.push($(item).attr('data-id'));
			}
		});

		$('#EmpIds').val(_ids);

		if (_ids.length > 0) {
			$('#insertform').dialog({
				modal: true,
				width: 410,
				height: 275,
				open: function () {
					$('#TotalDays').val('');
				}
			});
		}
		else {
			alert('Please choose at least one employee to continue!');
		}
	});

	$('#resetblc').click(function () {

		$('#resetform').dialog({
			modal: true,
			width: 410,
			height: 235,
			open: function () {
				$('#TotalDays').val('');
			}
		});

	});

	$('#insertform form').submit(function (evt) {
		evt.preventDefault();
		var $form = $(this);
		_totalDays = parseFloat($('#TotalDays').val());
		if ($form.valid()) {
			$.post(_global.root + 'ajax/AdjustAnnualLeaves', $('form').serialize(), function (response) {

				if (response == -1) {
					alert(_messages.invalidInputData);
				}
				else if (response) {
					$.each(_ids, function (index, value) {
						var currentAnnualLeaveBalance = parseFloat($('#alb' + value).text());
						$('#alb' + value).text(currentAnnualLeaveBalance + _totalDays);
					});
					$('#insertform').dialog('close');
				}
				else {
					alert(_messages.errorTryAgain);
				}
			});
		}
	});

	$('#resetform form').submit(function (evt) {
		evt.preventDefault();
		var $form = $(this);
		_totalDays = parseFloat($('#TotalDays').val());       

		if ($form.valid()) {
			$.post(_global.root + 'ajax/ResetAnnualLeaves', $('form').serialize(), function (response) {

				if (response == -1) {
					alert(_messages.invalidInputData);
				}
				else if (response) {

					$('#resetform').dialog('close');
				}
				else {
					alert(_messages.errorTryAgain);
				}
			});
		}
	});

	$('#cancel').click(function () {
		$('#insertform').dialog('close');
	});

	$('#cancelReset').click(function () {
		$('#resetform').dialog('close');
	});
});
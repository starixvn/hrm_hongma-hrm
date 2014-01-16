var nickName;
$(document).ready(function () {
	$("#Employee").autocomplete({
		source: function (request, response) {
			$.ajax({
				url: _global.root + 'ajax/getmembersbymanager',
				data: {
					keyword: request.term,
					groupId: $('#TeamId').val()
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
			$('#Employee').val(nickName);
			$('#EmployeeId').val(ui.item.value);
		},
		focus: function (event, ui) {
			nickName = ui.item.label;
			$('#Employee').val(nickName);
			$('#EmployeeId').val(ui.item.value);
			event.preventDefault();
		},
		change: function (event, ui) {
			nickName = ui.item.label;
			$('#Employee').val(nickName);
			$('#EmployeeId').val(ui.item.value);
		},
		close: function () {
			$('#Employee').val(nickName);
		}
	});

	// when user clear the textbox, we must set employeeId field to -1 for seaching all employees
	$('form').submit(function () {
		var employee = $('#Employee').val();
		if (employee.length == 0) {
			$('#EmployeeId').val(-1);
		}
		return true;
	});
});
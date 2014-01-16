var nickName;
$(document).ready(function () {
	$("#Employee").autocomplete({
		source: function (request, response) {
			$.ajax({
				url: _global.root + 'api/employee/getemployees',
				data: {
					query: request.term
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
		close: function () {
			$('#Employee').val(nickName);
		}
	});
});
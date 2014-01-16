var _event = null;

$(document).ready(function () {
	$('#scheduledefault').click(function () {
		document.location = _global.root + 'employee/defaultschedule/' + id;
	});

	$('#update').click(function () {
		$.post(_global.root + 'ajax/updateshift',
		{ id: id, date: _event.start.format("mm/dd/yyyy"), shift: $('#shifts').val()}, 
		function (data) {
			$('#row' + id).remove();
		});
		_event.title = $('#shifts option:selected').text();
		$('#calendar').fullCalendar('updateEvent', _event);
		$('#dialog').dialog('close');
	});

	$('#calendar').fullCalendar({
		theme: true,
		header: {
			left: '',
			center: 'title',
			right: 'prev,next'
		},
		editable: false,
		events: function (start, end, callback) {
			$.ajax({
				url: '/api/employee/GetWorkingSchedule',
				dataType: 'json',
				data: {
					date: $('#calendar').fullCalendar('getDate').format("mm/dd/yyyy"),
					id: id
				},
				success: function (data) {
					var events = [];
					$(data).each(function (index, item) {
						events.push({
							title: item.Title,
							start: item.Start
						});
					});
					callback(events);
				}
			});
		},
		eventClick: function (event, element) {
			_event = event;
			updateShiftDialog();
			$('#calendar').fullCalendar('updateEvent', event);

		}
	});
});

function updateShiftDialog() {
	$('#dialog').dialog({
		modal: false,
		title: 'Shift on ' + _event.start.format("mm/dd/yyyy"),
		open: function () {
			$('#current').text(_event.title);
		}
	});
}
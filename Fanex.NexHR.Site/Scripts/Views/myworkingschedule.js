$(document).ready(function () {
	$('#calendar').fullCalendar({
		theme: true,
		header:
		{
			left: '',
			center: 'title',
			right: 'prev,next'
		},
		editable: false,
		events: function (start, end, callback) {
			$.ajax({
				url: '/api/employee/GetWorkingSchedule',
				dataType: 'json',
				data:
				{
					date: $('#calendar').fullCalendar('getDate').format("mm/dd/yyyy"),
					id: _global.employeeId
				},
				success: function (data) {
					var events = [];
					$(data).each(function (index, item) {
						events.push({
							title: item.Title,
							start: item.Start,
							className: item.Title
						});
					});
					callback(events);
					$('.OFF').find('.fc-event-title').addClass('OFF');
				}
			});
		}
	});
});
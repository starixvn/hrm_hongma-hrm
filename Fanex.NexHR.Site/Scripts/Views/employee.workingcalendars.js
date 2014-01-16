function initMonthPick(today, month) {
	//Month Picker
	var dateRangePicker = $('#DateControl');

	dateRangePicker.monPickr({
		showDateRange: false,
		sortDesc: true,
		today: today,
		month: month,
		minDate: '2010/1/1',
		maxDate: $('#maxDate').val(),    
		afterChangeMonth: function () {
			var Month = dateRangePicker.monPickr("getMonth", "MM");
			var Year = dateRangePicker.monPickr("getMonth", "yyyy");
			$('#Month').val(Month);
			$('#Year').val(Year);
		}
	});

	$('.row').hover(
		function () {
			$(this).addClass('rowhover');
		}, function () {
			$(this).removeClass('rowhover');
		});
}
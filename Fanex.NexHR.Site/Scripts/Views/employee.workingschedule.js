$(document).ready(function () {

	var date = new Date().format("mm/dd/yyyy");

	if (getParam('date') == date) {
		$('#schedulemonth').attr('checked', true);
	}
	if (getParam('date') == null) {
		$('#scheduledefault').attr('checked', true);
	}

	$('#schedulemonth').click(function () {
		document.location = _global.root + 'employee/monthschedule/' + id;
	});
	$('#scheduledefault').click(function () {
		document.location = _global.root + 'employee/defaultschedule/' + id;
	});
});
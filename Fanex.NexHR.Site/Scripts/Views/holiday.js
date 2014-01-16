$(document).ready(function () {
	var year = $('#year').val();

	$('#submit').click(function () {
		year = $('#year').val();
		location.href = "/Holiday/Index?year=" + year;
	});
});


function deleteHoliday(id) {
	confirm('Are you sure you want to delete this holiday?', function () {
		$.post(_global.root + 'ajax/DeleteHoliday', { id: id }, function (data) {
			$('#row' + id).remove();
			autoIndex();
		});
	});
}
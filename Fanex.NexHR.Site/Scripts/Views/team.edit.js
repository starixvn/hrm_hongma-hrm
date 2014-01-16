function deleteTeam(id) {
	confirm('Are you sure you want to delete this team?', function () {
		$.post(_global.root + 'ajax/DeleteTeam', { id: id }, function (error) {
			if (error.length > 0) {
				alert(error);
				return;
			}
			$('#row' + id).remove();
			autoIndex();
		});
	});
}
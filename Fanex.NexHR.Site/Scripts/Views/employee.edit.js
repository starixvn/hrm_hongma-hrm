$(document).ready(function () {

	$('#JobTitleId').chosen();

	//Birthday picker.
	$("#DOBPicker").birthdaypicker({
		futureDates: true,
		maxYear: new Date().getFullYear(),
		maxAge: 75,
		defaultDate: $('#DOB').val()
	});

	$("#month, #day, #year").change(function () {
		var year = $("#year").val();
		var month = $("#month").val();
		var day = $("#day").val();
		var date = new Date(year, month - 1, day);

		$("#DOB").val(date.format("dd mmm yyyy"));
	});

	$('#SubGroupArray' + $('#Team_Id').val()).prop('checked', false);
	$('#Team_Id').change(function () {
		var id = $(this).val();
		$('#SubGroupArray' + id).prop('checked', false);
	});

	$('input[name=SubGroupArray]').change(function () {

		var id = $(this).val();
		var teamId = $('#Team_Id').val();
		if (id == teamId) {
			$(this).prop('checked', false);
		}

		if (!$(this).prop('checked')) {
			$('#LeaderGroupsArray' + id).prop('checked', false);
		}
	});

	$('input[name=LeaderGroupsArray]').change(function () {
		var id = $(this).val();
		var teamId = $('#Team_Id').val();

		if (id != teamId && $('#SubGroupArray' + id).prop('checked') == false) {
			$(this).prop('checked', false);
			alert('Teams\' leaders receive request must be the main team or other team(s)');
		}
	});

	bindSARole(true);
	bindLeaderRole(true);
	$('#RoleArray4').click(function () {
		bindSARole(false);
	});

	$('#RoleArray2').click(function () {
		bindLeaderRole(false);
	});

	$('#upload').click(function () {
		upload();
	});

	$('#delete').click(function () {
		confirm(_messages.confirmDelete, function () {
			var resignedDate = $('#ResignedDate').val();
			if (resignedDate.length == 0) {
				alert(_messages.requireResignedDate);
				$('#ResignedDate').focus();
				return;
			}

			$.post(_global.root + 'ajax/DeleteEmployee',
			{ id: $('#Id').val(), resignedDate: resignedDate },
			function (response) {
				if (response) {
					document.location = '/employee/usermanagement';
				}
				else {
					alert(_messages.errorTryAgain);
				}
			}).error(function () { alert(_messages.errorTryAgain); });
		});
	});
});

function bindSARole(onload) {
	if ($('#RoleArray4').prop('checked')) {
		$('#RoleArray3').prop('checked', true).prop('disabled', true);
		$('#RoleArray2').prop('checked', true).prop('disabled', true);
		$('#RoleArray1').prop('checked', true).prop('disabled', true);
	}
	else if (!onload) {
		$('#RoleArray3').prop('checked', false).prop('disabled', false);
		$('#RoleArray2').prop('checked', false).prop('disabled', false);
		$('#RoleArray1').prop('checked', false).prop('disabled', false);
	}
}

function bindLeaderRole(onload) {
	if ($('#RoleArray2').prop('checked')) {
		$('#RoleArray1').prop('checked', true).prop('disabled', true);
	}
	else if (!onload) {
		$('#RoleArray1').prop('checked', false).prop('disabled', false);
	}
}

function query() {
	var query = $('#keyword').val();
	$.getJSON(_global.root + 'api/employee/getemployees', { query: query, groupIds: null }, function (data) {
		$('#data tbody').children().remove();
		$('#itemTemplate').tmpl(data).appendTo('#data');
		if (query.length > 0) {
			$('#data tbody').highlight(query);
		}
	});
}

function upload() {
	var file = document.getElementById('file').files[0];
	if (file == undefined) return;
	if (file.size / 1024 > 5120) {
		alert('File is to big (' + file.size / 1024 + 'KB) to upload. Maximum = 5120KB');
		return;
	}

	var ext = file.name.substring(file.name.lastIndexOf('.') + 1, file.name.length).toLowerCase();
	var pass = /\.(gif|jpe?g|png)$/i.test(file.name);
	if (!pass) {
		alert('Please choose an image');
		return;
	}

	//notice
	actingButton('#upload', true);
	$('#m').text('');

	var fd = new FormData();
	fd.append('code', $('#Code').val());
	fd.append('file', file);

	var xhr = new XMLHttpRequest();
	xhr.addEventListener("load", uploadComplete, false);
	xhr.addEventListener("error", uploadFailed, false);
	xhr.open("POST", _global.root + "employee/uploadavatar");
	xhr.send(fd);
}

function uploadComplete(evt) {
	if (evt.target.status == 200) {
		//$.parseJSON(evt.target.response)
		//console.log(evt.target.response);
		var url = $('#avatar').attr('src');
		var parts = url.split('?f=');
		$('#avatar').attr('src', parts[0] + '?f=' + Math.random());
	}
	reloadButtonText('#upload');
}

function uploadFailed(evt) {
	$('#m').text('Upload fail, please try again!');
	reloadButtonText('#upload');
}
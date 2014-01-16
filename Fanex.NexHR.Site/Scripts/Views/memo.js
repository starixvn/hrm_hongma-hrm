/// <reference path="../Libs/jquery.min.js" />
/// <reference path="../Libs/fxHeader.js" />

//$(document).ready(function () {
//    fxheaderInit("memo",200,1,0);    
//    fxheader();

// });

function deleteMemo(id, fileName, type) {

	confirm('Are you sure you want to delete this memo?', function () {
		$.post(_global.root + 'ajax/deletememo', { id: id }, function (data) {
			$('#row_' + id).remove();
			autoIndex();
			if(type == 1)
				deleteMemoFile(fileName);
		});

	});
}

function deleteMemoFile(fileName) {
	var fd = new FormData();
	fd.append('fileName', fileName);

	var xhr = new XMLHttpRequest();
	xhr.addEventListener("load", deleteComplete, false);
	xhr.addEventListener("error", uploadFailed, false);
	xhr.open("POST", _global.root + "file/Delete");
	xhr.send(fd);
}

function uploadFailed(evt) {
}

function deleteComplete(evt) {
	if (evt.target.status == 200) {
		$('#attach').hide();
		$("#AttachFile").val("");
		$("#Contents").val("");
	}
}

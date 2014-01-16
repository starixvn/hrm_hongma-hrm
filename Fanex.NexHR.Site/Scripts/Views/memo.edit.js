$(document).ready(function () {
	initEditor('Contents', 'basic');

	$("#SentDate").datepicker({
		changeMonth: true,
		changeYear: true
	});
	var refNo = $("#RefNo").val();
	var subJect = $("#Subject").val();
	var forInfo = $("#ForInfo").val();
	var sentDate = $("#SentDate").val();
	var sentTo = $("#SentTo").val();
	var sentFrom = $("#SentFrom").val();
	var contents = $("#Contents").val();

	$("#btnClear").click(function () {
		$("#RefNo").val(refNo);
		$("#Subject").val(subJect);
		$("#ForInfo").val(forInfo);
		$("#SentDate").val(sentDate);
		$("#SentTo").val(sentTo);
		$("#SentFrom").val(sentFrom);
		$("#Contents").val(contents);
	});

	$("#Type_0, #Type_1").change(function () {
		ChangeType();
	});

	if ($("#Type").val() == "1") {
		$("#Type_1").attr("checked", "checked");

		if ($("#AttachFile").val().length > 0) {
			$('#attach').show();
			$("#attachFile").html($("#pathName").val());
		}
		else
			$('#attach').hide();
	}
	else
		$("#Type_0").attr("checked", "checked");
	ChangeType();

	$("form").submit(function (e) {
		if ($("#Type").val() == "1") {
			form = $(this);
			e.preventDefault();
			upload();
		}
	});
});

var form;

function ChangeType() {
	if ($("#Type_1").attr("checked") == "checked") {
		$("#content").hide();
		$("#upload").show();
		$("#Type").val("1");
	}
	else {
		$("#content").show();
		$("#upload").hide();
		$("#Type").val("0");
		$("#Contents_tbl").css("width", "612");
		$("#Contents_ifr").css("height", "335");
	}
}

function upload() {
	var file = document.getElementById('fileName').files[0];
	if (file == undefined) return;
	if (file.size / 1024 > 5120) {
		alert('File is to big (' + file.size / 1024 + 'KB) to upload. Maximum = 5120KB');
		return;
	}

	var ext = file.name.substring(file.name.lastIndexOf('.') + 1, file.name.length).toLowerCase();
	//    var pass = /\.(gif|jpe?g|png)$/i.test(file.name);
	//    if (!pass) {
	//        alert('Please choose an image');
	//        return;
	//    }

	var fd = new FormData();
    
	fd.append('file', file);
	fd.append('categoryID', $("#Category").val());
	fd.append('overRide', $("#override").attr("checked") == "checked" ? "1" : "0");

	var xhr = new XMLHttpRequest();
	xhr.addEventListener("load", uploadComplete, false);
	xhr.addEventListener("error", uploadFailed, false);
	xhr.open("POST", _global.root + "file/UploadFile");
	xhr.send(fd);
}

function uploadComplete(evt) {
	if (evt.target.status == 200) {
		$("#AttachFile").val(_global.root + evt.target.response);
		$.ajax({
			url: form.action,
			type: "Post",  
			data: form.serialize(),
			success: function (html) {
				document.location = _global.root + "Memo/Index";
			}
		});
	}
}

function deleteFile() {
	deleteMemoFile($("#AttachFile").val());
}

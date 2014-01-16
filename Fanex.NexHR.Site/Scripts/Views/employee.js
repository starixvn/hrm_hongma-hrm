$(document).ready(function () {
    $("#group").multiselect();

	var jobTitleId = $('#jobtitleid').val();

	$('#title').val(jobTitleId);
	$('#title').chosen();
	$("#title").trigger("liszt:updated");

	$('#keyword').keyup(function (event) {
		if (event.keyCode == 13) {
			query(jobTitleId);
		}
	});

	var q = getParam('query');
	if (q != null) {
		$('#keyword').val(q);
	}

	//search onload
	query(jobTitleId);

	$('#query').click(function (evt) {
		evt.preventDefault();
		query(jobTitleId);
	});

});

function query(jobTitleId) {

	if (jobTitleId == undefined || jobTitleId == '') {
		jobTitleId = $('#title').val();
	}

	var query = $('#keyword').val();
	actingButton('#query', true);

	if ($("#enable").attr("checked")) {
		enable = true;
	}
	else {
		if ($("#active").attr("checked")) {
			enable = false;
		}
		else {
			enable = null;
		}
	}

	var isDelete = null;
	if ($("#IsDeleted").attr("checked")) {
		isDelete = true;
	}
	else {
		isDelete = null;
	}

    var groups = $("#group").val() != null ? $("#group").val().join() : "";
	$.getJSON(
	_global.root + 'api/employee/getemployees'

	, { query: query, groupIds: groups, enable: enable, isDelete: isDelete, titleId: jobTitleId }
	, function (data) {
		//console.log(data);
		if (data.length > 0) {
			rowIndex = 1;

			$('#data tbody').children().remove();
			$('#itemTemplate').tmpl(data).appendTo('#data');

			$('.nick').tooltip({
				track: true,
				delay: 0,
				showURL: false,
				bodyHandler: function () {
					return '<img src="/employee/avatar/' + $(this).attr('data-code') + '" />';
				}
			});
			$("#title").trigger("liszt:updated");
		}
		else {
			$('#data tbody').children().remove();
			$(noInformation(12)).appendTo('#data');
		}
		reloadButtonText('#query');
	}).error(function () {
		reloadButtonText('#query');
		alert(_messages.errorTryAgain);
	});
	$('#keyword').focus().select();
}

var rowIndex = 1;
function index() {
	return rowIndex++;
}
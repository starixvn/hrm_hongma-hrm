$(document).ready(function () {

	//Validation Leader field
	$("#Leader").rules("add", {
			required: true,
			messages: {
			required: "The Leader field is required",
			}
	});

    //Validation Assistant leader field
//    $("#AssistantLeader").rules("add", {
//         required: true,
//         messages: {
//           required: "Assistant Leader field is required",
//         }
//    });

	//Seach leader
	$("#Leader").autocomplete({
		source: function (request, response) {
			$.ajax({
				url: _global.root + 'api/employee/getemployees',
				data: {
					query: request.term,
					$top: 10
				},
				success: function (data) {
					response($.map(data, function (item) {
						return {
							label: item.NickName + ' (' + item.FullName + ')',
							value: item.Id
						}
					}));
				}
			});
		},
		minLength: 1,
		select: function (event, ui) {
			nickName = ui.item.label;
			$('#LeaderId').val(ui.item.value);
		},
		focus: function (event, ui) {
			nickName = ui.item.label;
			$('#Leader').val(nickName);
			$('#LeaderId').val(ui.item.value);
			event.preventDefault();
		},
		change: function (event, ui) {
			nickName = ui.item.label;
			$('#Leader').val(nickName);
			$('#LeaderId').val(ui.item.value);
		},
		close: function () {
			$('#Leader').val(nickName);
			$('#LeaderNickName').val(nickName);
		}
	});

	//Seach Assistant Leader
	$("#AssistantLeader").autocomplete({
		source: function (request, response) {
			$.ajax({
				url: _global.root + 'api/employee/getemployees',
				data: {
					query: request.term,
					$top: 10
				},
				success: function (data) {
					response($.map(data, function (item) {
						return {
							label: item.NickName + ' (' + item.FullName + ')',
							value: item.Id
						}
					}));
				}
			});
		},
		minLength: 1,
		select: function (event, ui) {
			nickName = ui.item.label;
			$('#AssistantLeaderId').val(ui.item.value);
		},
		focus: function (event, ui) {
			nickName = ui.item.label;
			$('#AssistantLeader').val(nickName);
			$('#AssistantLeaderId').val(ui.item.value);
			event.preventDefault();
		},
		change: function (event, ui) {
			nickName = ui.item.label;
			$('#AssistantLeader').val(nickName);
			$('#AssistantLeaderId').val(ui.item.value);
		},
		close: function () {
			$('#AssistantLeader').val(nickName);
			$('#AssistantLeaderNickName').val(nickName);
		}
	});
});

//function deleteTeam(id) {
//    confirm('Are you sure you want to delete?', function () {
//        $.post(_global.root + 'ajax/DeleteTeam', { id: id }, function (data) {
//            $('#row' + id).remove();
//            autoIndex();
//        });
//    });
//}
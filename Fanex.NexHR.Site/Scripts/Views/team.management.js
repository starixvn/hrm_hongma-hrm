$(document).ready(function () {
	$('#addNewTeam').click(function () {

		$("#TeamCode").val("");
		$("#TeamName").val("");
		$("#Leader").val("");
		$("#AssistantLeader").val("");


		$('#addTeam').load(_global.root + "employee/addteam")
					.dialog({
						modal: true,
						width: 410,
						title: "Add new",
						height: 275,
						open: function (event, ui) {
							BindSearch();
						}
					});
	});

	$("a.editteam").click(function () {
		var currenItem = $(this);
		var teamId = currenItem.attr("value");

		$("#addTeam").load(_global.root + "employee/addteam/" + teamId)
					.dialog({
						modal: true,
						width: 410,
						title: "Edit",
						height: 275,
						open: function () {
							BindSearch();
						}
					});
	});

	$('#reset').click(function () {
		//$('#addTeam').dialog('close');
		alert("Hix");
	});

	function BindSearch() {
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
			close: function () {
				$('#AssistantLeader').val(nickName);
				$('#AssistantLeaderNickName').val(nickName);
			}
		});
	}

	//Summit form
	$('form').submit(function (evt) {
		evt.preventDefault();
		var $form = $(this);
		if ($form.valid()) {
			//console.log("form is valid");
			$.post(_global.root + 'ajax/AddNewTeam', $('form').serialize(), function (response) {
				//console.log("post");
				if (response == -1) {
					alert(_messages.invalidInputData);
				}
				else if (response) {
					alert("Done");
				}
				else {
					alert(_messages.errorTryAgain);
				}
			});
		}
	});
});
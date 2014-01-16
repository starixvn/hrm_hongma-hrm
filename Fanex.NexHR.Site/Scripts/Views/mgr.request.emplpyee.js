$(document).ready(function () {
    // menu
	bindMenu('.Pending', 'pendingMenu');
	bindMenu('.Approved', 'approvedMenu');
});

function processRequest(command, eventElement) {
	var requestId = $(eventElement).attr('requestId');

	$.post(_global.root + 'ajax/processrequest',
	{ command: command, id: requestId },
		function (response) {
		});

	$(eventElement).removeClass().destroyContextMenu();
	switch (command) {
		case 'Approve':
			$(eventElement).text('Approved').addClass('Approved');
			bindMenu(eventElement, 'approvedMenu');
			bindProcessedInfo(eventElement, false);
			break;
		case 'Reject':
			$(eventElement).text('Rejected').addClass('Rejected');
			bindProcessedInfo(eventElement, false);
			break;
		case 'Disapprove':
			$(eventElement).text('Pending').addClass('Pending');
			bindMenu(eventElement, 'pendingMenu');
			bindProcessedInfo(eventElement, true);
			break;
	}
}

function bindMenu(element, menu) {
	$(element).contextMenu({
		menu: menu
	},
	function (command, eventElement) {
		processRequest(command, eventElement);
	});
}

function bindProcessedInfo(element, clear) {
	var id = $(element).attr('requestid');
	if (clear) {
		$('#pb' + id).text('-');
		$('#pd' + id).text('-');
	}
	else {
		$('#pb' + id).text(_global.nickName);
		var now = new Date();
		$('#pd' + id).text(now.format('dd mmm, yyyy'));
	}
}
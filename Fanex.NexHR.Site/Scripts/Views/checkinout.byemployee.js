$(document).ready(function() {
	// menu
	bindMenu('.Pending', 'pendingMenu');
});

function cancelRequest(id) {
	confirm('Are you sure you want to cancel this request?', function () {
	    $.post(_global.root + 'request/deleteworkingtimerequest' + "/" + id, function (data) {
			$('#row' + id).remove();
			autoIndex();
		});
	});
}

function processRequest(command, eventElement) {
	var requestId = $(eventElement).attr('requestId');
	var requestType = $("#RequestType").val();
    
	//$(eventElement).removeClass().destroyContextMenu();
	switch(command) {
		case 'Edit' : 
			location.href = _global.root + requestType + "/Update/" + requestId;
			break;
		case 'Cancel' : 
			cancelRequest(requestId);
			break;
	}
}

function bindMenu(element, menu) {
	$(element).contextMenu({
		menu : menu
	}, function(command, eventElement){
		processRequest(command, eventElement);
	});
}
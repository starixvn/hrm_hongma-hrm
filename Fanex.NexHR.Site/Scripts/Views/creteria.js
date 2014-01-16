$(document).ready(function () {
	$("#submit").click(function () {
		actingButton('#submit', true);
		sleep(1000);
		reloadButtonText('#submit');
        
	});
});
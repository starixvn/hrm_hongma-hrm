$(document).ready(function () {

	var dataGrid = $('.datagrid-view').eq(0);
	var gridView1 = dataGrid.children('div.datagrid-view1');
	var gridView2 = dataGrid.children('div.datagrid-view2');

	gridView2.children("div.datagrid-body").triggerHandler("scroll");

	gridView2.children("div.datagrid-body").bind("scroll", function () {
		gridView1.children("div.datagrid-body").scrollTop($(this).scrollTop());
		gridView2.children("div.datagrid-header").scrollLeft($(this).scrollLeft());
	});

});
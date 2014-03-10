$(document).ready(function () {

    $("#deleteExt").live("click", function () {
        var id = $(this).closest('tr').attr('id');
        confirm('Are you sure you want to delete?', function () {
            $.post(_global.root + 'ajax/DeleteExtensionNumber',
			{ extId: id },
			function (response) {
			    if (response) {
			        document.location = '/extensionno/index';
			    }
			    else {
			        alert(_messages.errorTryAgain);
			    }
			}).error(function () {
			    alert(_messages.errorTryAgain);
			});
        });
    });

    $("#deleteExternal").live("click", function () {
        var id = $(this).closest('tr').attr('id');
        confirm('Are you sure you want to delete?', function () {
            $.post(_global.root + 'ajax/DeleteExternalGroup',
			{ extId: id },
			function (response) {
			    if (response) {
			        document.location = '/extensionno/ManageExternalGroup';
			    }
			    else {
			        alert(_messages.errorTryAgain);
			    }
			}).error(function () {
			    alert(_messages.errorTryAgain);
			});
        });
    });

});


$(function () {
    var name = $("#name"),
			allFields = $([]).add(name),
			tips = $(".validateTips");

    function updateTips(t) {
        tips.text(t);
    }

    function checkLength(o, n, min, max) {
        if (o.val().length > max || o.val().length < min) {
            o.addClass("ui-state-error");
            updateTips("Length of " + n + " must be between " +
					min + " and " + max + ".");
            return false;
        } else {
            return true;
        }
    }

    $("#dialog-form").dialog({
        autoOpen: false,
        height: 180,
        width: 400,
        modal: true,
        buttons: {
            "Create": function () {
                var bValid = true;
                allFields.removeClass("ui-state-error");
                bValid = bValid && checkLength(name, "job title", 3, 100);

                if (bValid) {
                    $.post(_global.root + 'ajax/AddNewExternalGroup',
						{ groupName: name.val() },
						function (response) {
						    if (response) {
						        document.location = '/extensionno/ManageExternalGroup';
						    }
						    else {
						        alert(_messages.errorTryAgain);
						    }
						}).error(function () {
						    alert(_messages.errorTryAgain);
						});
                }
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        },
        close: function () {
            updateTips('');
            allFields.val("").removeClass("ui-state-error");
        }
    });
    $("#btnNewExternalGroup").click(function () {
        $("#dialog-form").dialog("open");
    });
});

//function update GroupName
$(function () {
    var name = $("#name_edit"),
			allFields = $([]).add(name),
			tips = $(".validateTips");

    function updateTips(t) {
        tips.text(t);
    }

    function checkLength(o, n, min, max) {
        if (o.val().length > max || o.val().length < min) {
            o.addClass("ui-state-error");
            updateTips("Length of " + n + " must be between " +
					min + " and " + max + ".");
            return false;
        } else {
            return true;
        }
    }

    $("#editExternal").live("click", function () {
        var id = $(this).closest('tr').attr('id');
        var currentName = $(this).closest('tr').find('.groupName').text().trim();
        $("#name_edit").val(currentName);

        $("#dialog-form_edit").dialog({
            autoOpen: false,
            height: 180,
            width: 400,
            modal: true,
            buttons: {
                "Update": function () {
                    var bValid = true;
                    allFields.removeClass("ui-state-error");
                    bValid = bValid && checkLength(name, "job title", 3, 100);

                    if (bValid) {
                        $.post(_global.root + 'ajax/EditExternalGroup',
						{ groupName: name.val(), extId: id },
						function (response) {
						    if (response) {
						        document.location = '/extensionno/ManageExternalGroup';
						    }
						    else {
						        alert(_messages.errorTryAgain);
						    }
						}).error(function () {
						    alert(_messages.errorTryAgain);
						});
                    }
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            },
            close: function () {
                updateTips('');
                allFields.val("").removeClass("ui-state-error");
            }
        });

        $("#dialog-form_edit").dialog("open");
    });
});




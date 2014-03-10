$(document).ready(function () {
    
    var IsAddForm = $("#IsAddForm").val();
    if (IsAddForm == "True") {
        BindingDataToDropdown();
    }
    checkForChecked();

    $("#IsInternal").change(function () {
        BindingDataToDropdown();
        checkForChecked();
    });
    $("#Employee").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: _global.root + 'api/employee/getemployees',
                data: {
                    query: request.term
                },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: item.NickName + ' (' + item.FullName + ')',
                            value: item.Id,
                            fullname: item.FullName,
                            nickname: item.NickName
                        }
                    }));
                }
            });
        },
        minLength: 1,
        select: function (event, ui) {
            nickName = ui.item.label;
            $('#Employee').val(nickName);
            $('#EmpId').val(ui.item.value);

            $('#FullName').val(ui.item.fullname);
            $('#NickName').val(ui.item.nickname);
        },
        focus: function (event, ui) {
            nickName = ui.item.label;
            $('#Employee').val(nickName);
            $('#EmpId').val(ui.item != null ? ui.item.value : "");
            $('#FullName').val(ui.item.fullname);
            $('#NickName').val(ui.item.nickname);
            event.preventDefault();
        },
        change: function (event, ui) {
            nickName = ui.item != null ? ui.item.label : "";
            $('#Employee').val(nickName);
            $('#EmpId').val(ui.item != null ? ui.item.value : "");
            $('#FullName').val(ui.item.fullname);
            $('#NickName').val(ui.item.nickname);
        },
        close: function () {
            $('#Employee').val(nickName);
        }
    });

    var successed = $("#successed").val();
    if (successed == "true") {
        document.location = _global.root + "extensionno/index";
    }

});

function BindingDataToDropdown() {
    if ($("#IsInternal").is(":checked")) {
        $.getJSON(_global.root + 'api/report/GetGroupExtension', { isInternal: true, groupId: null },
                    function (data) {
                        if (data[0] != null) {
                            $("#TeamId").empty();
                            var options = "";
                            for (var i = 0; i < data.length; i++) {
                                options += "<option value='" + data[i].Value + "'>" + data[i].Text + "</option>"
                            }
                            $("#TeamId").html(options);
                        }
                    });
    }
    else {
        $.getJSON(_global.root + 'api/report/GetGroupExtension', { isInternal: false, groupId: null },
                    function (data) {
                        if (data[0] != null) {
                            $("#TeamId").empty();
                            var options = "";
                            for (var i = 0; i < data.length; i++) {
                                options += "<option value='" + data[i].Value + "'>" + data[i].Text + "</option>"
                            }
                            $("#TeamId").html(options);
                        }
                    });
    }
}

function checkForChecked() {

    if ($("#IsInternal").is(":checked")) {

        if ($("#EmpId").val() > 0) {
            var FullName = $("#NickName").val() + ' (' + $("#FullName").val() + ')';
            $("#Employee").val(FullName);
        }

        $("#FullName, #NickName").attr('readonly', true);
        $("#EmployeeField").show();
    }
    else {
        $("#FullName, #NickName").attr('readonly', false);
        $("#EmployeeField").hide();
    }
}
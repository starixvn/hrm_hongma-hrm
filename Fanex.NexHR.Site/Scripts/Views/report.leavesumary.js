$(document).ready(function () {
    $("#TeamId").multiselect();

    $("form").submit(function (e) {
        e.preventDefault();
        var teams = $("#TeamId").val() != null ? $("#TeamId").val().join() : "";
        var url = _global.root + "report/leavesummary?year=" + $("#Year").val()
        + "&TeamIds=" + teams + "&Employee=" + $("#Employee").val()
        + "&EmployeeId=" + $("#EmployeeId").val();
        location.href = url;
    });

    $("#btnExcel").click(function () {
        var teams = $("#TeamId").val() != null ? $("#TeamId").val().join() : "";
        var url = _global.root + "export/leavesummary?year=" + $("#Year").val()
        + "&TeamIds=" + teams + "&Employee=" + $("#Employee").val()
        + "&EmployeeId=" + $("#EmployeeId").val();
        location.href = url;
    });
});

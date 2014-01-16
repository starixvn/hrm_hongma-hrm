$(document).ready(function () {
    $("#TeamId").multiselect();

    $("#btnSubmit").click(function (e) {
        e.preventDefault();
        submitForm(false);
    });

    $("#btnExcel").click(function (e) {
        e.preventDefault();
        submitForm(true);
    });

    function submitForm(isExportExcel) {
        var teams = $("#TeamId").val() != null ? $("#TeamId").val().join() : "";
        var url = _global.root + "report/TotalWorkingHours?FromDate=" + $("#FromDate").val()
        + "&ToDate=" + $("#ToDate").val()
        + "&TeamIds=" + teams 
        + "&Employee=" + $("#Employee").val()
        + "&EmployeeId=" + $("#EmployeeId").val()
        + "&DateOption=" + $("#Date").val()
        + "&IsExportExcel=" + isExportExcel;

        location.href = url;
    }
});

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logoff.aspx.cs" Inherits="Fanex.NexHR.Site.Logoff" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
    <script>

        try {
            if (document.all) {
                document.execCommand("ClearAuthenticationCache");
                window.location = "Goodbye.aspx";
            }
            else {
                window.location = "Goodbye.aspx";
            }
        }
        catch (e) {
            alert("It was not possible to clear your credentials from browser cache. Please, close your browser window to ensure that you are completely logout of system.");
            self.close();
        } 
    </script>
</body>

</html>

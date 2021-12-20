
    $("#icon-fa-eye").click(function (e) {
        e.preventDefault();
    var type = $("#password").attr('type');
    switch (type) {
                case 'password':
    {
        $("#password").attr('type', 'text');
    $("#icon-fa-eye").attr('class', "fa fa-eye")
    return;
                    }
    case 'text':
    {
        $("#password").attr('type', 'password');
    $("#icon-fa-eye").attr('class', "fa fa-eye-slash")
    return;
                    }
            }
        });
    $("#icon-fa-cfeye").click(function (e) {
        e.preventDefault();
    var type = $("#confirmPassword").attr('type');
    switch (type) {
                case 'password':
    {
        $("#confirmPassword").attr('type', 'text');
    $("#icon-fa-cfeye").attr('class', "fa fa-eye")
    return;
                    }
    case 'text':
    {
        $("#confirmPassword").attr('type', 'password');
    $("#icon-fa-cfeye").attr('class', "fa fa-eye-slash")
    return;
                    }
            }
        });


$(function () {
    $("#items_per_page").select(function () {
            var valueNumber = $("#items_per_page").val();
            $.ajax({
                type: "POST",
                url: '@Url.Action("getBds")',
                data: 'valueNumber: ' + JSON.stringify(valueNumber),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function () {
                    
                },
                error: function () {
                    alert("Error while inserting data");
                }
                });
                return false;
                });
    });



$('#lendau').click(function () {
    $("html, body").animate({ scrollTop: 0 }, 600); //Animation giúp hoạt ảnh scroll ngược lên đầu trang sẽ mượt hơn
    return false;
});

$(document).ready(function () {
    $("button").click(function () {
        $.ajax({
            type: 'GET',
            url: '/cart/index',
            success: function (result) {
                $("#ajaxgiohang").html(result);
            }
        })
    })
}


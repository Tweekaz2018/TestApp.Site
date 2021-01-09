function AddItemToCartModal(id) {
    $.get('/Cart/AddItemToCart/' + id, function (data) {
        $('#dialogContent').html(data);
        $('#modDialog').modal('show');
    });
}

function GetOrderDetails(id) {
    $.get('/Order/GetOrderDetails/' + id, function (data) {
        $('#dialogContent').html(data);
        $('#modDialog').modal('show');
    });
}

function MakeOrder() {
    let promise = AddItemToCart();
    promise.then(
        function () {
            window.location.href = "/Cart/Index";
        }
    );
}

function ContinueShopping() {
    let promise = AddItemToCart();
    promise.then(
        function () {
            window.location.href = "/Shop/Index";
        }
    );
}

function AddItemToCart() {
    return new Promise(
        function (resolve, reject) {
        let itemId = $('#ItemId').val();
        let quantity = $("#quantity").val();
        $.post('/Cart/AddItemToCart', { itemId, quantity }, function () {
            resolve("ok");
        });
    });
}
//admin
//Categories
function AddCategoryModal() {
    $.get('/Admin/AddCategoryModal', function (data) {
        $('#dialogContent').html(data);
        $('#modDialog').modal('show');
    });
}

function UpdateCategory(id) {
    let categoryNewName = $('tbody').find('[data-id="' + id + '"]').first().val();
    $.post('/Admin/UpdateCategory/', { name: categoryNewName, id }, function () {
        window.location.reload();
    });
}

function DeleteCategory(id) {
    $.post("/Admin/DeleteCategory/" + id, function () {
        window.location.reload();
    });
}
//Paymethods
function AddPayMethodModal() {
    $.get('/Admin/AddPayMethodModal/', function (data) {
        $('#dialogContent').html(data);
        $('#modDialog').modal('show');
    });
}

function UpdatePayMethod(id) {
    let payMethodName = $('tbody').find('[data-id="' + id + '"]').first().val();
    $.post('/Admin/UpdatePayMethod/', { name: payMethodName, id }, function () {
        window.location.reload();
    });
}

function DeletePayMethod(id) {
    $.post("/Admin/DeletePayMethod/" + id, function () {
        window.location.reload();
    });
}
//DeliveryMethods
function AddDeliveryMethodModal() {
    $.get('/Admin/AddDeliveryMethodModal/', function (data) {
        $('#dialogContent').html(data);
        $('#modDialog').modal('show');
    });
}

function UpdateDeliveryMethod(id) {
    let deliveryMethodName = $('tbody').find('[data-id="' + id + '"]').first().val();
    $.post('/Admin/UpdateDeliveryMethod/', { name: deliveryMethodName, id }, function () {
        window.location.reload();
    });
}

function DeleteDeliveryMethod(id) {
    $.post("/Admin/DeleteDeliveryMethod/" + id, function () {
        window.location.reload();
    });
}
//Item
function GetItemAddModal() {
    $.get('/Admin/GetItemAddModal/', function (data) {
        $('#dialogContent').html(data);
        $('#modDialog').modal('show');
    });
}

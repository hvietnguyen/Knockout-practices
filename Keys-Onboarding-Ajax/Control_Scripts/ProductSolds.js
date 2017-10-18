function clearTable() {
    var table = $('#sales');
    table.html("");
}

function refreshTable(data) {
    var table = $('#sales');
    var row = $('<tr>');
    row.append($('<td>').text(data.ProductName));
    row.append($('<td>').text(data.CustomerName));
    row.append($('<td>').text(data.StoreName));
    row.append($('<td>').text(data.Date));
    var actions = "<input class=\"btn btn-sm btn-success\" type=\"button\" value=\"Edit\" onclick=\"edit('/ProductSolds/Edit/" + data.Id + "')\" />";
    actions += " <input class=\"btn btn-sm btn-danger\" type=\"button\" value=\"Delete\" onclick=\"deleteItem('/ProductSolds/Delete/" + data.Id + "')\" />";
    row.append($('<td>').html(actions));
    table.append(row);
}

function initiateTable(data) {
    var table = $('#sales');
    var header = $('<tr>');
    header.append($('<th>').text("Product"));
    header.append($('<th>').text("Customer"));
    header.append($('<th>').text("Store"));
    header.append($('<th>').text("Date"));
    header.append($('<th>').text("Actions"));
    table.append(header);
    for(var item of data) {
        let row = $('<tr>');
        row.append($('<td>').text(item.ProductName));
        row.append($('<td>').text(item.CustomerName));
        row.append($('<td>').text(item.StoreName));
        row.append($('<td>').text(item.Date));
        var actions = "<input class=\"btn btn-sm btn-success\" type=\"button\" value=\"Edit\" onclick=\"edit('/ProductSolds/Edit/" + item.Id + "')\" />";
        actions += " <input class=\"btn btn-sm btn-danger\" type=\"button\" value=\"Delete\" onclick=\"deleteItem('/ProductSolds/Delete/" + item.Id + "')\" />";
        row.append($('<td>').html(actions));
        table.append(row);
    }
}

function validateString(prop, value) {
    if (value == null || value == '' || value == undefined) {
        return { item: prop, message: 'Can not be empty' };
    } else {
        if (isNaN(value) && value.length > 50) {
            return { item: prop, message: 'Can not be longer than 50 characters' };
        }
        return '';
    }
}

function validateNumber(prop, value) {
    if (isNaN(value)) {
        return { item: prop, message: 'Only accept a number only' };
    } else {
        if (value == true || value == false || value == '') {
            return { item: prop, message: 'Can not be empty' };
        }
        return '';
    }
}

$(document).ready(function () {
    $.ajax({
        type: 'GET',
        url: '/ProductSolds/GetProductSolds',
        dataType: 'json',
        success: function (data) {
            console.log(data);
            initiateTable(data);
        },
        error: function (error) {
            console.log(error);
        }
    });

    // Save for Create new and Edit
    $('#myModal').on('click', '#save', function () {
        debugger;
        var form = $(this).parent().parent().parent('.form-horizontal');
        var product = form.find('#ProductId').val();
        var customer = form.find('#CustomerId').val();
        var store = form.find('#StoreId').val();
        var strDate = form.find('#DateSold').val();

        var errorDate = validateString('DateSold', strDate);
        if (errorDate != '') {
            form.find("[data-valmsg-for='DateSold']").html(errorDate.message);
            return;
        }
        var date = new Date(strDate);
        console.log(date);

        if (form.find('#Id')[0] != undefined) {   // Edit a customer
            console.log('call /ProductSolds/EditSale');
            var id = form.find('#Id')[0].value;
            var productSold = {
                Id: id,
                ProductId: product,
                CustomerId: customer,
                StoreId: store,
                DateSold: date.toISOString()
            };

            $.ajax({
                type: 'POST',
                url: '/ProductSolds/EditSale',
                dataType: 'json',
                data: productSold,
                success: function (data) {
                    debugger;
                    if (data != 'Error') {
                        console.log("Edit a ProductSold successfully!");
                        clearTable();
                        initiateTable(data);
                        $('#myModal .close').click();
                    } else {
                        return;
                    }

                },
                error: function (error) {
                    console.log(error);
                }
            });
        } else { // Add new Customer
            console.log('call /ProductSolds/CreateNew');
            var productSold = {
                ProductId: product,
                CustomerId: customer,
                StoreId: store,
                DateSold: date.toISOString()
            };

            $.ajax({
                type: 'POST',
                url: '/ProductSolds/CreateNew',
                dataType: 'json',
                data: productSold,
                success: function (data) {
                    debugger;
                    if (data != 'Error') {
                        console.log("Create a new ProductSold successfully!");
                        refreshTable(data);
                        $('#myModal .close').click();
                    } else {
                        return;
                    }
                },
                error: function (error) {
                    console.log(error);
                }
            });
        }
    });

    //Delete selected Customer
    $('#myModal').on('click', '#delete', function () {
        debugger;
        var form = $(this).parent().parent();
        var id = form.find('#Id')[0].value;
        $.ajax({
            type: 'POST',
            url: '/ProductSolds/DeleteSale/' + id,
            dataType: 'json',
            success: function (data) {
                debugger;
                if (data != null) {
                    console.log("Delete a ProductSold successfully!");
                    clearTable();
                    initiateTable(data);
                    $('#myModal .close').click();
                } else {
                    return;
                }

            },
            error: function (error) {
                console.log(error);
            }
        });
    });

});
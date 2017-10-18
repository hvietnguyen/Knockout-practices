function clearTable() {
    var table = $('#products');
    table.html("");
}

function refreshTable(data) {
    var table = $('#products');
    var row = $('<tr>');
    row.append($('<td>').text(data.Name));
    row.append($('<td>').text('$'+data.Price));
    var actions = "<input class=\"btn btn-sm btn-success\" type=\"button\" value=\"Edit\" onclick=\"edit('/Products/Edit/" + data.Id + "')\" />";
    actions += " <input class=\"btn btn-sm btn-danger\" type=\"button\" value=\"Delete\" onclick=\"deleteItem('/Products/Delete/" + data.Id + "')\" />";
    row.append($('<td>').html(actions));
    table.append(row);
}

function initiateTable(data) {
    var table = $('#products');
    var header = $('<tr>');
    header.append($('<th>').text("Name"));
    header.append($('<th>').text("Price"));
    header.append($('<th>').text("Actions"));
    table.append(header);
    for(var item of data) {
        let row = $('<tr>');
        row.append($('<td>').text(item.Name));
        row.append($('<td>').text('$'+item.Price));
        var actions = "<input class=\"btn btn-sm btn-success\" type=\"button\" value=\"Edit\" onclick=\"edit('/Products/Edit/" + item.Id + "')\" />";
        actions += " <input class=\"btn btn-sm btn-danger\" type=\"button\" value=\"Delete\" onclick=\"deleteItem('/Products/Delete/" + item.Id + "')\" />";
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
        url: '/Products/GetProducts',
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
        var name = form.find('#Name')[0].value;
        var price = form.find('#Price')[0].value;

        var errorName = validateString('Name', name);
        var errorPrice = validateNumber('Price', price);
        if (errorName != '') {
            form.find("[data-valmsg-for='Name']").html(errorName.message);
        }
        if (errorPrice != '') {
            form.find("[data-valmsg-for='Price']").text(errorPrice.message);
        }

        if (form.find('#Id')[0] != undefined) {   // Edit a customer
            console.log('call /Products/EditProduct/Id');
            var id = form.find('#Id')[0].value;
            var product = {
                Id: id,
                Name: name,
                Price: price
            };

            $.ajax({
                type: 'POST',
                url: '/Products/EditProduct',
                dataType: 'json',
                data: product,
                success: function (data) {
                    debugger;
                    if (data != 'Error') {
                        console.log("Edit a Product successfully!");
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
            console.log('call /Products/CreateNew');
            var product = {
                Name: name,
                Price: price
            };

            $.ajax({
                type: 'POST',
                url: '/Products/CreateNew',
                dataType: 'json',
                data: product,
                success: function (data) {
                    debugger;
                    if (data != 'Error') {
                        console.log("Create a new Product successfully!");
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
            url: '/Products/DeleteProduct/' + id,
            dataType: 'json',
            success: function (data) {
                debugger;
                if (data != null) {
                    console.log("Delete a Product successfully!");
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
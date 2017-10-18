function clearTable() {
    var table = $('#customers');
    table.html("");
}

function refreshTable(data) {
    var table = $('#customers');
    var row = $('<tr>');
    row.append($('<td>').text(data.Name));
    row.append($('<td>').text(data.Address));
    var actions = "<input class=\"btn btn-sm btn-success\" type=\"button\" value=\"Edit\" onclick=\"edit('/Customers/Edit/" + data.Id + "')\" />";
    actions += " <input class=\"btn btn-sm btn-danger\" type=\"button\" value=\"Delete\" onclick=\"deleteItem('/Customers/Delete/" + data.Id + "')\" />";
    row.append($('<td>').html(actions));
    table.append(row);
}

function initiateTable(data) {
    var table = $('#customers');
    var header = $('<tr>');
    header.append($('<th>').text("Name"));
    header.append($('<th>').text("Address"));
    header.append($('<th>').text("Actions"));
    table.append(header);
    for(var item of data) {
        let row = $('<tr>');
        row.append($('<td>').text(item.Name));
        row.append($('<td>').text(item.Address));
        var actions = "<input class=\"btn btn-sm btn-success\" type=\"button\" value=\"Edit\" onclick=\"edit('/Customers/Edit/" + item.Id + "')\" />";
        actions += " <input class=\"btn btn-sm btn-danger\" type=\"button\" value=\"Delete\" onclick=\"deleteItem('/Customers/Delete/" + item.Id + "')\" />";
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

$(document).ready(function () {
    $.ajax({
        type: 'GET',
        url: '/Customers/GetCustomers',
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
        var address = form.find('#Address')[0].value;

        var errorName = validateString('Name', name);
        var errorAddress = validateString('Address', address);
        if (errorName != '') {
            form.find("[data-valmsg-for='Name']").html(errorName.message);
        }
        if (errorAddress != '') {
            form.find("[data-valmsg-for='Address']").text(errorAddress.message);
        }

        if (form.find('#Id')[0] != undefined) {   // Edit a customer
            console.log('call /Customers/EditCustomer/Id');
            var id = form.find('#Id')[0].value;
            var customer = {
                Id:id,
                Name: name,
                Address: address
            };

            $.ajax({
                type: 'POST',
                url: '/Customers/EditCustomer',
                dataType: 'json',
                data: customer,
                success: function (data) {
                    debugger;
                    if (data != 'Error') {
                        console.log("Edit a Customer successfully!");
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
            console.log('call /Customers/CreateNew');
            var customer = {
                Name:name,
                Address:address
            };

            $.ajax({
                type: 'POST',
                url: '/Customers/CreateNew',
                dataType: 'json',
                data: customer,
                success: function (data) {
                    debugger;
                    if (data != 'Error') {
                        console.log("Create a new Customer successfully!");
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
            url: '/Customers/DeleteCustomer/' + id,
            dataType: 'json',
            success: function (data) {
                debugger;
                if (data != null) {
                    console.log("Delete a Customer successfully!");
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
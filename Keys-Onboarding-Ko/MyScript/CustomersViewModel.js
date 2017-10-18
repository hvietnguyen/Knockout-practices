ko.extenders.maxLength = function (target, max) {
    target.hasError = ko.observable();
    target.validationMessage = ko.observable();

    function validate(newValue) {
        target.hasError(newValue.length > max ? true : false);
        target.validationMessage("Cannot be longer than " + max + " characters.");
    }

    //initial validation
    validate(target);

    //validate whenever the value changes
    target.subscribe(validate);

    //return the original observable
    return target;
}

function Customer(data) {
    this.Id = ko.observable(data.Id);
    this.Name = ko.observable(data.Name).extend({ required: true, maxLength: 50 });
    this.Address = ko.observable(data.Address).extend({ required: true, maxLength: 50 });
}

var CustomersViewModel = function() {
    var self = this;
    self.customers = ko.observableArray([]);
    self.currentCustomer = ko.observable(null);
    self.isValidated = ko.observable(true);
    $.getJSON("/Customers/GetCustomers", function (data) {
        mappedCustomers = $.map(data, function (item) { return new Customer(item) });
        self.customers(mappedCustomers);
    });

    self.createNew = function () {
        debugger;
        $('#createModal').modal('show');
        self.currentCustomer(new Customer({ Name:'', Address:'' }));
        console.log(self.currentCustomer())
        
    }

    self.edit = function (customer) {
        debugger;
        $('#editModal').modal('show');
        self.currentCustomer(customer);
        console.log(self.currentCustomer());
       
    }

    self.delete = function (customer) {
        debugger;
        $('#deleteModal').modal('show');
        self.currentCustomer(customer);
    }

    self.save = function () {
        debugger;
        
        if (self.currentCustomer().Id() == undefined) {  // Create New
            //self.customers.push(self.currentCustomer);
            
            let customer = {
                Name: self.currentCustomer().Name(),
                Address: self.currentCustomer().Address()
            };
            $.post("/Customers/CreateNew/", customer, function (data) {
                debugger;
                if (data.error == null || data.error == false) {
                    self.customers.push(new Customer(data));
                    self.isValidated(true);
                    $('#createModal').modal('hide');
                } else {
                    console.log(data.message);
                    self.isValidated(false);
                    $('.errorMessage').text(data.message);
                    return;
                }
                
            });
        } else { // Edit
            let customer = {
                Id: self.currentCustomer().Id(),
                Name: self.currentCustomer().Name(),
                Address: self.currentCustomer().Address(),
            };
            $.post("/Customers/SaveEdit/", customer, function (data) {
                debugger;
                if (data.error == null || data.error == false) {
                    self.isValidated(true);
                    $('#editModal').modal('hide');
                } else {
                    console.log(data.message);
                    self.isValidated(false);
                    $('.errorMessage').text(data.message);
                    return;
                }
            });
        }
    }

    self.deleteConfirm = function () {
        debugger;
        let id = self.currentCustomer().Id();
            
        $.post("/Customers/DeleteConfirm/"+id, function (data) {
            debugger;
            if (data == "Successful") {
                self.customers.remove(self.currentCustomer());
                $('#deleteModal').modal('hide');
            } else {
                return;
            }

        });
    }

    self.cancel = function () {
        debugger;
        $.getJSON("/Customers/GetCustomers", function (data) {
            mappedCustomers = $.map(data, function (item) { return new Customer(item) });
            self.customers(mappedCustomers);
        });
        self.isValidated(true);

        if($("#editModal")!=null)
            $("#editModal").modal('hide');

        if ($("#createModal") != null)
            $("#createModal").modal('hide');
    }
   
}

var viewModel = new CustomersViewModel();

//The Validation initialization
ko.validation.init({ messagesOnModified: false, errorClass: 'errorStyle', insertMessages: true });
ko.applyBindings(viewModel);
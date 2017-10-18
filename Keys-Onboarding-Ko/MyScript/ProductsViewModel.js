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

ko.extenders.number = function (target, bool) {
    target.hasError = ko.observable();
    target.validationMessage = ko.observable();

    function validate(newValue) {
        var v = newValue instanceof Function ? newValue() : newValue;
        target.hasError(isNaN(v) ? true : false);
        target.validationMessage("Please enter the number");
    }

    //initial validation
    validate(target);

    //validate whenever the value changes
    target.subscribe(validate);

    //return the original observable
    return target;
}

function Product(data) {
    var self = this;
    self.Id = ko.observable(data.Id);
    self.Name = ko.observable(data.Name).extend({required: true, maxLength: 50 });
    self.Price = ko.observable(data.Price).extend({required: true, number: true });
    self.FormattedPrice = ko.computed(function () {
        var value = parseInt(self.Price());
        return value ? "$" + value.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,') : "None";
    });
}

var ProductsViewModel = function () {
    var self = this;
    self.products = ko.observableArray([]);
    self.currentProduct = ko.observable(null);
    self.isValidated = ko.observable(true);

    $.getJSON("/Products/GetProducts", function (data) {
        mappedProducts = $.map(data, function (item) { return new Product(item) });
        self.products(mappedProducts);
    });

    self.createNew = function () {
        debugger;
        $('#createModal').modal('show');
        var data = {Name:'', Price:''};
        self.currentProduct(new Product(data));
        console.log(self.currentProduct())

    }

    self.edit = function (product) {
        debugger;
        $('#editModal').modal('show');
        self.currentProduct(new Product({
            Id: product.Id(),
            Name: product.Name(),
            Price: product.Price()
        }));
        console.log(self.currentProduct());
    }

    self.delete = function (product) {
        debugger;
        $('#deleteModal').modal('show');
        self.currentProduct(product);
    }

    self.save = function () {
        debugger;
        if (self.currentProduct().Id() == undefined) {  // Create New
            //self.customers.push(self.currentCustomer);

            let product = {
                Name: self.currentProduct().Name(),
                Price: self.currentProduct().Price()
            };
            $.post("/Products/CreateNew/", product, function (data) {
                debugger;
                if (data.error == null || data.error == false) {
                    self.products.push(new Product(data));
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
            let product = {
                Id: self.currentProduct().Id(),
                Name: self.currentProduct().Name(),
                Price: self.currentProduct().Price(),
            };
            $.post("/Products/SaveEdit/", product, function (data) {
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
        let id = self.currentProduct().Id();

        $.post("/Products/DeleteConfirm/" + id, function (data) {
            debugger;
            if (data == "Successful") {
                self.products.remove(self.currentProduct());
                $('#deleteModal').modal('hide');
            } else {
                return;
            }

        });
    }

    self.cancel = function () {
        debugger;
        $.getJSON("/Products/GetProducts", function (data) {
            mappedProducts = $.map(data, function (item) { return new Product(item) });
            self.products(mappedProducts);
        });
        self.isValidated(true);
        if ($("#editModal") != null)
            $("#editModal").modal('hide');

        if ($("#createModal") != null)
            $("#createModal").modal('hide');
    }

}

var viewModel = new ProductsViewModel();
//The Validation initialization
ko.validation.init({ messagesOnModified: false, errorClass: 'errorStyle', insertMessages: true });
ko.applyBindings(viewModel);
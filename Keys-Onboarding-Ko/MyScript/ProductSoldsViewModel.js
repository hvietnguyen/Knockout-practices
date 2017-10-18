// date: data-binding for date time element
ko.bindingHandlers.date = {
    // https://disqus.com/home/discussion/jasonmitchellcom/binding_and_formatting_dates_using_knockout_and_moment_js/#comment-1374417079
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        ko.utils.registerEventHandler(element, 'change', function () {
            debugger;
            var value = valueAccessor();
            if (element.value !== null && element.value !== undefined && element.value.length > 0) {
                value(element.value);
            }
            else {
                value('');
            }
        });
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var value = valueAccessor();
        var allBindings = allBindingsAccessor();
        var valueUnwrapped = ko.utils.unwrapObservable(value());

        // Date formats: http://momentjs.com/docs/#/displaying/format/
        var pattern = allBindings.format || 'YYYY-MM-DD';

        var output = "-";
        //if (valueUnwrapped !== null && valueUnwrapped !== undefined && valueUnwrapped.length > 0) {
        //    output = moment(valueUnwrapped).format(pattern);
        //}

        output = moment(valueUnwrapped).format(pattern);

        if ($(element).is("input") === true) {
            $(element).val(output);
        } else {
            $(element).text(output);
        }
    }
};

//------------------------------------------------------------------------------

ko.extenders.validationDate = function (target, bool) {
    target.hasError = ko.observable();
    target.validationMessage = ko.observable();

    function validate(newValue) {
        debugger;
        if (bool) {
            var v = newValue instanceof Function ? newValue() : newValue;
            var isEmpty = ko.validation.utils.isEmptyVal(v);
            var isValid = moment(v, 'YYYY-MM-DD').isValid();
            target.hasError((isEmpty || !isValid) ? true : false);
            target.validationMessage("Date is not valid");
        }
        console.log(target.hasError());
    }

    //initial validation
    validate(target);

    //validate whenever the value changes
    target.subscribe(validate);

    //return the original observable
    return target;
}

function Customer(data) {
    var self = this;
    self.Id = data.Id;
    self.Name = data.Name;
    self.Address = data.Address;
}

function Product(data) {
    var self = this;
    self.Id = data.Id;
    self.Name = data.Name;
    self.Price = data.Price;
    self.formattedPrice = ko.computed(function () {
        var value = parseInt(self.Price);
        return value ? "$" + value.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,') : "None";
    });
}

function Store(data) {
    var self = this;
    self.Id = data.Id;
    self.Name = data.Name;
    self.Address = data.Address;
}

function ProductSold(data) {
    var self = this;
    self.Id = ko.observable(data.Id);
    self.Customer = ko.observable(data.Customer).extend({required: true});
    self.Product = ko.observable(data.Product).extend({ required: true });
    self.Store = ko.observable(data.Store).extend({ required: true });
    self.Date = ko.observable(new Date(data.Date)).extend({ validationDate: true });
    self.FormattedDate = ko.computed(function () {
        var date = new Date(self.Date());
        return date.getDate() + "-" + (parseInt(date.getMonth()) + 1) + "-" + date.getFullYear();
    });
}

var ProductSoldsViewModel = function () {
    var self = this;
    self.isValidated = ko.observable(true);
    self.productSolds = ko.observableArray([]);
    self.currentProductSold = ko.observable(null);
    
    $.getJSON("/ProductSolds/GetSales", function (data) {
        mappedProductSolds = $.map(data, function (item) { return new ProductSold(item) });
        self.productSolds(mappedProductSolds);
    });

    // Get Customers
    self.customers = [];
    self.selectedCustomer = ko.observable().extend({ required: true });
    $.getJSON("/Customers/GetCustomers", function (data) {
        mappedCustomers = $.map(data, function (item) { return new Customer(item) });
        self.customers=mappedCustomers;
        console.log(self.customers);
    });

    // Get Products
    self.products = [];
    self.selectedProduct = ko.observable().extend({ required: true });
    $.getJSON("/Products/GetProducts", function (data) {
        mappedProducts = $.map(data, function (item) { return new Product(item) });
        self.products=mappedProducts;
        console.log(self.products);
    });

    // Get Products
    self.stores = [];
    self.selectedStore = ko.observable().extend({ required: true });
    $.getJSON("/Stores/GetStores", function (data) {
        mappedStores = $.map(data, function (item) { return new Store(item) });
        self.stores=mappedStores;
        console.log(self.stores);
    });

    self.createNew = function () {
        if (self.products.length <= 0 || self.customers.length <= 0 || self.stores.length <= 0)
            return;
        
        $('#createModal').modal('show');
        self.currentProductSold(new ProductSold({
            Customer: undefined,
            Product: undefined,
            Store: undefined,
            Date: moment()
        }));
        console.log("New ProductSold: ", self.currentProductSold());
    }

    self.edit = function (productSold) {
        debugger;
        $('#editModal').modal('show');
        self.selectedProduct(productSold.Product().Id);
        self.selectedCustomer(productSold.Customer().Id);
        self.selectedStore(productSold.Store().Id);
        self.currentProductSold(productSold);
    }

    self.delete = function (productSold) {
        debugger;
        $('#deleteModal').modal('show');
        self.currentProductSold(productSold);
    }

    self.save = function () {
        debugger;
        if (self.currentProductSold().Id() == undefined) {  // Create New
            let productSold = {
                ProductId: self.currentProductSold().Product() == undefined ? '' : self.currentProductSold().Product().Id,
                CustomerId: self.currentProductSold().Customer() == undefined ? '' : self.currentProductSold().Customer().Id,
                StoreId: self.currentProductSold().Store() == undefined ? '' : self.currentProductSold().Store().Id,
                DateSold: new Date(self.currentProductSold().Date())!='Invalid Date'?new Date(self.currentProductSold().Date()).toISOString():null
            };
            $.post("/ProductSolds/CreateNew/", productSold, function (data) {
                debugger;
                if (data.error == null || data.error == false) {
                    self.productSolds.push(new ProductSold(data));
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
            debugger;
            var date = new Date(self.currentProductSold().Date());
            let productSold = {
                Id: self.currentProductSold().Id(),
                ProductId: self.selectedProduct(),
                CustomerId: self.selectedCustomer(),
                StoreId: self.selectedStore(),
                DateSold: date.toISOString()
            };
            $.post("/ProductSolds/SaveEdit/", productSold, function (data) {
                debugger;
                if (data.error == null || data.error == false) {
                    $.getJSON("/ProductSolds/GetSales", function (data) {
                        mappedProductSolds = $.map(data, function (item) { return new ProductSold(item) });
                        self.productSolds(mappedProductSolds);
                    });
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
        let id = self.currentProductSold().Id();

        $.post("/ProductSolds/DeleteConfirm/" + id, function (data) {
            debugger;
            if (data == "Successful") {
                self.productSolds.remove(self.currentProductSold());
                $('#deleteModal').modal('hide');
            } else {
                return;
            }

        });
    }

    self.cancel = function () {
        $.getJSON("/ProductSolds/GetSales", function (data) {
            mappedProductSolds = $.map(data, function (item) { return new ProductSold(item) });
            self.productSolds(mappedProductSolds);
        });
        
        self.isValidated(true);
        if ($("#editModal") != null)
            $("#editModal").modal('hide');

        if ($("#createModal") != null)
            $("#createModal").modal('hide');
    }

}

var viewModel = new ProductSoldsViewModel();
//The Validation initialization
ko.validation.init({ messagesOnModified: false, errorClass: 'errorStyle', insertMessages: true });
ko.applyBindings(viewModel);
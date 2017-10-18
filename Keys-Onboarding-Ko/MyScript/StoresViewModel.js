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

function Store(data) {
    this.Id = ko.observable(data.Id);
    this.Name = ko.observable(data.Name).extend({required: true, maxLength: 50});
    this.Address = ko.observable(data.Address).extend({required: true, maxLength: 50});
}

var StoresViewModel = function () {
    var self = this;
    self.stores = ko.observableArray([]);
    self.currentStore = ko.observable(null);
    self.isValidated = ko.observable(true);
    $.getJSON("/Stores/GetStores", function (data) {
        var mappedStores = $.map(data, function (item) { return new Store(item) });
        self.stores(mappedStores);
    });

    self.createNew = function () {
        debugger;
        $('#createModal').modal('show');
        self.currentStore(new Store({ Name: '', Address: '' }));
        console.log(self.currentStore())

    }

    self.edit = function (store) {
        debugger;
        $('#editModal').modal('show');
        self.currentStore(store);
        console.log(self.currentStore());

    }

    self.delete = function (store) {
        debugger;
        $('#deleteModal').modal('show');
        self.currentStore(store);
    }

    self.save = function () {
        debugger;
        if (self.currentStore().Id() == undefined) {  // Create New
            let store = {
                Name: self.currentStore().Name(),
                Address: self.currentStore().Address()
            };
            $.post("/Stores/CreateNew/", store, function (data) {
                debugger;
                if (data.error == null || data.error == false) {
                    self.stores.push(new Store(data));
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
            let store = {
                Id: self.currentStore().Id(),
                Name: self.currentStore().Name(),
                Address: self.currentStore().Address(),
            };
            $.post("/Stores/SaveEdit/", store, function (data) {
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
        let id = self.currentStore().Id();

        $.post("/Stores/DeleteConfirm/" + id, function (data) {
            debugger;
            if (data == "Successful") {
                self.stores.remove(self.currentStore());
                $('#deleteModal').modal('hide');
            } else {
                return;
            }

        });
    }

    self.cancel = function () {
        debugger;
        $.getJSON("/Stores/GetStores", function (data) {
            mappedStores = $.map(data, function (item) { return new Store(item) });
            self.stores(mappedStores);
        });
        self.isValidated(true);

        if ($("#editModal") != null)
            $("#editModal").modal('hide');

        if ($("#createModal") != null)
            $("#createModal").modal('hide');
    }

}

var viewModel = new StoresViewModel();
//The Validation initialization
ko.validation.init({ messagesOnModified: false, errorClass: 'errorStyle', insertMessages: true });
ko.applyBindings(viewModel);
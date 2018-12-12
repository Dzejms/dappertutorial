/// <reference path="../../Scripts/knockout-2.2.1.js" />
define(['durandal/http', 'durandal/app', 'durandal/plugins/router'],
    function (http, app, router) {
        var self = this;

        var ContactViewModel = function (data) {
            var self = this;

            self.id = data.id;
            self.firstName = ko.observable(data.firstName || '').extend({ required: true });
            self.lastName = ko.observable(data.lastName || '').extend({ required: true });
            self.email = ko.observable(data.email).extend({ required: true, email: true });
            self.company = ko.observable(data.company);
            self.title = ko.observable(data.title);

            var observableAddr = ko.utils.arrayMap(data.addresses, function (item) { return new AddressViewModel(item); });
            self.addresses = ko.observableArray(observableAddr);

            self.addAddress = function () {
                self.addresses.push(new AddressViewModel({}));
            }

            self.deleteAddress = function (item) {
                item.isDeleted(true);
            }

            self.errors = ko.validation.group(this);
        }

        var AddressViewModel = function (data) {
            var self = this;

            self.id = data.id;
            self.streetAddress = ko.observable(data.streetAddress).extend({ required: true });
            self.addressType = ko.observable(data.addressType);
            self.city = ko.observable(data.city);
            self.stateId = ko.observable(data.stateId);
            self.postalCode = ko.observable(data.postalCode);
            self.isDeleted = ko.observable(data.isDeleted);
        }

        self.states = ko.observableArray();
        self.contact = ko.observable(new ContactViewModel({}));

        var activate = function (data) {
            http.get("/api/lookups/states").then(function (data) {
                self.states(data);
            });

            if (data.id) {
                return http.get("/api/contacts/" + data.id).then(function (response) {
                    self.contact(new ContactViewModel(response));
                });
            }
            else {
                self.contact(new ContactViewModel({}));
            }
            
            return true;
        }

        var save = function () {
            if (self.contact().errors().length > 0) {
                self.contact().errors.showAllMessages();
                return;
            }

            if (self.contact.id === null) {
                http.put("/api/contacts/" + self.contact.id, self.contact).then(function (response) {
                    router.navigateTo("home");
                });
            }
            else {
                http.post("/api/contacts", self.contact).then(function (response) {
                    router.navigateTo("home");
                });
            }
        }

        var vm = {
            contact: contact,
            states: states,
            activate: activate,
            save: save
        };

        return vm;
    });
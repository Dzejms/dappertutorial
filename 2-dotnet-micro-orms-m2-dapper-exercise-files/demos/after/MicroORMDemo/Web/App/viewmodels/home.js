define(['durandal/http', 'durandal/app'],
    function (http, app) {
        var home = function () {
            var self = this;

            self.contacts = ko.observableArray();

            self.deleteContact = function (item) {
                app.showMessage("Are you sure you want to delete?", "Delete?", ['Yes', 'No']).then(function (response) {
                    if (response === 'Yes') {
                        http.delete("/api/contacts/" + item.id).then(function (response) {
                            self.contacts.remove(item);
                        });
                    }
                });
            }

            http.get("/api/contacts").then(function (response) {
                self.contacts(response);
            });
        };

        home.prototype.viewAttached = function (view) {
            //you can get the view after it's bound and connected to it's parent dom node if you want
        };

        return home;
    });
var loghub = loghub || {};

loghub.Routes = new function () {
    var sammy = Sammy();

    this.register = function (pages) {
        $.each(pages, function (index, page) {
            sammy.get(page.url, function () {
                page.render(this.params);
            });
        });
    };

    this.run = function (page) {
        sammy.run(page.url);
    };
};
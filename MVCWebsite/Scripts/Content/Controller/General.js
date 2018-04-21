$(document).ready(function () {
    $(".table-title").children(":not(.non-sortable)").click(function (e) {
        var element = e.currentTarget;
        var location = window.location.href;
        //switch sorting
        var sort = location.includes("sort=desc") ? "asc" : "desc";
        var column = element.innerHTML.trim();
        var search = new RegExp("SearchString=[^&]+");
        var searchexe = search.exec(location);
        var text = "";
        if (searchexe != null) {
            //hacky do not like but cant get captioning
            var text_1 = searchexe[0].replace("SearchString=", "");
        }
        location = location.split('?')[0] + "?sort=" + sort + "&column=" + column;
        if (text.length > 1) {
            location += "&SearchString=" + text;
        }
        window.location.href = location;
    });
    $("#SearchForm").submit(function (e) {
        var checked = $("input[type=checkbox]:checked").length;
        if (checked == 0) {
            if (!$("#formErrorText").length) {
                var p = $("<p>", { id: "formErrorText", "class": "error" });
                p.append("At least one checkbox must be selected");
                $("#SearchForm").append(p);
            }
            else {
                $("#formErrorText").fadeOut(100).fadeIn(100);
            }
            return false;
        }
    });
});
//# sourceMappingURL=General.js.map
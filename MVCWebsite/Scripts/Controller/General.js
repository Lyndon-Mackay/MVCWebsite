$(document).ready(() => {

    $(".table-title").children(":not(.non-sortable)").click(e => {
        let element = e.currentTarget;
        let location = window.location.href;

        //switch sorting
        let sort = location.includes("sort=desc") ? "asc" : "desc";
        let column = element.innerHTML.trim();
        let search = new RegExp("SearchString=[^&]+");
        let searchexe = search.exec(location)
        let text = "";
        if (searchexe != null) {
            //hacky do not like but cant get captioning
            let text = searchexe[0].replace("SearchString=", "");
        }
        location = location.split('?')[0] + "?sort=" + sort + "&column=" + column;
        if (text.length > 1) {
            location += "&SearchString=" + text;
        }
        window.location.href = location;
    });
    $("#SearchForm").submit(e => {
        let checked = $("input[type=checkbox]:checked").length;
        if (checked == 0) {
            if (!$("#formErrorText").length) {
                let p = $("<p>", { id: "formErrorText", "class": "error" });
                p.append("At least one checkbox must be selected");
                $("#SearchForm").append(p);
            }
            else {
               $("#formErrorText").fadeOut(100).fadeIn(100);
            }
            return false;
        }

    })
})

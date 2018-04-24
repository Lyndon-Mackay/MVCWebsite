$(document).ready(() => {
    $("input[type=checkbox]")
    {
        let locationParamsString = window.location.search.substr(1);
        let params = getAllParams(locationParamsString);
        for (var i = 0; i < params.length; i++) {
            let element = $(`input[name=${params[i].name}]`);
            if (element.is(":checkbox")) {
                element.prop('checked', params[i].value);
                //getting name without the search prefix and adding cell postfix for class
                let name = element.attr('name').replace("Search", "") + "Cell";

                $(`td.${name}`).mark(params.find(p => p.name == "SearchString").value, {
                    "element": "span",
                    "className": "highlight"
                });
            }
        }
    }
    //clicking on a sortable column change sort and column otherwise as normal
    $(".table-title").children(":not(.non-sortable)").click(e => {
        let element = e.currentTarget;
        let location = window.location.href;

        //gets url after the ?
        let locationParamsString = window.location.search.substr(1);
        let params = getAllParams(locationParamsString);
        let sort = params.find(p => p.name == "sort");
        if (sort == undefined) {
            sort = new Param("sort", "asc");
            params.push(sort);
        }
        console.log("params" + params)
        //switch sorting
        sort.value = sort.value == "desc" ? "asc" : "desc";

        let column = element.innerHTML.trim();
        let paramsColumn = params.find(p => p.name == "column")
        if (paramsColumn == undefined) {
            params.push(new Param("column", column));
        }
        else {
            paramsColumn.value = column;
        }

        location = getParamsInLocation(location, params);
        //beaware of location switching clearing the console
        window.location.href = location;
    });
    //make sure at least one checkbox is checked
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

    });

})
